using Microsoft.Owin.Hosting;
using System;
using System.Text;

namespace NextManComing_LoginServer
{
    class Program
    {
        static void Main(string[] args)
        {
			var config = LoginServerConfig.GetInstance();

			var addressBuilder = new StringBuilder(20);
			addressBuilder.AppendFormat("http://*:{0}/", config.LoginServerPort);
			//var baseAddress = addressBuilder.ToString();
			var baseAddress = "http://localhost:18000/";

			using (WebApp.Start<Startup>(url : baseAddress))
			{
				Console.WriteLine("LoginServer Initialized. BaseAddress : " + baseAddress);
				Console.WriteLine("LoginServer Listening... IPv4 Address is " + Util.GetLocalIpAddress());
				Console.ReadLine();
			}
        }
    }
}
