using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NetworkLibrary
{
    public interface ILogicQueue
    {
        void Enqueue(Packet msg);

        Queue<Packet> TakeAll();
    }
}
