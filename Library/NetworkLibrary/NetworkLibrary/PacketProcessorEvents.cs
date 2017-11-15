using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
    public partial class PacketProcessor
    {
		public event Action<Packet> OnLoginReq = delegate { };
    }
}
