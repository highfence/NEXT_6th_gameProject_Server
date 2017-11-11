using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NextManComing_LoginServer
{
    public class Program
    {
		static string baseAddress = "http://localhost:18000/";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

		public static IWebHost BuildWebHost(string[] args) =>

			WebHost.CreateDefaultBuilder(args)
				.UseKestrel()
				.UseStartup<Startup>()
				.UseUrls(baseAddress)
                .Build();
    }
}
