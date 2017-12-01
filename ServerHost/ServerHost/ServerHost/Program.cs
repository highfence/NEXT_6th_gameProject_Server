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

			service.Initialize(logicProcessor, userManager);
			service.Listen("0.0.0.0", 23452, 100);

			Console.WriteLine($"Server Initialized. Port(23452)");

            while (true)
			{
				System.Threading.Thread.Sleep(1000);
			}
        }
    }
}
