using System;
using System.Collections.Generic;
using System.Text;

public static class HttpPacket
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

	public struct UserJoinInReq
	{
		public string UserId;
		public string EncryptedPw;
	}

	public struct UserJoinInRes
	{
		public short Result;
	}

	public struct TokenValidationReq
	{
		public string UserId;
		public Int64 Token;
	}

	public struct TokenValidationRes
	{
		public short Result;
	}

	public struct RegistTokenReq
	{
		public string UserId;
		public Int64 Token;
	}

	public struct RegistTokenRes
	{
		public short Result;
	}

	public struct DeleteTokenReq
	{
		public string UserId;
		public Int64 Token;
	}

	public struct DeleteTokenRes
	{
		public short Result;
	}
}

public static class TcpPacket
{

}
