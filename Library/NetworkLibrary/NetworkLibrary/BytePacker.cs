using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace NetworkLibrary
{
	public delegate void CompletedMessageCallback(int packetId, ArraySegment<byte> buffer);

    internal class BytePacker
    {
		private int    messageSize;
		private byte[] messageBuffer = new byte[1024];

		// 현재 진행중인 버퍼의 인덱스를 가리키는 변수.
		// 패킷 하나를 완성한 뒤에는 0으로 초기화 시켜주어야 한다.
		private int currentPosition;

		public BytePacker()
		{
			messageSize = 0;
			currentPosition = 0;
			positionToRead = 0;
			remainBytes = 0;
		}

		// 데이터가 도착했을 때 이를 읽어주는 메소드.
		internal void OnReceive(byte[] buffer, int offset, int bytesTransferred, CompletedMessageCallback callback)
		{
			Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} Function Entry");

			// 이번 receive로 읽어오게 될 바이트 수.
			remainBytes = bytesTransferred;

			// 원본 버퍼의 포지션 값.
			// 패킷의 여러 개 뭉쳐서 올 경우 원본 버퍼의 포지션을 계속 앞으로 밀어주기 위함.
			int srcPosition = offset;

			while (remainBytes > 0)
			{
				var isCompleted = false;
				var header = new PacketHeader();

				if (currentPosition < Defines.HeaderSize)
				{
					positionToRead = Defines.HeaderSize;

					isCompleted = ReadUntil(buffer, ref srcPosition, offset, bytesTransferred);

					if (isCompleted == false)
					{
						// 아직 다 못읽었으므로 다음 receive를 기다린다.
						return;
					}

					// 헤더 하나를 온전히 읽어왔으므로 메시지의 사이를 구한다.
					header = GetPacketHeader();

					messageSize = header.BodySize;

					// 다음 목표 지점 (헤더 + 메시지 사이즈)
					positionToRead = messageSize + Defines.HeaderSize;
				}

				isCompleted = ReadUntil(buffer, ref srcPosition, offset, bytesTransferred);

				if (isCompleted)
				{
					// 패킷 하나를 완성시킨다.
					byte[] cloneBuffer = new byte[positionToRead - Defines.HeaderSize];
					Array.Copy(messageBuffer, Defines.HeaderSize, cloneBuffer, 0, positionToRead - Defines.HeaderSize);

					ClearBuffer();

					callback(header.PacketId, new ArraySegment<byte>(cloneBuffer, 0, cloneBuffer.Length));
				}
			}
		}


		// 버퍼의 첫부분을 떼다가 헤더를 만들어 준다.
		private PacketHeader GetPacketHeader()
		{
			try
			{
				byte[] headerBytes = new byte[Defines.HeaderSize];

				Array.Copy(messageBuffer, headerBytes, Defines.HeaderSize);

				return MessagePackSerializer.Deserialize<PacketHeader>(headerBytes);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);

				return null;
			}
		}

		// 읽어와야 할 목표 위치.
		private int positionToRead;

		// 남은 사이즈.
		private int remainBytes;

		// 목표 지점으로 설정된 위치까지의 바이트를 원본 버퍼로 부터 복사해주는 메소드.
		private bool ReadUntil(byte[] buffer, ref int srcPosition, int offset, int transffered)
		{
			if (currentPosition >= offset + transffered)
			{
				// 들어온 데이터만큼 다 읽은 상태이므로 더 이상 읽을 데이터가 없다.
				return false;
			}

			// 읽어와야 할 바이트.
			// 데이터가 분리되어 올 경우 이전에 읽어 놓은 값을 빼줘서 부족한 만큼
			// 읽어올 수 있도록 계산해 준다.
			int copySize = positionToRead - currentPosition;

			// 남은 데이터가 복사해야 할 바이트보다 작다면 가능한 만큼만 복사한다.
			if (remainBytes < copySize)
			{
				copySize = remainBytes;
			}

			// 버퍼에 복사
			Array.Copy(buffer, srcPosition, messageBuffer, currentPosition, copySize);

			// 원본 버퍼 포지션 이동.
			srcPosition += copySize;

			// 타겟 버퍼 포지션 이동.
			currentPosition += copySize;

			// 남은 바이트 수 변경
			remainBytes -= copySize;

			// 목표 지점에 도달하지 못했다면 false
			if (currentPosition < positionToRead)
			{
				return false;
			}

			return true;
		}

		// 내부 버퍼 초기화 메소드.
		private void ClearBuffer()
		{
			Array.Clear(messageBuffer, 0, messageBuffer.Length);

			currentPosition = 0;
			messageSize     = 0;
		}
    }
}
