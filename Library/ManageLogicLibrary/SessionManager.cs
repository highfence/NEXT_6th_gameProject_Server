using System;
using System.Collections.Generic;
using System.Text;
using NetworkLibrary;

namespace ManageLogicLibrary
{
    /*
	public class SessionManager : ISessionManageable
	{
		List<Session> sessionList;

		ConnectServerManager serverManager;


		public SessionManager(ConnectServerManager serverManager)
		{
			this.serverManager = serverManager;

			sessionList = new List<Session>();
		}


		public void Add(Session session)
		{
			lock (sessionList)
			{
				sessionList.Add(session);
			}
		}


		public int Remove(Session removeSession)
		{
			lock (sessionList)
			{
				return sessionList.RemoveAll(session => session == removeSession);
			}
		}


		public int GetSessionTotalCount()
		{
			return sessionList.Count;
		}


		public bool IsSessionValid(Session findSession)
		{
			lock (sessionList)
			{
				return sessionList.Exists(session => session.Equals(findSession));
			}
		}


		public Session GetServerSession(Session findSession)
		{
			Session serverSession;

			lock (sessionList)
			{
				serverSession = sessionList.Find(session => session == findSession); 
			}

			Remove(findSession);	

			return serverSession;
		}
	}
    */
}
