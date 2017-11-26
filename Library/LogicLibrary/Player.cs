using System;
using System.Collections.Generic;
using System.Text;
using NetworkLibrary;

namespace LogicLibrary
{
    public class Player
    {
        public string playerID { get; set; }

        public int indexInPool { get; set; }

        public ClientSession session { get; set; }

        public Vector3 position { get; set; }

        public int areaCode { get; set; }

        public int[] oldActiveArea = new int[9];
        public int[] activeArea = new int[9];
        public int[] inActiveArea = new int[5];


    }
    
}
