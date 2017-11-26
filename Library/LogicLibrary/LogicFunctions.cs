using NetworkLibrary;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogicLibrary
{
    public partial class LogicProcessor
    {
		private async Task OnLoginReqArrived(Packet receivedPacket)
		{
			var loginReq = MessagePackSerializer.Deserialize<LoginReq>(receivedPacket.Body);

			var tokenValidationReq = new HttpPacket.TokenValidationReq()
			{
				UserId = loginReq.UserId,
				Token = loginReq.Token
			};

			var reqDataByte = MessagePackSerializer.Serialize(tokenValidationReq);

			var tokenValidationRes = await networkService.HttpPost<HttpPacket.TokenValidationRes>("http://localhost:20000/DB/UserValidation", reqDataByte);

			if (tokenValidationRes.Result != (int)ErrorCode.None)
			{
				Console.WriteLine($"HttpPost TokenValidationReq failed. ErrorCode({tokenValidationRes.Result})");
			}

			var loginRes = new LoginRes()
			{
				Result = tokenValidationRes.Result
			};

			var sendPacket = new Packet(receivedPacket.Owner, (int)PacketId.LoginRes, MessagePackSerializer.Serialize(loginRes));

			var postSession = receivedPacket.Owner;

			postSession.Send(sendPacket);
		}
    }
}
