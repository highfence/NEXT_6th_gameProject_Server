using System;
using System.Collections.Generic;
using System.Text;

namespace NextManComing_DBServer
{
	public static class DBServerMain
	{
		public static ErrorCode Init()
		{
			var error = InitDB();

			// 에러가 이상할 경우 출력.
			if (error != ErrorCode.None) Console.WriteLine($"Starting failed in InitDB : {error}");

			return error;
		}

		// 레디스 연결을 미리 해둔다.
		// 필요할 때 연결을 시작하면 너무 느리므로.
		private static ErrorCode InitDB()
		{
			const string redisList = "localhost:6379";
			var error = RedisManager.Init(redisList);

			return error;
		}
	}
}
