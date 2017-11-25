using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
	// 패킷 로직을 담당할 인터페이스.
    public interface IPacketLogicHandler
    {
		void InsertPacket(Packet packet);
    }
}
