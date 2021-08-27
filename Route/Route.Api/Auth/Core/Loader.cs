#region header

// /*********************************************************************************************/
// Project :Authentication
// FileName : Loader.cs
// Time Create : 9:22 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using ConfigFile;
using DaoDatabase;
using DaoDatabase.AutoMapping;
using Route.Api.Auth.Core.ConfigFile;
using System;

namespace Route.Api.Auth.Core
{
    /// <summary>
    ///     load các thông tin cần thiết của hệ thống
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Loader : IPartImportsSatisfiedNotification
    {
        private const string Path = "Config/AuthConfig.xml";
        [Import]
        private IConfigManager _configManager;

        /// <summary>
        ///     Cấu hình
        /// </summary>
        public AuthConfig Config { get; private set; }

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            if (!Directory.Exists(HostingEnvironment.MapPath("~/bin/") + "Config"))
                Directory.CreateDirectory(HostingEnvironment.MapPath("~/bin/") + "Config");

            Config = _configManager.Read<AuthConfig>(HostingEnvironment.MapPath("~/bin/") + Path);
            //_configManager.Write<AuthConfig>(Config, HostingEnvironment.MapPath("~/bin/") + "Config/AuthConfigOut.xml");


            if (!String.IsNullOrWhiteSpace(Config.RouteDomain)) AuthConfig.RouteDomainUrl = Config.RouteDomain;
            if (!String.IsNullOrWhiteSpace(Config.GeoServer)) AuthConfig.GeoServerUrl = Config.GeoServer;



            var mapEntity =
                Assembly.GetAssembly(typeof(Models.Entity.Account))
                    .GetTypes()
                    .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                    .Select(m =>
                    {
                        var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                        return makeme;
                    }).ToList();

            UnitOfWorkFactory.RegisterDatabase(Config.DbName, new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                    DatabaseConfigFactory.GetDataConfig(false, Config.DbIp, 0, Config.DbName,
                        Config.DbUser, Config.DbPass, false, null)
            });
        }

        #endregion

        /// <summary>
        ///     tạo kênh kết nối tới máy chủ dữ liệu
        /// </summary>
        /// <returns></returns>
        public Reponsitory GetContext()
        {
            return UnitOfWorkFactory.GetUnitOfWork(Config.DbName, DbSupportType.MicrosoftSqlServer);
        }
    }
}