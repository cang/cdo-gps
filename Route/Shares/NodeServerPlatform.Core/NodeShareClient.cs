#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : NodeClient.cs
// Time Create : 10:06 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Threading;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols.BinarySerialization;
using Hik.Communication.Scs.Server;
using Log;
using NodeServerPlatform.Core.Models.Packets;

namespace NodeServerPlatform.Core
{
    public class NodeShareClient : INodeClient
    {
        private readonly IScsClient _client;
        private readonly NodeClientConfig _config;
        private readonly INodeShareHandleTable _handleTable;
        private readonly ILog _log;
        private readonly INodeSharePacketTable _packetTable;
        private readonly IScsServerClient _sClient;

        private int _reconnectFail;
        public int LimitReconect { get; set; } = 3;

        public NodeShareClient(NodeClientConfig config)
        {
            _config = config;
            _log = _config.Log;
            _packetTable = _config.PacketTab;
            _handleTable = _config.HandleTab;
            _client = ScsClientFactory.CreateClient(new ScsTcpEndPoint(_config.Ip, _config.Port));
            _client.Connected += _client_Connected;
            _client.MessageReceived += _client_MessageReceived;
            _client.Disconnected += _client_Disconnected;
            _client.WireProtocol = new BinarySerializationProtocol();
        }

        public NodeShareClient(NodeClientConfig config, IScsServerClient client)
        {
            _config = config;
            _log = _config.Log;
            _packetTable = _config.PacketTab;
            _handleTable = _config.HandleTab;
            _sClient = client;
            _sClient.Disconnected += _client_Disconnected;
            _sClient.MessageReceived += _client_MessageReceived;
            _sClient.WireProtocol = new BinarySerializationProtocol();
        }

        public string Name { get; set; }

        public bool Send(INodeShareSendPacket p)
        {
            if (null == _client && null == _sClient) return false;

            //Kiem tra phía client xem ket noi co chua
            if (null != _client && _client.CommunicationState != CommunicationStates.Connected) return false;

            //Kiem tra phía server xem ket noi client co chua
            //if (null != _sClient && _sClient.CommunicationState != CommunicationStates.Connected) return false;

            try
            {
                var opcode = _packetTable.GetOpcode(p.GetType());
                if (opcode >= -1)
                {
                    var data = NodeSharePacketFactory.CreateStream(new NodeBasePacket(opcode, p.Serializer()));

                    _client?.SendMessage(new ScsRawDataMessage(data));
                    _sClient?.SendMessage(new ScsRawDataMessage(data));

                    return true;
                }
                else
                {
                    _log.Warning("NodeClient", $"Type packet chưa được định nghĩa: {p.GetType()}");
                }
            }
            catch (Exception ex)
            {
                _log.Exception("NodeClient", ex, "Send packet lỗi");
            }

            return false;
        }

        public void Start()
        {
            if (_client.CommunicationState == CommunicationStates.Connected)
                return;

            _client.Connect();
        }

        public string GetRemoteIp()
        {
            return _client?.ToString() ?? _sClient?.RemoteEndPoint.ToString();
        }

        public event Action OnConected;

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _client?.Disconnect();
            _client?.Dispose();
            _sClient?.Disconnect();
        }

        #endregion

        public event Action<string, INodeClient> OnDisconnect;

        private void _client_Disconnected(object sender, EventArgs e)
        {
            Thread.Sleep(1000);//?
            if (_config.ReConnect)
            {
                _log.Info("NodeClient", $"{_config.Name}  Tiến hành kết nối lại từ client đến server do mất kết nối");
                try
                {
                    Start();
                }
                catch (Exception)
                {
                    _reconnectFail++;
                    if (LimitReconect != -1 && _reconnectFail > LimitReconect)
                    {
                        _log.Error("NodeClient",$"Không thể kết nối lại node {_config.Name} - {_config.Ip}:{_config.Port}");
                        if (!string.IsNullOrEmpty(_config.Name))
                            OnDisconnect?.Invoke(_config.Name, this);
                        return;
                    }
                    _client_Disconnected(sender, e);
                }
            }
            _reconnectFail = 0;
        }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                var dataMessage = e.Message as ScsRawDataMessage;
                if (dataMessage != null)
                {
                    var message = dataMessage;
                    var packet = NodeSharePacketFactory.CreatePacket(message.MessageData);
                    var type = _packetTable.GetPacket(packet.Opcode);
                    if (type != null)
                    {
                        var model = (INodeShareRecvPacket) Activator.CreateInstance(type, packet.Data);
                        model.Deserializer();
                        _log.Debug("NodeClient", $"Nhận được gói tin {packet.Opcode}---{model.GetType()}");
                        var obj = Convert.ChangeType(model, model.GetType());
                        //model.Handle(this);
                        var handle = _handleTable.GetHandle(packet.Opcode);
                        if (handle != null)
                        {
                            try
                            {
                                handle.GetHandle().DynamicInvoke(this, obj);
                            }
                            catch (Exception ex)
                            {
                                _log.Exception("NodeClient", ex,
                                    $"xử lý {handle.GetType()}   opcode : {packet.Opcode} lỗi");
                            }
                        }
                        else
                        {
                            _log.Warning("NodeClient",
                                $"Chưa có phần xử lý cho gói tin {packet.Opcode} - {model.GetType()}");
                        }
                    }
                    else
                    {
                        _log.Warning("NodeClient", $"Opcode chưa được định nghĩa: {packet.Opcode}");
                    }
                }
            }
            catch (Exception exception)
            {
                _log.Exception("NodeClient", exception, $"Xử lý gói tin lỗi");
            }
        }

        private void _client_Connected(object sender, EventArgs e)
        {
            _log.Info("NodeClient", $"Kết nối thành công tới server {_config.Name}---{_config.Ip}:{_config.Port}");
            OnConected?.Invoke();
        }

        public void GetListNeighbor()
        {
            if (_client != null)
            {
                // Hỏi thông tin node của thằng server
                Send(new P01GetListNeighbor());
            }
        }
    }
}