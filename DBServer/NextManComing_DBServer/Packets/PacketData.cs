using System;
using System.Collections.Generic;
using System.Text;

namespace NextManComing_DBServer
{
	// 로그인 서버와 통신하기 위해 정의된 패킷 구조체들.
	internal static class LoginServerPacket
	{
		public struct LoginReq
		{
			public string UserId;
			public string UserPw;
		}

		public struct LoginRes
		{
			public short Result;
			public Int64 Token;
		}
	}

	// 게임 서버와 통신하기 위해 정의된 패킷 구조체들.
	internal static class GameServerPacket
	{

	}
}
