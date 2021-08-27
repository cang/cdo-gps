using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;
using Log;
using Microsoft.AspNet.SignalR;
using Route.Api.Core;
using Route.Api.Hubs;
using Route.Core;
using StarSg.Core;
using Route.Api.Auth.Core;

namespace Route.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class MefLoader
    {
        private static string path = HostingEnvironment.MapPath("~/bin");
        public static RealTimeLog RealLog { get; } = new RealTimeLog();
        public static CompositionContainer Container { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public static void Register()
        {
            try
            {
                var aggregateCatalog = new AggregateCatalog();
                // var pluginsCatalog = new DirectoryCatalog(HostingEnvironment.MapPath("~/bin/Plugins"));
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path, "*.sgsi.dll"));
               

               
                var container = new CompositionContainer(aggregateCatalog);
                Container = container;
                var resolver = new MefDependencyResolver(container);
                //var c = container.GetExportedValues<ITaxiFilter>();
                //var attacks = container.GetExportedValues<IAttackLog>();

                var log = container.GetExportedValue<ILog>();
                log.InitLogConfig(System.Web.Configuration.WebConfigurationManager.AppSettings["LogLevel"]);

                var logFile = new LogFiles();
                logFile.SetPath(HostingEnvironment.MapPath("~"));
                log.InstallAttackLog(logFile);
                log.InstallAttackLog(RealLog);

                #region dành cho Auth : cài đặt thông tin database
                var acc = container.GetExportedValue<IAccountManager>();
                //var loader = container.GetExportedValue<Auth.Core.Loader>();
                acc.CreateStaticToken();
                #endregion  dành cho Auth : cài đặt thông tin database

                var models = container.GetExportedValues<IModuleFactory>();
                foreach (var moduleFactory in models)
                {
                    log.Success("SYSTEM", $"Load module :{moduleFactory.GetType()} thành công");
                }

                var t1 = Container.GetExportedValue<IDeviceRouteTableEvent>();
                t1.OnAdd += T1_OnAdd;
                t1.OnAdds += T1_OnAdds;
                t1.OnRemove += T1_OnRemove;

                //var t = Container.GetExportedValue<IDeviceRouteTableUpdate>();
                //t.Push(
                //    new DataCenterInfo
                //    {
                //        Id = Guid.NewGuid(),
                //        Ip = "127.0.0.1",
                //        NodeName = "ABC",
                //        Port = 80,
                //        ReportCount = 1
                //    }, 123456);
                // Install MEF dependency resolver for MVC
                // DependencyResolver.SetResolver(resolver);
                // Install MEF dependency resolver for Web API
                GlobalConfiguration.Configuration.DependencyResolver = resolver;

                acc.LoadBranch();
                acc.LoadSystemPair();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private static void T1_OnAdds(DataCenterInfo arg1, IList<long> arg2)
        {
            DeviceStatusHub.ClientManager.InsertOrUpdate(arg2);
        }

        private static void T1_OnRemove(DataCenterInfo arg1, long arg2)
        {
            DeviceStatusHub.ClientManager.RemoveSerial(arg2);
        }

        private static void T1_OnAdd(DataCenterInfo arg1, long arg2)
        {
            DeviceStatusHub.ClientManager.InsertOrUpdate(new List<long>() {arg2});
        }
    }
}