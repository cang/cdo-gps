using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;
using Timer = Hik.Threading.Timer;

namespace Hik.Communication.Scs.Communication.Channels.Tcp
{
    /// <summary>
    /// This class is used to communicate with a remote application over TCP/IP protocol.
    /// </summary>
    internal class TcpCommunicationChannel : CommunicationChannelBase
    {
        #region Public properties

        ///<summary>
        /// Gets the endpoint of remote application.
        ///</summary>
        public override ScsEndPoint RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }
        }
        private readonly ScsTcpEndPoint _remoteEndPoint;

        #endregion

        #region Private fields

        /// <summary>
        /// Size of the buffer that is used to receive bytes from TCP socket.
        /// </summary>
        private const int ReceiveBufferSize = 4 * 1024; //4KB

        /// <summary>
        /// This buffer is used to receive bytes 
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        /// Socket object to send/reveice messages.
        /// </summary>
        private readonly Socket _clientSocket;

        /// <summary>
        /// A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        /// <summary>
        /// This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object _syncLock;

        #endregion

        private readonly Timer _tCheckValidConnect = new Timer(10000);
        #region Constructor

        /// <summary>
        /// Creates a new TcpCommunicationChannel object.
        /// </summary>
        /// <param name="clientSocket">A connected Socket object that is
        /// used to communicate over network</param>
        /// <param name="realtimeCheck"></param>
        public TcpCommunicationChannel(Socket clientSocket,bool realtimeCheck)
        {
            _clientSocket = clientSocket;
            _clientSocket.NoDelay = true;

            var ipEndPoint = (IPEndPoint)_clientSocket.RemoteEndPoint;
            _remoteEndPoint = new ScsTcpEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port);

            _buffer = new byte[ReceiveBufferSize];
            _syncLock = new object();
            //if (realtimeCheck)
            //{
            //    _tCheckValidConnect.Elapsed += _tCheckValidConnect_Elapsed;
            //    _tCheckValidConnect.Start();
            //}
        }

        

  

        #endregion

        #region Public methods

        /// <summary>
        /// Disconnects from remote application and closes channel.
        /// </summary>
        public override void Disconnect()
        {
            if (CommunicationState != CommunicationStates.Connected)
            {
                return;
            }

            _running = false;
            try
            {
                if (_clientSocket.Connected)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                }

                _clientSocket.Dispose();
            }
            catch
            {

            }
            _tCheckValidConnect.Stop();
            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Starts the thread to receive messages from socket.
        /// </summary>
        protected override void StartInternal()
        {
            _running = true;
            if (_clientSocket != null)
            {
                //_clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _clientSocket.RemoteEndPoint;
                socketEventArg.SetBuffer(_buffer, 0, _buffer.Length);
                socketEventArg.Completed += ReceiveCallback;
                _clientSocket.ReceiveAsync(socketEventArg);
            }
           
            //_clientSocket.
        }

        /// <summary>
        /// Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected override void SendMessageInternal(IScsMessage message)
        {
            //Send message
            var totalSent = 0;
            lock (_syncLock)
            {
                //Create a byte array from message according to current protocol
                var messageBytes = WireProtocol.GetBytes(message);
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _clientSocket.RemoteEndPoint;
                socketEventArg.UserToken = null;
                socketEventArg.Completed += (s, e) =>
                {
                    totalSent += messageBytes.Length - totalSent;
                    if (totalSent < messageBytes.Length)
                    {
                        e.SetBuffer(messageBytes, totalSent, messageBytes.Length - totalSent);
                        _clientSocket.SendAsync(socketEventArg);
                    }
                };
                //Send all bytes to the remote application
                //while (totalSent < messageBytes.Length)
                {
                    //var sent = _clientSocket.Send(messageBytes, totalSent, messageBytes.Length - totalSent, SocketFlags.None);
                   
                    // Set properties on context object
                   
                    socketEventArg.SetBuffer(messageBytes, totalSent, messageBytes.Length - totalSent);
                    _clientSocket.SendAsync(socketEventArg);
                    //if (sent <= 0)
                    //{
                    //    throw new CommunicationException("Message could not be sent via TCP socket. Only " + totalSent + " bytes of " + messageBytes.Length + " bytes are sent.");
                    //}

                }

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// This method is used as callback method in _clientSocket's BeginReceive method.
        /// It reveives bytes from socker.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void ReceiveCallback(object s, SocketAsyncEventArgs e)
        {
            if(!_running)
            {
                return;
            }

            try
            {
                //Get received bytes count
                var bytesRead = e.BytesTransferred;
                if (bytesRead > 0)
                {
                    LastReceivedMessageTime = DateTime.Now;

                    //Copy received bytes to a new byte array
                    var receivedBytes = new byte[bytesRead];
                    Array.Copy(_buffer, 0, receivedBytes, 0, bytesRead);

                    //Read messages according to current wire protocol
                    var messages = WireProtocol.CreateMessages(receivedBytes);
                    
                    //Raise MessageReceived event for all received messages
                    foreach (var message in messages)
                    {
                        OnMessageReceived(message);
                    }
                }
                else
                {
                    throw new CommunicationException("Tcp socket is closed");
                }

                //Read more bytes if still running
                if (_running)
                {
                    _clientSocket.ReceiveAsync(e);
                   // _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
                }
            }
            catch(Exception ex)
            {
               // Console.WriteLine(ex);
                Disconnect();
            }
        }
        
        #endregion
    }

    internal class TcpComunicationChannelUwp : CommunicationChannelBase
    {
        private StreamSocket socket;
        private bool isCheckConnect;
        private CancellationTokenSource _cancel;
        private Task _recvData;
        public override ScsEndPoint RemoteEndPoint { get; }
        /// <summary>
        /// Size of the buffer that is used to receive bytes from TCP socket.
        /// </summary>
        private const int ReceiveBufferSize = 4 * 1024; //4KB

        /// <summary>
        /// This buffer is used to receive bytes 
        /// </summary>
        private readonly byte[] _buffer;

        private readonly object _syncLock = new object();

        public TcpComunicationChannelUwp(StreamSocket socket, bool isCheckConnect)
        {
            _cancel = new CancellationTokenSource();
            this.socket = socket;
            this.isCheckConnect = isCheckConnect;
            RemoteEndPoint =
                new ScsTcpEndPoint(this.socket.Information.RemoteAddress.DisplayName,
                    int.Parse(this.socket.Information.RemotePort));
            _buffer = new byte[ReceiveBufferSize];

        }

        private async void DetectClientDisconect()
        {
            while (true)
            {
                if (_cancel.IsCancellationRequested)
                    return;
                await Task.Delay(5000, _cancel.Token).ContinueWith((m) => { });
                SendMessage(new ScsRawDataMessage(new byte[1] {0}));
            }
        }

        public async override void Disconnect()
        {
            try
            {
                if (!_cancel.IsCancellationRequested)
                    _cancel.Cancel();
                else return;
                CommunicationState = CommunicationStates.Disconnected;
                await socket.CancelIOAsync();
                socket?.Dispose();
                OnDisconnected();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
           
        }

        protected override void StartInternal()
        {
            _recvData = Task.Factory.StartNew(RecvHandle,_cancel.Token);
            Task.Factory.StartNew(DetectClientDisconect);
        }

        private async void RecvHandle()
        {
            try
            {
                while (true)
                {
                    if (_cancel.IsCancellationRequested)
                        return;
                    // if()
                    var stream = socket.InputStream.AsStreamForRead();
                    {
                        var len =
                            await stream.ReadAsync(_buffer, 0, ReceiveBufferSize, _cancel.Token).ConfigureAwait(false);
                        if (len > 0)
                        {
                            var receivedBytes = new byte[len];
                            Array.Copy(_buffer, 0, receivedBytes, 0, len);

                            //Read messages according to current wire protocol
                            var messages = WireProtocol.CreateMessages(receivedBytes);

                            //Raise MessageReceived event for all received messages
                            foreach (var message in messages)
                            {
                                OnMessageReceived(message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // An established connection was aborted by the software in your host machine.
                Debug.WriteLine($"Nhận dữ liệu lỗi : {e}");
                Disconnect();
            }

        }

        protected async override void SendMessageInternal(IScsMessage message)
        {
            try
            {
                var totalSent = 0;
               // lock (_syncLock)
                {
                    //Create a byte array from message according to current protocol
                    var messageBytes = WireProtocol.GetBytes(message);
                    //SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                    //socketEventArg.RemoteEndPoint = _clientSocket.RemoteEndPoint;
                    //socketEventArg.UserToken = null;
                    //socketEventArg.Completed += (s, e) =>
                    //{
                    //    totalSent += messageBytes.Length - totalSent;
                    //    if (totalSent < messageBytes.Length)
                    //    {
                    //        e.SetBuffer(messageBytes, totalSent, messageBytes.Length - totalSent);
                    //        _clientSocket.SendAsync(socketEventArg);
                    //    }
                    //};
                    //Send all bytes to the remote application
                   // while (totalSent < messageBytes.Length)
                    {
                        //var sent = _clientSocket.Send(messageBytes, totalSent, messageBytes.Length - totalSent, SocketFlags.None);
                        var stream = socket.OutputStream.AsStreamForWrite();
                        {
                            
                           // await
                            await stream.WriteAsync(messageBytes, 0, messageBytes.Length,_cancel.Token).ConfigureAwait(false);
                            //.ContinueWith((m) => { }).Wait(_cancel.Token);
                            //await 
                            await stream.FlushAsync(_cancel.Token).ConfigureAwait(false); //.ContinueWith((m) => { }).Wait(_cancel.Token);
                            //totalSent += messageBytes.Length - totalSent;
                            //.GetAwaiter();
                        }
                        // Set properties on context object

                        //socketEventArg.SetBuffer(messageBytes, totalSent, messageBytes.Length - totalSent);
                        //_clientSocket.SendAsync(socketEventArg);
                        //if (sent <= 0)
                        //{
                        //    throw new CommunicationException("Message could not be sent via TCP socket. Only " + totalSent + " bytes of " + messageBytes.Length + " bytes are sent.");
                        //}

                    }
                    //if (messageBytes.Length == 0)
                    //{

                    //    var stream = socket.OutputStream.AsStreamForWrite();
                    //    {
                    //        await stream.WriteAsync(new byte[0], 0, 0, _cancel.Token);
                    //        await stream.FlushAsync(_cancel.Token);
                    //        //.GetAwaiter();
                    //    }
                    //}

                    LastSentMessageTime = DateTime.Now;
                    OnMessageSent(message);
                }
                
            }
            catch (Exception e)
            {
                Disconnect();
            }
        }
    }
}
