using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoginServer 
{
	internal class LoginServerConfig
	{
		private static LoginServerConfig instance;

		protected LoginServerConfig()
		{
			using (StreamReader r = new StreamReader("../../../../Common/ServerData.json"))
			{
				var configString = r.ReadToEnd();

				var configJson = JObject.Parse(configString);

				LoginServerAddress = configJson["LoginServerAddress"].ToString();
				LoginServerPort = Convert.ToInt32(configJson["LoginServerPort"].ToString());

				DBServerAddress = configJson["DBServerAddress"].ToString();
				DBServerPort = Convert.ToInt32(configJson["DBServerPort"].ToString());

				ManageServerAddress = configJson["ManageServerAddress"].ToString();
				ManageServerPort = Convert.ToInt32(configJson["ManageServerPort"].ToString());
			}
		}

		public static LoginServerConfig GetInstance()
		{
			if (instance == null)
			{
				instance = new LoginServerConfig();
			}

			return instance;
		}

		public string LoginServerAddress { get; private set; }
		public int LoginServerPort { get; private set; }

		public string DBServerAddress { get; private set; }
		public int DBServerPort { get; private set; }

		public string ManageServerAddress { get; private set; }
		public int ManageServerPort { get; private set; }
	}
}