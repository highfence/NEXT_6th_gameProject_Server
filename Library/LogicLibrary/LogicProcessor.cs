using NetworkLibrary;
using System.Threading;
using System.Collections.Generic;
using NLog;
using CommonLibrary.TcpPacket;

namespace LogicLibrary
{
    public partial class LogicProcessor : IPacketHandleable
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		NetworkService		 networkService;
		DoubleBufferingQueue messageQueue;
		AutoResetEvent		 messageEvent;
		ISessionManageable	 userManager;


		public LogicProcessor(NetworkService service, ISessionManageable userManager)
		{
			networkService   = service;
			this.userManager = userManager;

			messageQueue = new DoubleBufferingQueue();
			messageEvent = new AutoResetEvent(false);
		}


		public void StartLogic()
		{
			var logicThread = new Thread(LogicThread);
			logicThread.Start();
		}


		/// <summary>
		/// 네트워크 단에서 만들어진 패킷을 받아오는 메소드.
		/// 패킷을 받으면 메시지 이벤트를 활성화하여 로직 스레드가 돌아가도록 한다.
		/// <param name="packet"></param>
		/// </summary>
		void IPacketHandleable.InsertPacket(Packet packet)
		{
			logger.Debug($"InsertPacket. Id({packet.PacketId}), Session({packet.Owner.Socket.Handle})");

			messageQueue.Enqueue(packet);

			messageEvent.Set();
		}


		/// <summary>
		/// 실제로 로직이 돌아가게 될 스레드.
		/// 메시지가 오게 되는 상황까지 이벤트를 기다리다가 이벤트가 활성화되면 큐에 쌓인 모든 메시지를 처리해준다.
		/// </summary>
		private void LogicThread()
		{
			while (true)
			{
				messageEvent.WaitOne();

				DispatchAll(messageQueue.GetAllPackets());
			}
		}


		/// <summary>
		/// 더블 버퍼링 큐의 아웃 큐를 인자로 받아 쌓여있던 모든 패킷을 처리하는 메소드.
		/// 단순히 메시지를 꺼내서 주인이 있는 유효한 메시지 인지 확인하고, 해당 패킷에 묶여있던
		/// 메소드들을 호출해준다.
		/// </summary>
		/// <param name="messageQueue"></param>
		private void DispatchAll(Queue<Packet> messageQueue)
		{
			while (messageQueue.Count > 0)
			{
				logger.Debug($"LogicProcessor Dispatch Start. Queue Count : ({messageQueue.Count})");

				var message = messageQueue.Dequeue();

				if (userManager.IsSessionValid(message.Owner))
				{
					InvokePacketEvents(message);
				}
			}
		}


		/// <summary>
		/// 받았던 패킷 아이디에 해당하는 함수를 호출해준다. 
		/// </summary>
		/// <param name="receivedPacket"></param>
		private void InvokePacketEvents(Packet receivedPacket)
		{
			switch ((PacketId)receivedPacket.PacketId)
			{
				case PacketId.ServerConnectReq :
					OnLoginReqArrived(receivedPacket);
					break;
			}
		}
	}
}
