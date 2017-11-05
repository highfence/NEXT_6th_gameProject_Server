using Microsoft.Owin.Hosting;
using System;
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

			using (WebApp.Start<Startup>(url: addressBuilder.ToString()))
			{
				var result = DBServerMain.Init();

				if (result != ErrorCode.None)
				{
					Console.WriteLine("DBServer Initialize Failed. ErrorCode : ", result);
					return;
				}

				Console.WriteLine("DBServer Initialized. BaseAddress : ", addressBuilder.ToString());
				Console.ReadLine();
			}
        }
    }
}
