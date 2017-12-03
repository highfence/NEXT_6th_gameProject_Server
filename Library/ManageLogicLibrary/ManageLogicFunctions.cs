using CommonLibrary;
using CommonLibrary.HttpPacket;
using CommonLibrary.TcpPacket;
using MessagePack;
using NetworkLibrary;
using System.Threading.Tasks;

namespace ManageLogicLibrary
{
	public partial class ManageLogicProcessor : IPacketHandleable
	{
		private async Task OnServerListReqArrived(Packet receivedPacket)
		{
			var req = MessagePackSerializer.Deserialize<ServerListReq>(receivedPacket.Body);

			logger.Debug($"Packet Arrived. Session({receivedPacket.Owner.Socket.Handle})");

			var tokenValidationReq = new TokenValidationReq()
			{
				UserId = req.Id,
				Token = req.Token
			};

			var tokenValidationRes = await networkService.HttpPost<TokenValidationReq, TokenValidationRes>("http://localhost:20000/DB/TokenValidation", tokenValidationReq);

			logger.Debug($"DB Server Response to TokenValidation. Result({tokenValidationRes.Result}) Session({receivedPacket.Owner.Socket.Handle})");

			if (tokenValidationRes.Result != (int)ErrorCode.None)
			{
				logger.Debug($"HttpPost TokenValidationReq failed. ErrorCode({tokenValidationRes.Result})");
			}

			var res = new ServerListRes();

			var serverManager = (ConnectServerManager)this.serverManager;
			serverManager.WriteServerList(ref res);

			var byteMessage = MessagePackSerializer.Serialize(res);

			var sendPacket = new Packet(receivedPacket.Owner, (int)PacketId.ServerListRes, byteMessage);

			var postSession = receivedPacket.Owner;

			postSession.Send(sendPacket);
		}
	}
}
