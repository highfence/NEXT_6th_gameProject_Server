using System;
using System.Collections.Generic;
using System.Threading;

namespace NetworkLibrary
{
    public partial class PacketProcessor
    {
		NetworkService		 networkService;
		DoubleBufferingQueue messageQueue;
		AutoResetEvent		 logicEvent;
		UserManager			 userManager;

		public PacketProcessor(NetworkService service, UserManager userManager)
		{
			networkService = service;
			this.userManager = userManager;

			messageQueue = new DoubleBufferingQueue();
			logicEvent = new AutoResetEvent(false);
		}

		public void StartLogic()
		{
			Thread logicThread = new Thread(LogicThread);
			logicThread.Start();
		}

		private void LogicThread()
		{
			while (true)
			{
				logicEvent.WaitOne();

				DispatchAll(messageQueue.GetAllPackets());
			}
		}

		private void DispatchAll(Queue<Packet> messageQueue)
		{
			while (messageQueue.Count > 0)
			{
				var message = messageQueue.Dequeue();

				if (userManager.IsSessionValid(message.Owner))
				{
					// 여기에 로직 짜면 됨.
					Console.WriteLine("Arrived!");
				}
			}
		}
	}
}
