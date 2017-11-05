using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextManComing_LoginServer
{
    internal class DataContainer
    {
		private static DataContainer instance;

		protected DataContainer() { }

		public static DataContainer GetInstance()
		{
			if (instance == null)
			{
				instance = new DataContainer();
			}

			return instance;
		}
    }
}
