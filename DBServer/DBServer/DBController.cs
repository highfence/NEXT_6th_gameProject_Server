using System;
using System.Threading.Tasks;
using System.Web.Http;
using CommonLibrary;
using CommonLibrary.HttpPacket;

namespace DBServer
{
	public class DBController : ApiController
	{
		// 해당 유저가 등록되어 있는지 확인하는 메소드.
		[Route("DB/UserValidation")]
		[HttpPost]
		public async Task<UserValidationRes> GetUserValidation(UserValidationReq req)
		{
			var res = new UserValidationRes();

			Console.WriteLine($"UserValidation Request. Id : {req.UserId}, Pw : {req.EncryptedPw}");

			var isUserExisted = await MongoDBManager.IsUserExist(req.UserId, req.EncryptedPw);

			res.Result = (int)isUserExisted;

			return res;
		}

		// 유저를 등록하는 메소드.
		[Route("DB/AddUser")]
		[HttpPost]
		public async Task<UserJoinInRes> AddUser(UserJoinInReq req)
		{
			var res = new UserJoinInRes();

			Console.WriteLine($"AddUser Request. Id : {req.UserId}, Pw : {req.EncryptedPw}");

			var result = await MongoDBManager.JoinUser(req.UserId, req.EncryptedPw);

			res.Result = (int)result;

			return res;
		}

		// 유저의 토큰 값이 일치하는지 확인해주는 메소드.
		[Route("DB/TokenValidation")]
		[HttpPost]
		public async Task<TokenValidationRes> GetTokenValidation(TokenValidationReq req)
		{
			var res = new TokenValidationRes();

			Console.WriteLine($"Token Validation Request. Id : {req.UserId}, Token : {req.Token}");

			var result = await AuthTokenManager.CheckAuthToken(req.UserId, req.Token);

			res.Result = (int)result;

			return res;
		}

		// 유저의 토큰 값을 기록하는 메소드.
		[Route("DB/RegistToken")]
		[HttpPost]
		public async Task<RegistTokenRes> GetTokenAuth(RegistTokenReq req)
		{
			var res = new RegistTokenRes();

			Console.WriteLine($"Token Auth Request. Id : {req.UserId}, Token : {req.Token}");

			try
			{
				await AuthTokenManager.RegistAuthToken(req.UserId, req.Token);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				res.Result = (int)ErrorCode.TokenRegistError;
				return res;
			}

			res.Result = (int)ErrorCode.None;
			return res;
		}

		// 유저의 토큰 값을 삭제하는 메소드.
		[Route("DB/DeleteToken")]
		[HttpPost]
		public async Task<DeleteTokenRes> DeleteToken(DeleteTokenReq req)
		{
			var res = new DeleteTokenRes();

			Console.WriteLine($"Token Delete Request. Id : {req.UserId}, Token : {req.Token}");

			// 유효한 값인지 우선 검사.
			var validation = await AuthTokenManager.CheckAuthToken(req.UserId, req.Token);
			if (validation != (int)ErrorCode.None)
			{
				res.Result = (int)validation;
				return res;
			}

			// 유효한 요청이라면 삭제.
			try
			{
				await AuthTokenManager.DeleteAuthToken(req.UserId);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				res.Result = (int)ErrorCode.TokenDeleteError;
				return res;
			}

			res.Result = (int)ErrorCode.None;
			return res;
		}

	}
}