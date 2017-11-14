using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkLibrary
{
	public class Packet
	{
		public int Position { get; internal set; }
		public Byte[] Buffer { get; internal set; }

		internal void CopyTo(Packet packet)
		{
			throw new NotImplementedException();
		}

		internal void RecordSize()
		{
			throw new NotImplementedException();
		}
	}
}
