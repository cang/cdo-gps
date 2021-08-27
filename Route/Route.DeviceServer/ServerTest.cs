#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : Server.cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.ComponentModel.Composition;
using System.IO;
using System.Web.Hosting;
using ConfigFile;
using CorePacket;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Server;
using Log;
using Route.Core;
using ServerCore;
using StarSg.Core;

#endregion

namespace Route.DeviceServer
{
    /// <summary>
    ///     server chạy hệ thống nhận dự liệu từ phần cứng
    /// </summary>
    [Export(typeof (IModuleFactory))]
    [Export(typeof (ServerTest))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ServerTest : IModuleFactory, IPartImportsSatisfiedNotification
    {
        private const string ConfigFolder = "Config";
        private readonly string _path = HostingEnvironment.MapPath("~/bin/");

        [Import] private IClientCachePacket _cachePacket;

        /// <summary>
        ///     chưa thông tin cấu hình của server
        /// </summary>
        private ServerConfigTest _cfg;

        [Import] private IConfigManager _configManager;

        [Import] private IDeviceRouteTable _deviceRoute;

        [Import] private IDevicePacketHandleTable _handleTable;

        [Import] private ILog _log;

        [Import] private IDevicePacketTable _packetTable;

        private IScsServer _server;
        public int MaxConnect { get; } = 10000;
        public int Concurent => _server.Clients.Count;

        public void OnImportsSatisfied()
        {
            if (!Directory.Exists(_path + @"\" + ConfigFolder))
                Directory.CreateDirectory(_path + @"\" + ConfigFolder);
            _cfg = _configManager.Read<ServerConfigTest>($@"{_path}\{ConfigFolder}\DeviceServerTest.xml");
            _log.Debug("SERVER", $"Begin start device listener  {_cfg.Ip}:{_cfg.Port}");
            _server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(_cfg.Ip, _cfg.Port, true));
            _server.MaxConcurentConnect = MaxConnect;
            _server.ClientConnected += _server_ClientConnected;
            _server.Start();
            _log.Success("SERVER", "Khởi động thành công hệ thống server nhận dữ liệu từ thiết bị");
        }

        /// <summary>
        ///     tiếp nhận thông tin kết nối
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            new ClientConnectTest(e.Client, _log, _packetTable, _handleTable, _cachePacket, _deviceRoute);
        }
    }
}