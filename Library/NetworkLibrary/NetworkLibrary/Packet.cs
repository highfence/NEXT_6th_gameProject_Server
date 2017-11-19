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

		public Packet()
		{
			Owner = null;
			PacketId = 0;
			Body = null;
		}

		public Packet(ClientSession owner, int packetId, ArraySegment<byte> buffer)
		{
			Owner = owner;
			PacketId = packetId;
			Body = new byte[buffer.Count];

			Array.Copy(buffer.Array, Body, buffer.Array.Length);
		}
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

	public enum PacketId
	{
		LoginReq = 101
	}
}
