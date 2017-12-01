﻿using System;
using System.Collections.Generic;
using System.Text;

public enum ErrorCode
{
	None = 0,

	UnIdentifiedError = 10,

	// 700 번대는 ManageServer 에러 코드.

	// 800 번대는 DBServer, LoginServer 에러 코드.
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

	// 900 번대는 LoginServer 에러 코드.
	LoginRequestException = 900,
}
