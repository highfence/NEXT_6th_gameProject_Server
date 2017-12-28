using System;
using System.Collections.Generic;
using System.Text;
using NetworkLibrary;
namespace LogicLibrary
{
    class User
    {
        public string userID { get; set; }

        public int indexInPool { get; set; }

        public ClientSession session { get; set; }

        public Vector3 position { get; set; }

        public int areaCode { get; set; }

        public int[] oldActiveArea = new int[9];
        public int[] activeArea = new int[9];
        public int[] inActiveArea = new int[5];

        public bool AllocUser(ClientSession newSession ,string id)
        {
            if(newSession == null)
            {
                return false;
            }

            session = newSession;
            userID = id;

            //TODO: position을 어떻게 설정해 줄것인가?
            return true;
        }

        public void Init()
        {
            userID = "";
            session = null;
            position.x = 0.0f;
            position.y = 0.0f;
            position.z = 0.0f;
            areaCode = 0;

            for(int i = 0; i<oldActiveArea.Length;++i)
            {
                oldActiveArea[i] = 0;
            }

            for(int i = 0;i<activeArea.Length;++i)
            {
                activeArea[i] = 0;
            }

            for(int i = 0;i<inActiveArea.Length;++i)
            {
                inActiveArea[i] = 0;
            }
        }
    }
}
