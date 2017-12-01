using NetworkLibrary;
using System.Collections.Generic;

namespace ManageLogicLibrary
{
	// 현재 접속중인 서버를 관리하는 클래스.
	public class ConnectServerManager : ISessionManageable
	{
		List<ServerSession> connectedServers;


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


		bool ISessionManageable.IsSessionValid(Session findSession)
		{
			lock (connectedServers)
			{
				return connectedServers.Exists(connectedServer => connectedServer.Equals(findSession));
			}
		}


		int ISessionManageable.GetSessionTotalCount()
		{
			return connectedServers.Count;
		}
	}
}
