using System;
using System.Collections.Generic;
using System.Net.Sockets;
using MessagePack;

namespace NetworkLibrary
{
    public class ClientSession
    {
		public Socket Socket { get; set; }

		public SocketAsyncEventArgs receiveEventArgs { get; private set; }
		public SocketAsyncEventArgs sendEventArgs { get; private set; }

		private BytePacker bytePacker;
		private PacketProcessor packetProcessor;

		Queue<Packet> sendQueue;

		public ClientSession(PacketProcessor processor)
		{
			sendQueue = new Queue<Packet>();
			bytePacker = new BytePacker();

			packetProcessor = processor;
		}

		public void SetEventArgs(SocketAsyncEventArgs receiveEventArgs, SocketAsyncEventArgs sendEventArgs)
		{
			this.receiveEventArgs = receiveEventArgs;
			this.sendEventArgs = sendEventArgs;
		}

		public void Send(Packet packet)
		{
			// TODO :: sendPacket이 제대로 생성이 되는지 확인.
			var sendPacket = new Packet();
			sendPacket = packet;

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
			// TODO :: Send 짜야함.
			lock (sendQueue)
			{
				//var sendPacket = sendQueue.Peek();

				//// 헤더에 패킷 사이즈를 기록한다.
				//sendPacket.RecordSize();

				//// 이번에 보낼 패킷 사이즈만큼 버퍼 크기를 설정하고.
				//sendEventArgs.SetBuffer(sendEventArgs.Offset, sendPacket.Position);

				//// 패킷 내용을 SocketAsyncEventArgs 버퍼에 복사한다.
				//Array.Copy(sendPacket.Buffer, 0, sendEventArgs.Buffer, sendEventArgs.Offset, sendPacket.Position);

				//// 비동기 전송 시작.
				//bool pending = Socket.SendAsync(sendEventArgs);
				//if (pending == false)
				//{
				//	ProcessSend(sendEventArgs);
				//}
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

		internal void OnReceive(byte[] buffer, int offset, int bytesTransferred)
		{
			bytePacker.OnReceive(buffer, offset, bytesTransferred, OnMessagePacked);
		}

		private void OnMessagePacked(int packetId, ArraySegment<byte> buffer)
		{
			packetProcessor.OnMessage(this, packetId, buffer);

			var req = MessagePackSerializer.Deserialize<LoginReq>(buffer);

			Console.WriteLine($"Req UserId({req.UserId}, Token({req.Token}))");
		}

		internal void Close()
		{
			Console.WriteLine("Close");
			throw new NotImplementedException();
		}
	}
}
