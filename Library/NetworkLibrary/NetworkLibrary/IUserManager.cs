using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
	// IUserManager 인터페이스
	public interface IUserManager
	{
		// 해당 세션에 연결된 유저가 유효한지 확인해주는 메소드.
		bool IsUserExist(ClientSession owner);
	}
}
