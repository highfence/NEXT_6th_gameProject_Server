using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace NetworkLibrary
{
    public class Packet
    {
		public ClientSession Owner { get; private set; }
		public int PacketId { get; private set; }
		public byte[] Body { get; private set; }
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
