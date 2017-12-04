using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkLibrary
{
	public class NetworkService
    {
		ClientListener clientListener;

		SocketAsyncEventArgsPool receiveEventArgsPool;
		SocketAsyncEventArgsPool sendEventArgsPool;

		HttpNetwork			httpNetwork;
		BufferManager		bufferManager;
		ISessionManageable	userManager;
		IPacketHandleable   logicHandler;

		public delegate void SessionHandler(Session session);
		public SessionHandler OnSessionCreated { get; set; }


		public void Initialize(IPacketHandleable logicHandler, ISessionManageable userManager)
		{
			// TODO :: 서버 config 클래스 구현.
			const int maxConnections = 10000;
			const int bufferSize     = 1024;
			const int preAllocCount  = 2;

			bufferManager = new BufferManager(maxConnections * bufferSize * preAllocCount, bufferSize);
			bufferManager.InitBuffer();

			receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnections);
			sendEventArgsPool    = new SocketAsyncEventArgsPool(maxConnections);
			MakeEventPools(maxConnections);

			httpNetwork = new HttpNetwork();

			this.userManager  = userManager;
			this.logicHandler = logicHandler;
		}


		/// <summary>
		/// Http Post를 보내는 래핑 메소드.
		/// </summary>
		/// <typeparam name="REQUEST_T"></typeparam>
		/// <typeparam name="RESULT_T"></typeparam>
		/// <param name="postUri"></param>
		/// <param name="postData"></param>
		/// <returns></returns>
		public async Task<RESULT_T> HttpPost<REQUEST_T, RESULT_T>(string postUri, REQUEST_T postData) where RESULT_T : new()
		{
			return await httpNetwork.HttpPostRequest<REQUEST_T, RESULT_T>(postUri, postData);
		}


		/// <summary>
		/// 통신에 사용할 이벤트 풀을 생성하는 메소드.
		/// </summary>
		/// <param name="maxConnections"></param>
		private void MakeEventPools(int maxConnections)
		{
			SocketAsyncEventArgs arg;

			for (var i = 0; i < maxConnections; ++i)
			{
				// Receive Pool 생성
				{
					arg            = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
					arg.UserToken  = null;

					bufferManager.SetBuffer(arg);

					receiveEventArgsPool.Push(arg);
				}

				// Send Pool 생성
				{
					arg            = new SocketAsyncEventArgs();
					arg.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
					arg.UserToken  = null;

					// send 버퍼는 보낼 때 따로 지정.
					arg.SetBuffer(null, 0, 0);

					sendEventArgsPool.Push(arg);
				}
			}
		}


		/// <summary>
		/// 비동기 Send가 완료되었을 경우 호출되는 콜백 메서드.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void OnSendCompleted(object sender, SocketAsyncEventArgs eventArgs)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// 비동기 Receive가 완료되었을 경우 호출되는 콜백 메서드.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void OnReceiveCompleted(object sender, SocketAsyncEventArgs eventArgs)
		{
			if (eventArgs.LastOperation == SocketAsyncOperation.Receive)
			{
				ProcessReceive(eventArgs);
				return;
			}

			throw new ArgumentException("OnReceivedCompleted error. Last operation on the socket was not a receive.");
		}


		/// <summary>
		/// clientListener를 통해 새로운 클라이언트를 받기 시작하는 메소드.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="backlog"></param>
		public void Listen(string host, int port, int backlog)
		{
			clientListener = new ClientListener();
			clientListener.OnClientConnected += OnNewClientConnected;
			clientListener.StartListen(host, port, backlog);
		}


		/// <summary>
		/// 새로운 클라이언트가 접속하였을 때 호출되는 메소드.
		/// </summary>
		/// <param name="clientSocket"></param>
		/// <param name="token"></param>
		private void OnNewClientConnected(Socket clientSocket, object token)
		{
			Console.WriteLine($"New Client Connected. Socket handle({clientSocket.Handle})");

			var receiveArgs = receiveEventArgsPool.Pop();
			var sendArgs	= sendEventArgsPool.Pop();

			var session = new Session(logicHandler);
			receiveArgs.UserToken = session;
			sendArgs.UserToken	  = session;

			session.SetEventArgs(receiveArgs, sendArgs);
			session.Socket = clientSocket;

			OnSessionCreated?.Invoke(receiveArgs.UserToken as Session);

			// 클라이언트로부터 데이터를 수신할 준비를 한다.
			BeginReceive(clientSocket, receiveArgs, sendArgs);
		}


		/// <summary>
		/// 비동기 Receive를 시작 시키는 메서드.
		/// </summary>
		/// <param name="clientSocket"></param>
		/// <param name="receiveArgs"></param>
		/// <param name="sendArgs"></param>
		private void BeginReceive(Socket clientSocket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
		{
			try
			{
				// 비동기 수신 시작.
				bool pending = clientSocket.ReceiveAsync(receiveArgs);
				if (pending == false)
				{
					ProcessReceive(receiveArgs);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("BeginReceive failed. Message : " + e.Message);
			}
		}


		/// <summary>
		/// 비동기 Receive를 진행하는 메서드.
		/// </summary>
		/// <param name="eventArgs"></param>
		private void ProcessReceive(SocketAsyncEventArgs eventArgs)
		{
			Session session = eventArgs.UserToken as Session;
			if (eventArgs.BytesTransferred > 0 && eventArgs.SocketError == SocketError.Success)
			{
				// 이 이후의 작업은 각 세션에서 진행한다.
				session.OnReceive(eventArgs.Buffer, eventArgs.Offset, eventArgs.BytesTransferred);

				// 다음 메시지 수신.
				var pending = session.Socket.ReceiveAsync(eventArgs);
				if (pending == false)
				{
					ProcessReceive(eventArgs);
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
