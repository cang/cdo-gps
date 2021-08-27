using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Windows.Networking;
using Windows.Networking.Sockets;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;

namespace Hik.Communication.Scs.Communication.Channels.Tcp
{
    /// <summary>
    /// This class is used to listen and accept incoming TCP
    /// connection requests on a TCP port.
    /// </summary>
    internal class TcpConnectionListener : ConnectionListenerBase
    {
        /// <summary>
        /// The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly ScsTcpEndPoint _endPoint;

        /// <summary>
        /// Server socket to listen incoming connection requests.
        /// </summary>
        private StreamSocketListener _listenerSocket;

        /// <summary>
        /// The thread to listen socket
        /// </summary>
        //private Thread _thread;

        /// <summary>
        /// A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        /// <summary>
        /// Creates a new TcpConnectionListener for given endpoint.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        public TcpConnectionListener(ScsTcpEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        /// <summary>
        /// Starts listening incoming connections.
        /// </summary>
        public override void Start()
        {
            try
            {

                StartSocket();
                _running = true;
            }
            catch (Exception)
            {
                
                throw;
            }
            // _thread = new Thread(DoListenAsThread);
            //_thread.Start();
        }

        /// <summary>
        /// Stops listening incoming connections.
        /// </summary>
        public override void Stop()
        {
            _running = false;
            StopSocket();
        }

        /// <summary>
        /// Starts listening socket.
        /// </summary>
        private void StartSocket()
        {
            _listenerSocket = new StreamSocketListener();
            _listenerSocket.Control.KeepAlive = true;
            _listenerSocket.Control.QualityOfService = SocketQualityOfService.Normal;
            _listenerSocket.ConnectionReceived += _listenerSocket_ConnectionReceived;
            try
            {
                _listenerSocket.BindEndpointAsync(new HostName(_endPoint.IpAddress), _endPoint.TcpPort.ToString());
                //.GetResults();
                // await _listenerSocket.BindServiceNameAsync(_endPoint.TcpPort.ToString());
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           
        }

        private void _listenerSocket_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            OnCommunicationChannelConnected(new TcpComunicationChannelUwp(args.Socket,
                                _endPoint.IsCheckConnect));

        }

        /// <summary>
        /// Stops listening socket.
        /// </summary>
        private async void StopSocket()
        {
            try
            {
               await _listenerSocket.CancelIOAsync();
                _listenerSocket.Dispose();
               // _listenerSocket.Stop();
            }
            catch
            {

            }
        }

        ///// <summary>
        ///// Entrance point of the thread.
        ///// This method is used by the thread to listen incoming requests.
        ///// </summary>
        //private void DoListenAsThread()
        //{
        //    while (_running)
        //    {
        //        try
        //        {
        //            var clientSocket = _listenerSocket.AcceptTcpClient();
        //            try
        //            {

        //                if (clientSocket.Connected)
        //                {

        //                    OnCommunicationChannelConnected(new TcpCommunicationChannel(clientSocket.Client,
        //                        _endPoint.IsCheckConnect));

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("Event Connect Error:  " + ex);
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex);
        //            //Disconnect, wait for a while and connect again.
        //            StopSocket();
        //            Thread.Sleep(1000);
        //            if (!_running)
        //            {
        //                return;
        //            }

        //            try
        //            {
        //                StartSocket();
        //            }
        //            catch
        //            {

        //            }
        //        }
        //    }
        //}
    }
}
