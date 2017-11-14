using System;
using NetworkLibrary;
using System.Collections.Generic;

namespace ServerHost
{
    class Program
    {
		static void Main(string[] args)
		{
			var service = new NetworkService();

			service.Initialize();
			service.Listen("0.0.0.0", 23452, 100);

			Console.WriteLine($"Server Initialized. Port(23452)");

			while (true)
			{
				System.Threading.Thread.Sleep(1000);
			}
		}
    }
}
