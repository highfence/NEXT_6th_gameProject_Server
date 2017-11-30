using System;
using System.Collections.Generic;
using System.Text;

namespace LoginServer
{
	// 클라이언트와 통신하기 위해 정의된 패킷 구조체들.
	public static class ClientPacket
	{
		public struct LoginReq
		{
			public string UserId;
			public string UserPw;
		}

		public struct LoginRes
		{
			public int Result;
			public long Token;
		}

		public struct LogoutReq
		{
			public string UserId;
			public string UserPw;
			public long Token;
		}
	}

	// DB 서버와 통신하기 위해 정의된 패킷 구조체들.
	public static class DBServerPacket
	{
		public struct UserValidationReq
		{
			public string UserId;
			public string EncryptedPw;
		}

		public struct UserValidationRes
		{
			public int Result;
		}

		public struct UserJoinInReq
		{
			public string UserId;
			public string EncryptedPw;
		}

		public struct UserJoinInRes
		{
			public int Result;
		}

		public struct TokenValidationReq
		{
			public string UserId;
			public long Token;
		}

		public struct TokenValidationRes
		{
			public int Result;
		}

		public struct RegistTokenReq
		{
			public string UserId;
			public long Token;
		}

		public struct RegistTokenRes
		{
			public int Result;
		}

		public struct DeleteTokenReq
		{
			public string UserId;
			public long Token;
		}

		public struct DeleteTokenRes
		{
			public int Result;
		}
	}
}