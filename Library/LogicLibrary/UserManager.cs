using NetworkLibrary;

namespace LogicLibrary
{
	public class UserManager : ISessionManageable
    {
		bool ISessionManageable.IsSessionValid(Session clientSession)
		{
			return true;
		}

		int ISessionManageable.GetSessionTotalCount()
		{
			return 0;
		}
    }
}
