﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CloudStructures;
using CommonLibrary;

namespace DBServer
{
	public static class RedisManager
	{
		private static RedisGroup redisGroupBasic;

		public static ErrorCode Init(string address)
		{
			try
			{
				var basicRedisConnectionString = address.Split(',').ToList();
				var redisSettings = new RedisSettings[basicRedisConnectionString.Count()];

				if (!basicRedisConnectionString.Any()) return ErrorCode.RedisInvalidAddress;

				for (var i = 0; i < basicRedisConnectionString.Count(); ++i)
				{
					redisSettings[i] = new RedisSettings(basicRedisConnectionString[i], db: 0);
				}

				redisGroupBasic = new RedisGroup(groupName: "Basic", settings: redisSettings);

				// 서버마다 초기에 연결 한번 해놓는다. (막상 필요할 때 연결하려면 느리니까)
				for (var i = 0; i < basicRedisConnectionString.Count(); ++i)
				{
					var key = i.ToString() + "_test";
					var redis = new RedisString<int>(redisGroupBasic, key);
					var result = redis.Set(11);

					if (result.Result == false)
					{
						return ErrorCode.RedisStartFailed;
					}
				}

				return ErrorCode.None;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return ErrorCode.RedisStartFailed;
			}
		}

		// float을 사용할 수 없다(Redis). 대신 double을 사용하자.
		public static async Task<bool> SetStringAsync<T>(string key, T dataObject)
		{
			try
			{
				var redis = new RedisString<T>(redisGroupBasic, key);
				await redis.Set(dataObject);
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public static async Task<bool> SetStringAsyncWhenNoExists<T>(string key, T dataObject)
		{
			try
			{
				var redis = new RedisString<T>(redisGroupBasic, key);
				var result = await redis.Set(dataObject, null, StackExchange.Redis.When.NotExists);
				return result;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public static async Task<Tuple<bool, T>> GetStringAsync<T>(string key)
		{
			try
			{
				var redis = new RedisString<T>(redisGroupBasic, key);
				var value = await redis.Get();

				if (value.HasValue)
				{
					return Tuple.Create(true, value.Value);
				}

				return Tuple.Create(false, default(T));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
		}

		public static async Task<bool> DeleteStringAsync<T>(string key)
		{
			try
			{
				var redis = new RedisString<T>(redisGroupBasic, key);
				var result = await redis.Delete();
				return result;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
		}
	}

	public static class AuthTokenManager
	{
		// 레디스 서버에 토큰을 등록하는 메소드.
		public static async Task RegistAuthToken(string userId, Int64 token)
		{
			await RedisManager.SetStringAsync<DBUserSession>(userId,
				new DBUserSession() { AuthToken = token, ClientVersion = 1, ClientDataVersion = 1 });
		}

		// 레디스 서버에 등록되어 있는 토큰과 일치하는지를 확인하는 메소드.
		public static async Task<ErrorCode> CheckAuthToken(string userId, Int64 token)
		{
			var sessionInfo = await RedisManager.GetStringAsync<DBUserSession>(userId);

			if (sessionInfo.Item1 == false)
			{
				return ErrorCode.UnregistedId;
			}
			else if (sessionInfo.Item2.AuthToken != token)
			{
				return ErrorCode.InvalidToken;
			}
			return ErrorCode.None;
		}

		// 레디스 서버에 아이디로 등록되어 있는 토큰을 지워주는 메소드.
		public static async Task DeleteAuthToken(string userId)
		{
			await RedisManager.DeleteStringAsync<DBUserSession>(userId);
		}
	}

	public class DBUserSession
	{
		public Int64 AuthToken;
		public short ClientVersion;
		public short ClientDataVersion;
	}
}