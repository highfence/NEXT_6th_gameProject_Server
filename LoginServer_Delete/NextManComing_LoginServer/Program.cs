using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextManComing_LoginServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = LoginServerConfig.GetInstance();

			var addressBuilder = new StringBuilder(20);
			addressBuilder.AppendFormat("http://*:{0}/", config.LoginServerPort);

			var baseAddress = "http://localhost:18000/";

			using (WebApp.Start<Startup>(url : baseAddress))
			{
				Console.WriteLine($"Login server initialized. Base address : {baseAddress}");
				Console.ReadLine();
			}
		}
	}
}
