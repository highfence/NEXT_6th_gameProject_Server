using NetworkLibrary;

namespace LogicLibrary
{
	public class UserManager : ISessionManageable
    {
		bool ISessionManageable.IsUserExist(ISession owner)
		{
			return true;
		}
    }
}
