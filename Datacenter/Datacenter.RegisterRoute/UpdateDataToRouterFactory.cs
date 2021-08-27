#region header

// /*********************************************************************************************/
// Project :Datacenter.UpdateDataToRoute
// FileName : UpdateDataToRouterFactory.cs
// Time Create : 2:33 PM 22/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web.Hosting;
using ConfigFile;
using Log;
using NodeServerPlatform.Core;
using StarSg.Core;

namespace Datacenter.RegisterRoute
{
    [Export(typeof (IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UpdateDataToRouterFactory : IModuleFactory, IPartImportsSatisfiedNotification
    {
        internal static readonly string ConfigPath = HostingEnvironment.MapPath("~/bin/Config/")+"UpdataConfig.xml";
        [Import] private UpdateData _client;
        [Import] private IConfigManager _configManager;
        [Import] private INodeShareHandleTable _handleTable;
        [Import] private ILog _log;
        [Import] private INodeSharePacketTable _packetTable;

        internal static UpdateDataConfig Config { get; set; }

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            _log.Info("UpdateDataToRouterFactory", "OnImportsSatisfied");
            //todo : load các thông tin cần thiết ở đây.

            // đọc các cấu hình từ file config lên
            Config = _configManager.Read<UpdateDataConfig>(ConfigPath);

            // mở kết nối qua route
            Task.Factory.StartNew(ConnectToRouter);
        }

        private void ConnectToRouter()
        {
            try
            {
                _log.Info("UpdateDataToRouterFactory", "ConnectToRouter");
                _client.Connect(Config.DefaultIp, Config.Port);
            }
            catch (Exception e)
            {
                _log.Exception("UpdateDataToRouterFactory", e,"Kết nối tới router lỗi");
                _log.Info("UpdateDataToRouterFactory", "ConnectToRouter bị lỗi, tiến hành kết nối lại");
                ConnectToRouter();
            }

        }

        #endregion
    }
}