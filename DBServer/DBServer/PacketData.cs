﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NextManComing_DBServer
{
	// 다른 서버와 통신하기 위해 정의된 패킷 구조체들.
	public static class HttpPacket
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
			public Int64 Token;
		}

		public struct TokenValidationRes
		{
			public int Result;
		}

		public struct RegistTokenReq
		{
			public string UserId;
			public Int64 Token;
		}

		public struct RegistTokenRes
		{
			public int Result;
		}

		public struct DeleteTokenReq
		{
			public string UserId;
			public Int64 Token;
		}

		public struct DeleteTokenRes
		{
			public int Result;
		}
	}
}
