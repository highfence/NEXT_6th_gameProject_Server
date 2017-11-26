using System;
using System.Collections.Generic;
using System.Text;
using NetworkLibrary;

namespace LogicLibrary
{
    public class UserManager : IUserManager
    {

        // TODO :: 서버 config 클래스 구현.
        const int maxConnections = 10000;

        Player[] freePlayer;
        Stack<int> freePlayerIndex;

        public UserManager(int poolCapacity)
        {
            freePlayer = new Player[poolCapacity];

            for(int i = 0;i< poolCapacity;++i)
            {
                freePlayerIndex.Push(i);
                freePlayer[i].indexInPool = i;
            }
        }

        bool IUserManager.IsUserExist(ClientSession owner)
		{
			return true;
		}
    }
}
