using System;

// LoginServer, DBServer와의 통신을 위해 필요한 관련 Http Packet들 정리.
namespace CommonLibrary.HttpPacket
{
	#region LOGIN SERVER PACKETS

	// 클라이언트의 로그인 요청 패킷.
	public struct LoginReq
	{
		public string UserId;
		public string UserPw;
	}

	// 로그인 서버의 로그인 답변 패킷.
	public struct LoginRes
	{
		public int Result;
		public long Token;
		public string ManageServerAddr;
		public int ManageServerPort;
	}

    public struct CreateUserReq
    {
        public string UserId;
        public string UserPw;
    }

    public struct CreateUserRes
    {
        public int Result;
    }

	// 클라이언트의 로그 아웃 요청 패킷.
	public struct LogoutReq
	{
		public string UserId;
		public string UserPw;
		public long Token;
	}

	#endregion

	#region DB SERVER PACKETS

	// MongoDB에 유저의 유효성을 조회해보는 요청.
	public struct UserValidationReq
	{
		public string UserId;
		public string EncryptedPw;
	}

	// 유저 유효성 조회 결과 패킷.
	public struct UserValidationRes
	{
		public int Result;
	}

	// 유저 가입 신청 패킷.
	public struct UserJoinInReq
	{
		public string UserId;
		public string EncryptedPw;
	}

	// 유저 가입 신청 패킷.
	public struct UserJoinInRes
	{
		public int Result;
	}

	// 토큰 유효성 검증 요청 패킷.
	public struct TokenValidationReq
	{
		public string UserId;
		public Int64 Token;
	}

	// 토큰 유효성 검증 결과 패킷.
	public struct TokenValidationRes
	{
		public int Result;
	}

	// 토큰 등록 요청 패킷.
	public struct RegistTokenReq
	{
		public string UserId;
		public Int64 Token;
	}

	// 토큰 등록 결과 패킷.
	public struct RegistTokenRes
	{
		public int Result;
	}

	// 토큰 삭제 요청 패킷.
	public struct DeleteTokenReq
	{
		public string UserId;
		public Int64 Token;
	}

	// 토큰 삭제 결과 패킷.
	public struct DeleteTokenRes
	{
		public int Result;
	}

	#endregion 
}
