using System;
using System.ComponentModel.Composition;
using System.Linq;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Server;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Models.Packets;

namespace NodeServerPlatform.Server
{
    public abstract class BaseNodeServer : INodeShareServer
    {
        private NodeServerConfig _config;
        [Import] private INodeShareHandleTable _handleTable;

        [Import] private ILog _log;
        [Import] private INodeSharePacketTable _packetTable;
        private IScsServer _server;

        public void StartServer(NodeServerConfig config)
        {
            _config = config;
            _server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(_config.Ip, _config.Port));
            _server.ClientConnected += _server_ClientConnected;
            _server.ClientDisconnected += _server_ClientDisconnected;
            _server.Start();
            Name = _config.Name;
            //_log.Success("NodeServer", "Started Server");
        }

        private void _server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            _log.Debug("NodeServer", $"Node ngắt kết nối {e.Client}");
        }

        private void _server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            _log.Debug("NodeServer", $"Có node mới kết nối {e.Client}");
            var cl = new NodeShareClient(new NodeClientConfig(_log, _packetTable, _handleTable), e.Client);
            // hỏi thằng kết nối thông tin của nó
            cl.Send(new P03YourInfo());
            e.Client.Tag = cl;
        }

        #region Implementation of INodeServer

        public void SendAll(INodeShareSendPacket p)
        {
            try
            {
                var list = _server.Clients.GetAllItems();
                foreach (var c in list.Select(client => client.Tag as INodeClient))
                {
                    c?.Send(p);
                }
            }
            catch (Exception e)
            {
                _log.Exception("NodeServer", e, "Send all message error");
            }
        }

        public void SendTo(string nodeName, INodeShareSendPacket p)
        {
            var client = _server.Clients.GetAllItems();
            _log.Debug("base-node", $"số lượng client :{client.Count}");
            var t = client.FirstOrDefault(m =>
            {
                var c = m.Tag as INodeClient;
                if (m.Tag != null)
                    _log.Debug("base-node", $"client tag type : {m.Tag.GetType()}");
                if (c == null) return false;
                _log.Debug("base-node", $"node name :{c.Name}");
                return c.Name == nodeName;
            })?.Tag as INodeClient;

            if (t == null)
            {
                _log.Warning("", $"Không tìm thấy node với tên {nodeName}");
                return;
            }

            t.Send(p);
        }

        public string Name { get; private set; }

        #endregion
    }
}