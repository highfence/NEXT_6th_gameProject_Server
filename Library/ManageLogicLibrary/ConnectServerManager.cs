using CommonLibrary.TcpPacket;
using NetworkLibrary;
using System.Collections.Generic;
using System;

namespace ManageLogicLibrary
{
	// 현재 접속중인 서버를 관리하는 클래스.
	public class ConnectServerManager : ISessionManageable
	{
		List<ServerSession> connectedServers;


		// ServerListRes 패킷에 대해서 필요한 정보들을 작성해주는 메서드.
		public void WriteServerList(ref ServerListRes res)
		{
			var connectedNumber = connectedServers.Count;

			res.ServerCount = connectedNumber;
			res.ServerList = new List<string>(connectedNumber);
			res.ServerCountList = new List<int>(connectedNumber);

			foreach (var session in connectedServers)
			{
				res.ServerList.Add(session.AddrEndPoint);
				res.ServerCountList.Add(session.Count);
			}
		}


		public ConnectServerManager()
		{
			connectedServers = new List<ServerSession>();
		}


		public void Add(Session session)
		{
			lock (connectedServers)
			{
				connectedServers.Add(new ServerSession(session));
			}
		}

		
		public int Remove(Session removeSession)
		{
			lock (connectedServers)
			{
				return connectedServers.RemoveAll(connectedServer => connectedServer.Equals(removeSession));
			}
		}


		int ISessionManageable.GetSessionTotalCount()
		{
			return connectedServers.Count;
		}


		bool ISessionManageable.IsSessionValid(Session findSession)
		{
			lock (connectedServers)
			{
				return connectedServers.Exists(connectedServer => connectedServer.Equals(findSession));
			}
		}


		internal ServerSession GetOwner(Session findSession)
		{
			lock(connectedServers)
			{
				return connectedServers.Find(connectedServer => connectedServer.Equals(findSession));
			}
		}
	}
}
