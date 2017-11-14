using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using MessagePack;

namespace NetworkLibrary
{
    internal class Defines
    {
		public static readonly int HeaderSize = 3;
    }

	[MessagePackObject]
	public class PacketHeader
	{
		[Key(0)]
		public int PacketId;
		[Key(1)]
		public int BodySize;
	}

	[MessagePackObject]
	public class LoginReq
	{
		[Key(0)]
		public string UserId;
		[Key(1)]
		public Int64 Token;
	}
}
