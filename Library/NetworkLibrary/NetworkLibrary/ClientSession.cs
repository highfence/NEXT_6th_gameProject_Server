using System;
using System.Collections.Generic;
using System.Net.Sockets;
using MessagePack;

namespace NetworkLibrary
{
    public class ClientSession
    {
		public Socket Socket { get; set; }

		public SocketAsyncEventArgs ReceiveEventArgs { get; private set; }
		public SocketAsyncEventArgs SendEventArgs	 { get; private set; }

		BytePacker		    bytePacker;
		IPacketLogicHandler packetLogicHandler;

		// TODO :: 컨커런시 큐로 바꾸고 락 풀기.
		Queue<Packet> sendQueue;

		public ClientSession(IPacketLogicHandler packetLogicHandler)
		{
			sendQueue  = new Queue<Packet>();
			bytePacker = new BytePacker();

			this.packetLogicHandler = packetLogicHandler;
		}

		// 해당 세션에서 사용할 EventArgs를 풀에서 받아와 세팅해주는 메소드.
		public void SetEventArgs(SocketAsyncEventArgs receiveEventArgs, SocketAsyncEventArgs sendEventArgs)
		{
			ReceiveEventArgs = receiveEventArgs;
			SendEventArgs	 = sendEventArgs;
		}

		public void Send(Packet sendPacket)
		{
			lock (sendQueue)
			{
				// 큐가 비어있다면 큐에 추가하고 곧바로 비동기 전송 메서드 호출.
				if (sendQueue.Count <= 0)
				{
					sendQueue.Enqueue(sendPacket);
					StartSend();
					return;
				}

				// 큐에 무언가 패킷이 들어있다면 아직 이전 전송이 완료되지 않은 상태이므로
				// 큐에 추가만 하고 리턴한다.
				sendQueue.Enqueue(sendPacket);
			}
		}

		private void StartSend()
		{
			lock (sendQueue)
			{
				var sendPacket = sendQueue.Peek();

				// 헤더에 패킷 사이즈를 기록한다.
				var packetByte = MessagePackSerializer.Serialize(sendPacket);

				var header = new PacketHeader()
				{
					BodySize = packetByte.Length,
					PacketId = sendPacket.PacketId
				};

				var headerByte = MessagePackSerializer.Serialize(header);

				// 이번에 보낼 패킷 사이즈만큼 버퍼 크기를 설정하고.
				var buffer = new byte[headerByte.Length + packetByte.Length];

				// 패킷 내용을 SocketAsyncEventArgs 버퍼에 복사한다.
				Array.Copy(headerByte, 0, buffer, 0, headerByte.Length);
				Array.Copy(packetByte, 0, buffer, headerByte.Length, packetByte.Length);

				// 비동기 전송 시작.
				bool pending = Socket.SendAsync(SendEventArgs);
				if (pending == false)
				{
					ProcessSend(SendEventArgs);
				}
			}
		}

		private void ProcessSend(SocketAsyncEventArgs sendEventArgs)
		{
			lock (sendQueue)
			{
				sendQueue.Dequeue();

				if (sendQueue.Count > 0)
				{
					StartSend();
				}
			}
		}

		// 해당 세션이 메시지를 수신하였을 때를 핸들링하는 메소드.
		// 멤버 바이트 패커에게 받은 데이터 처리를 부탁한다.
		internal void OnReceive(byte[] buffer, int offset, int bytesTransferred)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			bytePacker.OnReceive(buffer, offset, bytesTransferred, OnMessagePacked);
		}

		// 바이트 패커가 받은 데이터 처리를 끝냈을 때 불러주는 콜백 메소드.
		// 해당 데이터로 패킷을 만들고, 이를 처리할 수 있도록 로직 핸들러에게 넣어준다.
		private void OnMessagePacked(int packetId, ArraySegment<byte> buffer)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			var req = MessagePackSerializer.Deserialize<LoginReq>(buffer);

			Console.WriteLine($"Req UserId({req.UserId}, Token({req.Token}))");

			var packet = new Packet(this, packetId, buffer); 

			packetLogicHandler.InsertPacket(packet);
		}

		// TODO :: 소켓 등의 처리를 해야하기도 할 것 같고.
		// 근데 서버라서 안해도 될 것 같고.
		internal void Close()
		{
			Console.WriteLine("Close");
			throw new NotImplementedException();
		}
	}
}
