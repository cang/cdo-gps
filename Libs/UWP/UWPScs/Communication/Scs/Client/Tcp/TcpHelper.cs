using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Hik.Communication.Scs.Client.Tcp
{
    /// <summary>
    /// This class is used to simplify TCP socket operations.
    /// </summary>
    internal static class TcpHelper
    {
        static ManualResetEvent _clientDone = new ManualResetEvent(false);
        /// <summary>
        /// This code is used to connect to a TCP socket with timeout option.
        /// </summary>
        /// <param name="endPoint">IP endpoint of remote server</param>
        /// <param name="timeoutMs">Timeout to wait until connect</param>
        /// <returns>Socket object connected to server</returns>
        /// <exception cref="SocketException">Throws SocketException if can not connect.</exception>
        /// <exception cref="TimeoutException">Throws TimeoutException if can not connect within specified timeoutMs</exception>
        public static Socket ConnectToServer(EndPoint endPoint, int timeoutMs)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = endPoint;

                //socket.Blocking = false;
                //socket.Connect(endPoint);
                //socket.Blocking = true;
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
                {
                    // Retrieve the result of this request
                   // result = e.SocketError.ToString();

                    // Signal that the request is complete, unblocking the UI thread
                    _clientDone.Set();
                });

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();
                
                _clientDone.Reset();

                // Make an asynchronous Connect request over the socket
                socket.ConnectAsync(socketEventArg);

                // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(timeoutMs);

                return socket;
            }
            catch (SocketException socketException)
            {
                //if (socketException.ErrorCode != 10035)
                //{
                //    socket.Close();
                //    throw;
                //}

                //if (!socket.Poll(timeoutMs * 1000, SelectMode.SelectWrite))
                //{
                //    socket.Close();
                throw new TimeoutException("The host failed to connect. Timeout occured.");
                //}

                //socket.Blocking = true;
                return socket;
            }
        }
    }
}
