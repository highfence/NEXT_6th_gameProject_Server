using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;


namespace NetworkLibrary
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up and assigned to SocketAsyncEventArgs objects for use
    /// with each socket I/O operation.  This enables bufffers to be easily reused and gaurds against fragmenting heap memory.
    /// 
    /// The operations exposed on the BufferManager class are not thread safe.
    /// </summary>
    internal class BufferManager
    {

        int TotalBufferSize;                 // the total number of bytes controlled by the buffer pool
        byte[] Buffer;                // the underlying byte array maintained by the Buffer Manager
        int CurrentIndex;
        int BufferSizeBySocketAsyncEventArgs;

        public BufferManager(int totalBytes, int bufferSize)
        {
            if ((totalBytes % bufferSize) != 0)
            {
                throw new System.ArgumentException("P(totalBytes % bufferSize) != 0", "bufferSize");
            }

            TotalBufferSize = totalBytes;
            CurrentIndex = 0;
            BufferSizeBySocketAsyncEventArgs = bufferSize;
        }

        /// <summary>
        /// Allocates buffer space used by the buffer pool
        /// </summary>
        public void InitBuffer()
        {
            // create one big large buffer and divide that out to each SocketAsyncEventArg object
            Buffer = new byte[TotalBufferSize];
        }

        /// <summary>
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object
        /// </summary>
        /// <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if ((TotalBufferSize - BufferSizeBySocketAsyncEventArgs) < CurrentIndex)
            {
                return false;
            }

            args.SetBuffer(Buffer, CurrentIndex, BufferSizeBySocketAsyncEventArgs);
            CurrentIndex += BufferSizeBySocketAsyncEventArgs;

            return true;
        }
    }
