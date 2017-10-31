using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
    //TODO: 하트 비트 분리하기
    public class SessionServer : Session
    {
        public SessionServer(Int64 uniqueId, IPacketDispatcher dispatcher) : base(uniqueId, dispatcher)
        {

        }
    }
}
