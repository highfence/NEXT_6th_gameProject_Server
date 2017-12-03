using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
	// 패킷 처리를 담당할 인터페이스.
    public interface IPacketHandleable
    {
		// 패킷을 처리하도록 넘겨주는 메서드.
		void InsertPacket(Packet packet);
    }
}
