using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkLibrary
{
	// 네트워크 라이브러리 내부에서 새로운 클라이언트의 접속을 감지하는 클래스.
    internal class ClientListener
    {
		SocketAsyncEventArgs acceptArgs;

		Socket listenSocket;

		AutoResetEvent flowControllEvent;

		// 새로운 클라이언트가 접속했을 때 호출되는 델리게이트.
		public delegate void NewClientHandler(Socket clientSocket, object token);
		public NewClientHandler OnClientConnected;

		public ClientListener()
		{
			OnClientConnected = null;
		}

		public void StartListen(string host, int port, int backlog)
		{
			// 소켓 생성.
			listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			IPAddress address;

			if (host == "0.0.0.0")
			{
				address = IPAddress.Any;
			}
			else
			{
				address = IPAddress.Parse(host);
			}

			var endPoint = new IPEndPoint(address, port);

			try
			{
				listenSocket.Bind(endPoint);
				listenSocket.Listen(backlog);

				acceptArgs = new SocketAsyncEventArgs();
				acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

				// 클라이언트가 들어오기를 기다린다.
				var listenThread = new Thread(ListenThread);
				listenThread.Start();
			}
			catch (Exception e)
			{
				Console.WriteLine($"ClientListener listen failed. Message : {e.Message}");
			}
		}

		private void ListenThread()
		{
			// accpet 처리 제어를 위해 이벤트 객체 생성.
			// 초기 상태는 false 상태.
			flowControllEvent = new AutoResetEvent(false);

			while (true)
			{
				// SocketAsyncEventArgs를 재사용하기 위해서 null로 만들어 준다.
				acceptArgs.AcceptSocket = null;

				var pending = true;
				try
				{
					// 비동기로 accept를 호출.
					// 동기적으로 완료될 수도 있기 때문에 pending 확인 필요.
					pending = listenSocket.AcceptAsync(acceptArgs);
				}
				catch (Exception e)
				{
					Console.WriteLine($"ListenThread AcceptAsync failed. Message : {e.Message}");
					continue;
				}

				if (pending == false)
				{
					OnAcceptCompleted(null, acceptArgs);
				}

				// 클라이언트 접속 처리가 완료되면 이벤트 객체의 신호를 전달받아 다시 루프를 실행한다.
				flowControllEvent.WaitOne();
			}
		}

		private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				// 새로 생긴 소켓을 보관해 놓은 뒤, 다음 연결을 받아들임.
				var clientSocket = e.AcceptSocket;
				flowControllEvent.Set();

				// Listener의 역할에만 충실하기 위해 별도의 처리는 다른 필요한 클래스에서 콜백 메소드로 수행.
				OnClientConnected?.Invoke(clientSocket, e.UserToken);

				return;
			}
			else
			{
				Console.WriteLine("Failed to accept client.");
			}

			flowControllEvent.Set();
		}
	}
}
