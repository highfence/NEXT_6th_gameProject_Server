using NetworkLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicLibrary
{
    public partial class LogicProcessor
    {
		private void OnLoginReqArrived(Packet receivedPacket)
		{
			Console.WriteLine("Login Req Arrived!");
		}
    }
}
