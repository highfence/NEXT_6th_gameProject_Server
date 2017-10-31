using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
    public interface IPeer
    {
        void OnRemoved();


        void Send(Packet pkt);


        void DisConnect();
    }
}
