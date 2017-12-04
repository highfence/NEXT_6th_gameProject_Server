using System;
using NetworkLibrary;
using ManageLogicLibrary;
using NLog;

namespace ManageServer
{
    class Program
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
			var service = new NetworkService();
			var serverManager = new ConnectServerManager();
			var sessionManager = new SessionManager(serverManager);
			var logicProcessor = new ManageLogicProcessor(service, serverManager);
			logicProcessor.SessionManagerSetting(sessionManager as ISessionManageable);
			logicProcessor.StartLogic();

			service.Initialize(logicProcessor, sessionManager);

			var listenAddress = "0.0.0.0";
			var listenPort = 19000;
			var backlog = 100;

			service.Listen(listenAddress, listenPort, backlog);

			logger.Debug($"Manage Server Initialized. Address({listenAddress}), Port({listenPort}), Backlog({backlog})");

			while (true)
			{
				System.Threading.Thread.Sleep(1000);
			}
        }
    }
}
