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

        int currentUserNum = 0;

        List<User> player = new List<User>();
        Stack<int> freeUserIndex;

        public UserManager(int poolCapacity)
        {
            

            for (int i = 0; i < poolCapacity; ++i)
            {
                player.Add(new User());

                freeUserIndex.Push(i);
                player[i].Init();
                player[i].indexInPool = i;
            }
        }

        public bool IsUserExist(ClientSession owner)
		{
            //풀에 뭐라도 있는지 확인한후 있다면 다음 없다면 거짓
            if(IsEmpty() == true)
            {
                return false;
            }
            //할당된 인덱스를 모두 확인해서 클라이언트를 찾는다

            var user = FindUser(owner);

            //있다면 참 없다면 거짓
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }

		}

        public bool IsFull()
        {
            if(currentUserNum == maxConnections)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsEmpty()
        {
            if (currentUserNum == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       public int AllocNewUserToSession(ClientSession newSession,string userID)
        {
            //풀이 비어있는지 확인한후
            if(IsFull() == true)
            {
                return (int)ErrorCode.UserPoolIsFull;
            }
            
            //이미 존재하는 유저인지 확인한후
            if(IsUserExist(newSession) == true)
            {
                return (int)ErrorCode.UserAlreadyExist;
            }

            //사용가능한 유저 객체의 인덱스를 가져와서
            var freeIndex = freeUserIndex.Pop();

            //새로운 유저객체를 세션에 연결하고
            player[freeIndex].AllocUser(newSession, userID);

            //TODO:areaManager에 등록해줘야 한다.

            return 0;
        }

        int IUserManager.FreeUser(ClientSession targetSession)
        {
            //풀에 뭐라도 있는지 확인한후
            if (IsEmpty() == true)
            {
                return (int)ErrorCode.UserPoolIsEmpty;
            }

            //사용중 인덱스모두를 확인해서 유저가 존재하는지 확인한후
            var user = FindUser(targetSession);
            var notUseIndex = user.indexInPool;
            //없다면 오류코드
            if(user == null)
            {
                return (int)ErrorCode.UserNotExist;
            }
            else            //있다면 객체를 초기화 한후
            {
                user.Init();
            }

            //그 인덱스를 사용가능 유저 인덱스에넣는다.

            freeUserIndex.Push(notUseIndex);

            return 0;
        }

        User FindUser(ClientSession session)
        {
            if(session == null)
            {
                return null;
            }

            //TODO:원래 리스트로 사용하는 유저를 관리하면서 찾을 까 했는데 일단 다검색하는걸로.
            for (int i = 0; i < player.Count; ++i)
            {

                if (player[i].session == session)
                {
                    return player[i];
                }
            }

            return null;
        }

        User this[int i]
        {
            get
            {
                if(player[i].session != null)
                {
                    return player[i];
                }
                else
                {
                    return null;
                }

            }
            set
            {
                if (player[i].session != null)
                {
                    player[i] = value;
                }
            }
        }
    }
}
