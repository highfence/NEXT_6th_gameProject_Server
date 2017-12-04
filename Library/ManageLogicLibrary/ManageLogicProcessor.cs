using NetworkLibrary;
using NLog;
using System.Threading;
using System;
using System.Collections.Generic;
using CommonLibrary.TcpPacket;

namespace ManageLogicLibrary
{
	public partial class ManageLogicProcessor : IPacketHandleable
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		NetworkService networkService;
		DoubleBufferingQueue messageQueue;
		AutoResetEvent messageEvent;
		ISessionManageable serverManager;
		ISessionManageable sessionManager;

		
		public ManageLogicProcessor(NetworkService service, ISessionManageable serverManager)
		{
			networkService = service;
			this.serverManager = serverManager;

			messageQueue = new DoubleBufferingQueue();
			messageEvent = new AutoResetEvent(false);
		}


		public void SessionManagerSetting(ISessionManageable sessionManager)
		{
			this.sessionManager = sessionManager;

			var manager = sessionManager as SessionManager;

			networkService.OnSessionCreated += manager.Add;
		}


		public void StartLogic()
		{
			var logicThread = new Thread(LogicThread);
			logicThread.Start();
		}


		// 네트워크 단에서 만들어진 패킷을 받아오는 메서드.
		// 패킷을 받으면 메시지 이벤트를 활성화하여 로직 스레드가 돌아가도록 한다.
		public void InsertPacket(Packet packet)
		{
			logger.Debug($"Insert packet. Id({packet.PacketId}), Session({packet.Owner.Socket.Handle})");

			messageQueue.Enqueue(packet);

			messageEvent.Set();
		}


		// 실제로 로직이 돌아가게 될 스레드.
		// 메시지가 오게 되는 상황까지 이벤트를 기다리가가 이벤트가 활성화되면 큐에 쌓인 모든 메시지를 처리해준다.
		private void LogicThread()
		{
			while (true)
			{
				messageEvent.WaitOne();

				DispatchAll(messageQueue.GetAllPackets());
			}
		}


		// 더블 버퍼링 큐의 아웃 큐를 인자로 받아 쌓여있던 모든 패킷을 처리하는 메서드.
		// 단순히 메시지를 꺼내서 주인이 있는 유효한 메시지 인지 확인하고, 해당 패킷에 묶여있던
		// 메서드들을 호출해준다.
		private void DispatchAll(Queue<Packet> messageQueue)
		{
			while (messageQueue.Count > 0)
			{
				logger.Debug($"LogicProcessor Dispatch Start. Queue Count : ({messageQueue.Count})");

				var message = messageQueue.Dequeue();

				if (serverManager.IsSessionValid(message.Owner))
				{
					InvokePacketEvents(message);
				}
			}
		}


		private void InvokePacketEvents(Packet message)
		{
			switch ((PacketId)message.PacketId)
			{
				case PacketId.ServerListReq:
					OnServerListReqArrived(message);
					break;

				case PacketId.ServerRegistReq:
					OnServerRegistReqArrived(message);
					break;
			}
		}

	}
}
