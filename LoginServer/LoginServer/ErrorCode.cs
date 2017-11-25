using System;
using System.Collections.Generic;
using System.Text;

public enum ErrorCode
{
	None = 0,

	// 800 ~ 900 번대는 임시로 DBServer, LoginServer 에러 코드로 쓰고 있겠음.
	MongoDBFindError = 800,
	MongoDBInsertError = 801,

	InvalidId = 810,
	InvalidPw = 811,

	IdAlreadyExist = 820,

	RedisInvalidAddress = 850,
	RedisStartFailed = 851,

	TokenRegistError = 852,
	TokenDeleteError = 853,

	UnregistedId = 860,
	InvalidToken = 861,

	LoginRequestException = 900,
}
