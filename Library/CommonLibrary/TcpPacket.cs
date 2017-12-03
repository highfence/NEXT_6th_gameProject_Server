using MessagePack;
using System;
using System.Collections.Generic;

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

	#region MANAGE SERVER PACKETS

	[MessagePackObject]
	public class ServerRegistRes
	{
		[Key(0)]
		public string Address;
		[Key(1)]
		public int Port;
	}

	[MessagePackObject]
	public class ServerRegistReq
	{
		[Key(0)]
		public int Result;
	}

	[MessagePackObject]
	public class ServerListReq
	{
		[Key(0)]
		public string Id;
		[Key(1)]
		public Int64 Token;
	}

	[MessagePackObject]
	public class ServerListRes
	{
		[Key(0)]
		public int Result;
		[Key(1)]
		public int ServerCount;
		[Key(2)]
		public List<string> ServerList;
		[Key(3)]
		public List<int> ServerCountList;
	}

	#endregion

	#region GAME SERVER PACKETS


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
		// Manage Server 관련 패킷은 100번대 사용.

		ServerRegistRes = 100,
		ServerRegistReq = 101,

		ServerListReq = 110,
		ServerListRes = 111,

		// 게임 서버 관련 패킷은 200번대 사용.

		ServerConnectReq = 201,
		ServerConnectRes = 202,
	}

	#endregion
}
