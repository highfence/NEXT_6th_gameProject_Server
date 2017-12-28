using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
    public interface IUserSessionManageable
    {
        
        bool IsSessionValid(Session session);

        int GetSessionTotalCount();

        bool IsUserExist(Session owner);

        int AllocNewUserToSession(Session newSession, string userID);

        int FreeUser(Session targetSession);

        bool IsFull();

        bool IsEmpty();
    }
}
