using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using System.Text;

namespace NextManComing_DBServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = DBServerConfig.GetInstance();

			var addressBuilder = new StringBuilder(20);
			addressBuilder.AppendFormat("http://*:{0}/", config.DBServerPort);

			//var baseAddress = addressBuilder.ToString();
			var baseAddress = "http://localhost:20000/";

			using (WebApp.Start<Startup>(url: baseAddress))
			{
				var result = DBServerMain.Init();

				if (result != ErrorCode.None)
				{
					Console.WriteLine("DBServer Initialize Failed. ErrorCode : {0}", result);
					return;
				}

				Console.WriteLine("DBServer Initialized. BaseAddress : {0}", baseAddress);
				Console.ReadLine();
			}
		}
	}
}
