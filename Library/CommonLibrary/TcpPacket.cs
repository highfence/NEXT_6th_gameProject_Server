using MessagePack;
using System;

namespace CommonLibrary.TcpPacket
{
	[MessagePackObject]
	public class PacketHeader
	{
		[Key(0)]
		public int PacketId;
		[Key(1)]
		public int BodySize;

		public PacketHeader()
		{
			PacketId = 0;
			BodySize = 0;
		}

		public PacketHeader(int bodySize, int packetId)
		{
			BodySize = bodySize;
			PacketId = packetId;
		}
	}

	[MessagePackObject]
	public class ServerConnectReq
	{
		[Key(0)]
		public string UserId;
		[Key(1)]
		public Int64 Token;
	}

	[MessagePackObject]
	public class ServerConnectRes
	{
		[Key(0)]
		public int Result;
	}

	public enum PacketId
	{
		LoginReq = 101,
		LoginRes = 102
	}
}
