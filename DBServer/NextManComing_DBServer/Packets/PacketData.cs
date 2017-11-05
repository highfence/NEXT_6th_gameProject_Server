using System;
using System.Collections.Generic;
using System.Text;

namespace NextManComing_DBServer
{
	// 로그인 서버와 통신하기 위해 정의된 패킷 구조체들.
	internal static class LoginServerPacket
	{
		public struct UserValidationReq 
		{
			public string UserId;
			public string EncryptedPw;
		}

		public struct UserValidationRes
		{
			public short Result;
		}
	}

	// 게임 서버와 통신하기 위해 정의된 패킷 구조체들.
	internal static class GameServerPacket
	{

	}
}
