using System;
using System.Collections.Generic;
using System.Text;

namespace NextManComing_LoginServer
{
	// 클라이언트와 통신하기 위해 정의된 패킷 구조체들.
	internal static class ClientPacket
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

		public struct LogoutReq
		{
			public string UserId;
			public string UserPw;
			public Int64 Token;
		}
	}

	// DB 서버와 통신하기 위해 정의된 패킷 구조체들.
	internal static class DBServerPacket
	{
		public struct UserValidationReq
		{
			public string UserId;
			public string EncryptedPw;
		}

		public struct UserValidationRes
		{
			public short Result;
			public Int64 Token;
		}

	}

}
