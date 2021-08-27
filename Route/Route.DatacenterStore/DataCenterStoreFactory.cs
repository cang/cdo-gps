#region header

// /*********************************************************************************************/
// Project :Route.DatacenterStore
// FileName : DataCenterStoreFactory.cs
// Time Create : 9:09 AM 24/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using System.Web.Hosting;
using ConfigFile;
using Log;
using NodeServerPlatform.Server;
using StarSg.Core;

namespace Route.DatacenterStore
{
    [Export(typeof (IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DataCenterStoreFactory : IModuleFactory, IPartImportsSatisfiedNotification
    {
        private string Configfile = HostingEnvironment.MapPath("~/bin/Config/")+"Datacenter.xml";
        [Import] private IConfigManager _configManager;
        [Import] private ILog _log;
        [Import] private DataCenterStoreServer _server;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            _log.Debug("Datacenter", "khởi động hệ thống datacenter store");

            //todo : đọc file cấu hình lên 
            var config = _configManager.Read<DatacenterStoreConfig>(Configfile);
            //todo: cài đặt và chạy server
            _server.StartServer(new NodeServerConfig {Ip = config.Ip, Port = config.Port});
        }

        #endregion
    }
}