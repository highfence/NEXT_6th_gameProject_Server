using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NextManComing_LoginServer.Controllers
{
    public class LoginController : Controller
    {
		[Route("Login/Login")]
		[HttpPost]
		public async Task<ClientPacket.LoginRes> LoginRequest(ClientPacket.LoginReq reqPacket)
		{
			var resPacket = new ClientPacket.LoginRes();

			// 유저 패스워드 암호화.
			var encryptedPassword = Encrypter.EncryptString(reqPacket.UserPw);

			Console.WriteLine($"Login Request : Id({reqPacket.UserId}), Pw({encryptedPassword})");

			try
			{
				// DB에 유저가 가입되어 있는지를 조사한다.
				var userValidationReq = new DBServerPacket.UserValidationReq()
				{
					UserId = reqPacket.UserId,
					EncryptedPw = encryptedPassword
				};

				var userValidationRes = await HttpSender.RequestHttp<DBServerPacket.UserValidationReq, DBServerPacket.UserValidationRes>(
					);
			}
			catch (Exception e)
			{

			}
		}

    }
}