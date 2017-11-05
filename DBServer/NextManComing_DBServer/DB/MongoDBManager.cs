using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextManComing_DBServer
{
	internal class DBUser
	{
		public Int64 UId;
		public string Id;
		public string Pw;
	}

    internal class MongoDBManager
    {
		// 유저의 정보가 MongoDB에 적혀있는 것과 일치하는지 판단해주는 메소드.
		public static async Task<LoginServerPacket.UserValidationRes> GetUserValidation(string userId, string encryptedPw)
		{
			var userValidation = new LoginServerPacket.UserValidationRes()
			{
				Result = (short)ErrorCode.None
			};

			var collection = GetCollection<DBUser>(UserDBName, LoginCollectionName);
			DBUser findUser;

			try
			{
				findUser = await collection.Find(recordedUser => recordedUser.Id == userId).FirstOrDefaultAsync();
			}
			catch (Exception e)
			{
				// MongoDB에서 Find하다가 예외가 발생한 경우.
				Console.WriteLine(e.Message);
				userValidation.Result = (short)ErrorCode.MongoDBFindError;

				return userValidation;
			}

			if (string.IsNullOrEmpty(findUser.Id))
			{
				// 유저 정보가 없다면 에러 반환.
				userValidation.Result = (short)ErrorCode.InvalidId;
			}
			else if (findUser.Pw != encryptedPw)
			{
				// 비밀번호가 일치하지 않는다면 에러 반환.
				userValidation.Result = (short)ErrorCode.InvalidPw;
			}

			return userValidation;
		}

		// MongoDB에 유저의 정보를 등록하는 메소드.
		public static async Task<LoginServerPacket.UserJoinInRes> JoinUser(string userId, string encryptedPw)
		{
			var userJoinRes = new LoginServerPacket.UserJoinInRes()
			{
				Result = (short)ErrorCode.None
			};

			var collection = GetCollection<DBUser>(UserDBName, LoginCollectionName);
			DBUser findUser;

			// 요청된 아이디와 중복된 아이디가 있는지 확인한다.
			try
			{
				findUser = await collection.Find(recordedUser => recordedUser.Id == userId).FirstOrDefaultAsync();
			}
			catch (Exception e)
			{
				// MongoDB에서 Find하다가 예외가 발생한 경우.
				Console.WriteLine(e.Message);
				userJoinRes.Result = (short)ErrorCode.MongoDBFindError;

				return userJoinRes;
			}

			// 중복된 아이디가 있다면 유저 정보가 이미 있다고 적어놓고 반환한다.
			if (findUser != null)
			{
				userJoinRes.Result = (short)ErrorCode.IdAlreadyExist;
				return userJoinRes;
			}

			// 중복된 아이디가 없다면 새로 등록해준다.
			var newUser = new DBUser()
			{
				Id = userId,
				UId = DateTime.Now.Ticks,
				Pw = encryptedPw
			};

			try
			{
				await collection.InsertOneAsync(newUser);
			}
			catch (Exception e)
			{
				// MongoDB에 등록하는 과정에서 오류가 일어났을 경우.
				Console.WriteLine(e.Message);
				userJoinRes.Result = (short)ErrorCode.MongoDBInsertError;
			}

			return userJoinRes;
		}

		private const string connectionString = "mongodb://localhost:27017/?maxPoolSize=200";

		private const string UserDBName = "UserDB";

		private const string LoginCollectionName = "UserLoginInfo";

		private static IMongoDatabase GetMongoDatabase(string dbName)
		{
			return new MongoClient(connectionString).GetDatabase(dbName);
		}

		private static IMongoCollection<T> GetCollection<T>(string dbName, string collectionName)
		{
			return GetMongoDatabase(dbName).GetCollection<T>(collectionName);
		}
    }
}
