using NetworkLibrary;

namespace LogicLibrary
{
	public class UserManager : ISessionManageable
    {
		bool ISessionManageable.IsUserExist(Session clientSession)
		{
			return true;
		}
    }
}
