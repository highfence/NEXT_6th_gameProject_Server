using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NetworkLibrary
{
    public class NetworkService
    {
		ClientListener clientListener;

		SocketAsyncEventArgsPool receiveEventArgsPool;
		SocketAsyncEventArgsPool sendEventArgsPool;

		BufferManager bufferManager;

		public delegate void SessionHandler(ClientSession session);
		public SessionHandler OnSessionCreated { get; set; }

		public void Initialize()
		{
			// TODO :: 서버 config 클래스 구현.
			const int maxConnections = 10000;
			const int bufferSize = 1024;
			const int preAllocCount = 1;

			receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnections);
			sendEventArgsPool = new SocketAsyncEventArgsPool(maxConnections);

			bufferManager = new BufferManager(maxConnections * bufferSize * preAllocCount, bufferSize);

			SocketAsyncEventArgs arg;

			for (var i = 0; i < maxConnections; ++i)
			{
				// Receive Pool 생성
				{
					arg = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
					arg.UserToken = null;

					bufferManager.SetBuffer(arg);

					receiveEventArgsPool.Push(arg);
				}


				// Send Pool 생성
				{
					arg = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
					arg.UserToken = null;

					// send 버퍼는 보낼 때 따로 지정.
					arg.SetBuffer(null, 0, 0);

					sendEventArgsPool.Push(arg);
				}
			}
		}

		private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Receive)
			{
				ProcessReceive(e);
				return;
			}

			throw new ArgumentException("OnReceivedCompleted error. Last operation on the socket was not a receive.");
		}

		public void Listen(string host, int port, int backlog)
		{
			clientListener = new ClientListener();
			clientListener.OnClientConnected += OnClientConnected;
			clientListener.StartListen(host, port, backlog);
		}

		private void OnClientConnected(Socket clientSocket, object token)
		{
			var receiveArgs = receiveEventArgsPool.Pop();
			var sendArgs = sendEventArgsPool.Pop();

			var userToken = new ClientSession();
			userToken.SetEventArgs(receiveArgs, sendArgs);
			receiveArgs.UserToken = userToken;

			OnSessionCreated?.Invoke(receiveArgs.UserToken as ClientSession);

			// 클라이언트로부터 데이터를 수신할 준비를 한다.
			BeginReceive(clientSocket, receiveArgs, sendArgs);
		}

		private void BeginReceive(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
		{
			var session = receiveArgs.UserToken as ClientSession;
			session.socket = clientSocket;

			// 비동기 수신 시작.
			bool pending = clientSocket.ReceiveAsync(receiveArgs);
			if (pending == false)
			{
				ProcessReceive(receiveArgs);
			}
		}

		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			ClientSession session = e.UserToken as ClientSession;
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				// 이 이후의 작업은 각 세션에서 진행한다.
				session.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);

				// 다음 메시지 수신.
				var pending = session.socket.ReceiveAsync(e);
				if (pending == false)
				{
					ProcessReceive(e);
				}
			}
			else
			{
				try
				{
					session.Close();
				}
				catch (Exception)
				{
					Console.WriteLine("Already closed socket.");
				}
			}
		}
	}
}
