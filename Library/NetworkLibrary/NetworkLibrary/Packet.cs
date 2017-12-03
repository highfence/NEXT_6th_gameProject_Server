using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace NetworkLibrary
{
	public class Packet
	{
		public Session Owner { get; private set; }

		public int PacketId { get; private set; }
		public byte[] Body { get; private set; }

		public Packet()
		{
			Owner = null;
			PacketId = 0;
			Body = null;
		}

		public Packet(Session owner, int packetId, ArraySegment<byte> buffer)
		{
			Owner = owner;
			PacketId = packetId;
			Body = new byte[buffer.Count];

			Array.Copy(buffer.Array, Body, buffer.Array.Length);
		}

		public Packet(Session owner, int packetId, byte[] serializedPacketBody)
		{
			Owner = owner;
			PacketId = packetId;
			Body = new byte[serializedPacketBody.Length];

			Array.Copy(serializedPacketBody, Body, serializedPacketBody.Length);
		}
	}
}
