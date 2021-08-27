#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : Global.cs
// Time Create : 2:44 PM 28/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml;
using Core;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Models;
using NodeServerPlatform.Server.Cfg;
using StarSg.Core;
using StarSg.Utils;

namespace NodeServerPlatform.Server
{
    [Export(typeof (IModuleFactory))]
    [Export(typeof (NodeServerFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NodeServerFactory : IModuleFactory, IPartImportsSatisfiedNotification
    {
        //private IList<string> PendingConnections { get; } = new List<string>();
        private readonly string _path = HostingEnvironment.MapPath("~/bin/Config/");
        [Import] private INodeShareHandleTable _handleTable;
        [Import] private ILog _log;
        [Import] private INodeSharePacketTable _packetTable;
        [Import] private NodeShareServer _sever;
        // [ImportMany] private IHandlePacket _hs; 
        public ConfigFile Config { get; private set; }
        public IDictionary<string, INodeClient> Clients { get; set; } = new Dictionary<string, INodeClient>();

        public async void ConnectToNewNeighbor(string name, string ip, int port)
        {
            //lock (Clients)
            {
                if (Config.Name == name) return;
                if (Clients.ContainsKey(name)) return;
                _log.Info("Global", $"{name} : {ip}:{port}");
                await TryConnectToNode(name, ip, port);
                if (!Config.Neighbor.ContainsKey(name))
                {
                    Config.Neighbor.Add(name, new NeighborInfo {Ip = ip, Name = name, Port = (short) port});
                    SaveConfig();
                }
            }
        }


        private void AddClient(string name, NodeShareClient client)
        {
            if (!Clients.ContainsKey(name))
            {
                client.OnDisconnect += Client_OnDisconnect;
                Clients.Add(name, client);
                client.GetListNeighbor();
            }
            else
            {
                client.Dispose();
            }
        }

        private void Client_OnDisconnect(string name, INodeClient client)
        {
            if (string.IsNullOrEmpty(name)) return;
            Clients.Remove(name);
        }

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            //try
            //{
                ReadConfig();

                _sever.StartServer(new NodeServerConfig { Ip = Config.Ip, Port = Config.Port, Name = Config.Name });
                _log.Success("Global", $"Khởi động server {Config.Name}");
               // Console.Title = Config.Name;
                ConnectToNeighbor();
                AlertNodeAlone();

            //}
            //catch (Exception ex)
            //{
            //    _log.Exception("ServerFactory",ex,"Load Server Factory error");
            //}
            
        }

        private void AlertNodeAlone()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (Clients.Count == 0)
                    {
                        //_log.Warning("Global", "Node dang chạy ở trạng thái không liên kết , vui lòng cấu hình lại");
                    }

                    await Task.Delay(500);
                }
            });
        }

        private void SaveConfig()
        {
            lock (Config)
            {
                var ser = new DataContractSerializer(typeof (ConfigFile));
                if (File.Exists(_path + "nodeconfig.xml"))
                    File.Delete(_path + "nodeconfig.xml");
                var fStream = File.Create(_path + "nodeconfig.xml");
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t"
                };
                var writer = XmlWriter.Create(fStream, settings);
                //cfg.Fix();
                ser.WriteObject(writer, Config);
                writer.Flush();
                writer.Dispose();
                fStream.Dispose();
            }
        }

        private async Task TryConnectToNode(string name, string ip, int port)
        {
            _log.Debug("Global", $"Start  connect {name}  {ip}:{port}");
            var fail = 0;
            while (true)
            {
                try
                {
                    //lock (Clients)
                    {
                        if (Clients.ContainsKey(name))
                            return;
                        if (Config.Name == name) return;
                        var connect = new NodeShareClient(new NodeClientConfig(_log, _packetTable, _handleTable)
                        {
                            Ip = ip,
                            Port = port,
                            ReConnect = true,
                            Name = name
                        }) {Name = name};

                        connect.Start();
                        //Clients.Add(Config.TmpName, connect);
                        AddClient(name, connect);
                        _log.Debug("Global", $"Kết nối qua node {connect.GetRemoteIp()}");
                        break;
                    }
                }
                catch (Exception)
                {
                    fail++;
                    if (fail > 3)
                    {
                        if (Config.Neighbor.ContainsKey(name))
                        {
                            Config.Neighbor.Remove(name);
                            SaveConfig();
                        }
                        _log.Error("Global", $"Không thể kết nối tới node {name} - {ip}:{port}");
                        return;
                    }
                    await Task.Delay(5000);
                    _log.Warning("Global", $"Kết nối lại lần {fail} tới node {name}");
                }
            }
        }

        private async void ConnectToNeighbor()
        {
            if (Config.Neighbor.Count == 0)
            {
                // kết nối vào 1 node mặc định đã cấu hình từ đầu
                if (!string.IsNullOrEmpty(Config.TmpIp))
                {
                    await
                        Task.Factory.StartNew(
                            async () => await TryConnectToNode(Config.TmpName, Config.TmpIp, Config.TmpPort));
                }
            }
            else
            {
                // kết nối vào danh sách các node hàng xóm đã lưu
                var tmp = Config.Neighbor.Values.ToList();
                foreach (var info in tmp.Where(m => !Clients.ContainsKey(m.Name)))
                {
                    await Task.Factory.StartNew(async () => await TryConnectToNode(info.Name, info.Ip, info.Port));
                }
            }
        }

        private void ReadConfig()
        {
            if (!File.Exists(_path + "nodeconfig.xml"))
            {
                Config = new ConfigFile
                {
                    Ip = "127.0.0.1",
                    Name = "Node_0",
                    Port = 1300,
                    TmpIp = string.Empty,
                    TmpPort = 0
                };

                SaveConfig();
            }
            else
            {
                var fStream = File.Open(_path + "nodeconfig.xml", FileMode.Open);

                var reader = XmlReader.Create(fStream);
                var ser = new DataContractSerializer(typeof (ConfigFile));
                Config = (ConfigFile) ser.ReadObject(reader);
                reader.Dispose();
                fStream.Dispose();
            }
        }

        #endregion
    }
}