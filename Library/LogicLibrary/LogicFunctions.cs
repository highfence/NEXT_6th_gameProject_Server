using NetworkLibrary;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace LogicLibrary
{
    public partial class LogicProcessor
    {
		private async Task OnLoginReqArrived(Packet receivedPacket)
		{
			var loginReq = MessagePackSerializer.Deserialize<LoginReq>(receivedPacket.Body);

			logger.Debug($"Function Entry. Session({receivedPacket.Owner.Socket.Handle}) LoginReq UserId({loginReq.UserId}), Token({loginReq.Token})");

			var tokenValidationReq = new HttpPacket.TokenValidationReq()
			{
				UserId = loginReq.UserId,
				Token = loginReq.Token
			};

			var tokenValidationRes = await networkService.HttpPost<HttpPacket.TokenValidationReq, HttpPacket.TokenValidationRes>("http://localhost:20000/DB/TokenValidation", tokenValidationReq);

			logger.Debug($"DB Server Response to TokenValidation. Result({tokenValidationRes.Result}) Session({receivedPacket.Owner.Socket.Handle})");

			if (tokenValidationRes.Result != (int)ErrorCode.None)
			{
				logger.Debug($"HttpPost TokenValidationReq failed. ErrorCode({tokenValidationRes.Result})");
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
