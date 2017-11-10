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

    internal static class MongoDBManager
    {
		// 해당하는 유저의 정보가 MongoDB에 있는지 확인해주는 메소드.
		public static async Task<ErrorCode> IsUserExist(string userId, string encryptedPw)
		{
			var collection = GetCollection<DBUser>(userDBName, loginCollectionName);
			DBUser findUser;

			try
			{
				findUser = await collection.Find(x => x.Id == userId).FirstOrDefaultAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return ErrorCode.MongoDBFindError;
			}

			if (findUser != null)
			{
				return ErrorCode.None;
			}
			else
			{
				return ErrorCode.InvalidId;
			}
		}

		// MongoDB에 지정한 유저의 정보를 등록하는 메소드.
		public static async Task<ErrorCode> JoinUser(string userId, string encryptedPw)
		{
			// 일단 해당하는 정보의 유저가 이미 있는지를 확인.
			var checkValidation = await IsUserExist(userId, encryptedPw);
			// 이미 존재한다면 에러코드 반환.
			if (checkValidation == ErrorCode.None)
			{
				return ErrorCode.IdAlreadyExist;
			}

			// 존재하지 않는다면 가입 절차 진행.
			var newUser = new DBUser
			{
				Id = userId,
				Pw = encryptedPw,
				UId = DateTime.Now.Ticks,
			};

			var collection = GetCollection<DBUser>(userDBName, loginCollectionName);
			try
			{
				await collection.InsertOneAsync(newUser);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return ErrorCode.MongoDBInsertError;
			}

			return ErrorCode.None;
		}

		private const string connectionString = "mongodb://localhost:27017/?maxPoolSize=200";

		private const string userDBName = "UserDB";

		private const string loginCollectionName = "UserLoginInfo";

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
