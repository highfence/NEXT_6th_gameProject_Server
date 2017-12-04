using LogicLibrary;
using NetworkLibrary;
using NLog;
using System;

namespace ServerHost
{
	class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
		{
			var service		   = new NetworkService();
			var userManager	   = new UserManager();
			var logicProcessor = new LogicProcessor(service, userManager);
			logicProcessor.StartLogic();

			var listenAddress = "0.0.0.0";
			var listenPort = 23452;
			var backlog = 100;

			service.Initialize(logicProcessor, userManager);
			service.Listen(listenAddress, listenPort, backlog);

			Console.WriteLine($"Server Initialized. Address({listenAddress}), Port({listenPort}), Backlog({backlog})");

            while (true)
			{
				// TODO :: 여기에 현재 몇명이 접속했는지 등의 정보를 Console.Title로 기록.
				System.Threading.Thread.Sleep(1000);
			}
        }
    }
}
