using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace NetworkLibrary
{
	// https://msdn.microsoft.com/ko-kr/library/system.net.sockets.socketasynceventargs.socketasynceventargs(v=vs.100).aspx
	// 의 클래스를 그대로 사용.
	internal class SocketAsyncEventArgsPool
    {
		Stack<SocketAsyncEventArgs> pool;

		// Initializes the object pool to the specified size
		//
		// The "capacity" parameter is the maximum number of 
		// SocketAsyncEventArgs objects the pool can hold
		public SocketAsyncEventArgsPool(int capacity)
		{
			pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		// Add a SocketAsyncEventArg instance to the pool
		//
		//The "item" parameter is the SocketAsyncEventArgs instance 
		// to add to the pool
		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
			lock (pool)
			{
				pool.Push(item);
			}
		}

		// Removes a SocketAsyncEventArgs instance from the pool
		// and returns the object removed from the pool
		public SocketAsyncEventArgs Pop()
		{
			lock (pool)
			{
				return pool.Pop();
			}
		}

		// The number of SocketAsyncEventArgs instances in the pool
		public int Count
		{
			get { return pool.Count; }
		}


	}
}
