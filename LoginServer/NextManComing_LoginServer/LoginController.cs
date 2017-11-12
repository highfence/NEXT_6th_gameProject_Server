using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace NextManComing_LoginServer
{
	public class LoginController : ApiController
	{
		[Route("Login/Login")]
		[HttpPost]
		public async Task<ClientPacket.LoginRes> LoginRequest(ClientPacket.LoginReq reqPacket)
		{
			var resPacket = new ClientPacket.LoginRes();

			// 유저 패스워드 암호화.
			var encryptedPassword = Encrypter.EncryptString(reqPacket.UserPw);

			Console.WriteLine($"Login request : Id({reqPacket.UserId}), Pw({encryptedPassword})");

			try
			{
				// DB에 유저가 가입되어 있는지를 조사한다.
				var userValidationReq = new DBServerPacket.UserValidationReq()
				{
					UserId = reqPacket.UserId,
					EncryptedPw = encryptedPassword
				};

				var config = LoginServerConfig.GetInstance();

				var userValidationRes = await HttpSender.RequestHttp<DBServerPacket.UserValidationReq, DBServerPacket.UserValidationRes>(
						config.DBServerAddress, config.DBServerPort, "DB/UserValidation", userValidationReq);

				// 가입되어 있지 않다면 에러 반환.
				if (userValidationRes.Result != (int)ErrorCode.None)
				{
					Console.WriteLine($"Invalid login request : Error({userValidationRes.Result}), Id({reqPacket.UserId}), Pw({encryptedPassword})");
					resPacket.Result = userValidationRes.Result;
					resPacket.Token = -1;
					return resPacket;
				}

				// 가입되어있다면 토큰을 생성한다.
				resPacket.Token = TokenGenerator.GetInstance().CreateToken();

				// DB Server에 토큰을 등록한다.
				var registTokenReq = new DBServerPacket.RegistTokenReq()
				{
					UserId = reqPacket.UserId,
					Token = resPacket.Token
				};

				var registTokenRes = await HttpSender.RequestHttp<DBServerPacket.RegistTokenReq, DBServerPacket.RegistTokenRes>(
						config.DBServerAddress, config.DBServerPort, "DB/RegistToken", registTokenReq);

				// 토큰 등록이 실패했다면 에러 반환.
				if (registTokenRes.Result != (int)ErrorCode.None)
				{
					Console.WriteLine($"Token regist failed : Error({registTokenRes.Result}), Id({reqPacket.UserId}), Pw({encryptedPassword})");
					resPacket.Result = registTokenRes.Result;
					resPacket.Token = -1;
					return resPacket;
				}

				// 모든 절차가 완료되었다면 정상값 반환.
				Console.WriteLine($"Login request completed : Id({reqPacket.UserId}), Pw({encryptedPassword})");
				resPacket.Result = (int)ErrorCode.None;
				return resPacket;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error invoked in login request. Message : {e.Message}");
				resPacket.Result = (int)ErrorCode.LoginRequestException;
				return resPacket;
			}
		}

	}
}