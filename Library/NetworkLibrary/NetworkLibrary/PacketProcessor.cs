using System;
using System.Collections.Generic;
using System.Threading;

namespace NetworkLibrary
{
	public partial class PacketProcessor
	{
		private NetworkService networkService;
		private DoubleBufferingQueue messageQueue;
		private AutoResetEvent logicEvent;
		private UserManager userManager;

		public PacketProcessor(NetworkService service, UserManager userManager)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			networkService = service;
			this.userManager = userManager;

			messageQueue = new DoubleBufferingQueue();
			logicEvent = new AutoResetEvent(false);
		}

		public void StartLogic()
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			Thread logicThread = new Thread(LogicThread);
			logicThread.Start();
		}

		private void LogicThread()
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			while (true)
			{
				logicEvent.WaitOne();

				DispatchAll(messageQueue.GetAllPackets());
			}
		}

		private void DispatchAll(Queue<Packet> messageQueue)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			while (messageQueue.Count > 0)
			{
				var message = messageQueue.Dequeue();

				if (userManager.IsSessionValid(message.Owner))
				{
					InvokePacketEvents(message);
				}
			}
		}

		// 받은 패킷에 이벤트를 걸어놓았던 함수들을 모두 실행시킨다.
		private void InvokePacketEvents(Packet receivedPacket)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			switch ((PacketId)receivedPacket.PacketId)
			{
				case PacketId.LoginReq:
					OnLoginReq.Invoke(receivedPacket);
					Console.WriteLine("Successfully arrived on LoginReq");
					break;
			}
		}

		internal void OnMessage(ClientSession clientSession, int packetId, ArraySegment<byte> buffer)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			var packet = new Packet();
			packet.SetInfo(clientSession, packetId, buffer);

			messageQueue.Enqueue(packet);

			logicEvent.Set();
		}
	}
}