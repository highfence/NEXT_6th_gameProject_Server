using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CommonLibrary.HttpPacket;
using CommonLibrary;
namespace LoginServer
{
    class CreateUserController: ApiController
    {
        [Route("Login/CreateUser")]
        [HttpPost]
        public async Task<CreateUserRes>CreateUserRequest(CreateUserReq reqPacket)
        {
            var resPacket = new CreateUserRes();

            //패킷으로부터 아이디와 비번을 알아낸다.
            //비번을 암호화 한후
            var encryptedPassword = Encrypter.EncryptString(reqPacket.UserPw);

            Console.WriteLine($"Create User : Id({reqPacket.UserId}), Pw({encryptedPassword})");


            //아이디와 비번을 데이터 베이스에 저장 이미 존재하는지는 DB 서버에서 확인한다.
            try
            {
                //패킷 생성
                var userJoinReq = new UserJoinInReq()
                {
                    UserId = reqPacket.UserId,
                    EncryptedPw = encryptedPassword
                };

                var config = LoginServerConfig.GetInstance();

                var userJoinRes = await HttpSender.RequestHttp<UserJoinInReq, UserJoinInRes>
                    (config.DBServerAddress, config.DBServerPort, "DB/AddUser", userJoinReq);

                //유저 생성이 완료되지 않았다면 에러
                if(userJoinRes.Result != (int)ErrorCode.None)
                {
                    Console.WriteLine($"User Create Fail : Error({userJoinRes.Result}), Id({reqPacket.UserId}), Pw({encryptedPassword})");
                    resPacket.Result = userJoinRes.Result;
                    return resPacket;
                }

                //완료되면 완료 패킷

                Console.WriteLine($"User Create Success : Id({reqPacket.UserId}), Pw({encryptedPassword})");
                resPacket.Result = (int)ErrorCode.None;

                return resPacket;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error Occur at Create User Request. Message : {e.Message}");
                resPacket.Result = (int)ErrorCode.CreateUserRequestException;

                return resPacket;
            }
        }
    }
}
