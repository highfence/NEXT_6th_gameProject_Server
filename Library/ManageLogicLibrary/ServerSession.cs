﻿using System;
using NetworkLibrary;

namespace ManageLogicLibrary
{
	// 서버에서 필요한 몇가지 기능들을 위하여 정의한 기존의 세션 래퍼 클래스.
	internal class ServerSession : IEquatable<Session>
	{
		public Session Session { get; private set; }

		public string Address { get; private set; }


		public ServerSession(Session baseSession)
		{
			Session = baseSession;
		}


		// 기존 세션과의 비교를 위하여 정의한 비교 연산.
		public bool Equals(Session compareSession)
		{
			return Session == compareSession;
		}
	}
}
