using NetworkLibrary;

namespace LogicLibrary
{
	public class UserManager : IUserManager
    {
		bool IUserManager.IsUserExist(ClientSession owner)
		{
			return true;
		}
    }
}
