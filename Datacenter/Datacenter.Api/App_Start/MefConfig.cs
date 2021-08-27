#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : MefConfig.cs
// Time Create : 1:38 PM 06/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition.Hosting;
using System.Web.Hosting;
using System.Web.Http;
using Datacenter.Api.Core;
using DataCenter.Core;
using Log;
using StarSg.Core;
using Datacenter.DataTranport;

#endregion

namespace Datacenter.Api
{
    public class MefConfig
    {
        private static CompositionContainer container;
        public static RealTimeLog RealLog { get; } = new RealTimeLog();

        public static void Register()
        {
            try
            {
                var path = HostingEnvironment.MapPath("~/bin");
                //var directoryCatalog = new DirectoryCatalog(path);
                var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path, "*.sgsi.dll"));
                container = new CompositionContainer(aggregateCatalog, true);
                var resolver = new MefDependencyResolver(container);
                //var c = container.GetExportedValues<ITaxiFilter>();
                //var attacks = container.GetExportedValues<IAttackLog>();

                var log = container.GetExportedValue<ILog>();
                log.InitLogConfig(System.Web.Configuration.WebConfigurationManager.AppSettings["LogLevel"]);

                var logFile = new LogFiles();
                logFile.SetPath(HostingEnvironment.MapPath("~"));
                log.InstallAttackLog(logFile);
                log.InstallAttackLog(RealLog);
                

                var module = container.GetExportedValues<IModuleFactory>();
                foreach (var factory in module)
                {
                    log.Success("SYSTEM", $"Khởi động thành công module {factory.GetType()}");
                }
                // Install MEF dependency resolver for MVC
                // DependencyResolver.SetResolver(resolver);
                // Install MEF dependency resolver for Web API

                var dataStore = container.GetExportedValue<IDataStore>();
                var loader = container.GetExportedValue<Loader>();
                dataStore.Reload(loader.Config.MotherSql.Id,  HostingEnvironment.MapPath("~/Data"), HostingEnvironment.MapPath("~/bin/Config"));

                var bgtTranport = container.GetExportedValue<IBgtTranport>();
                bgtTranport.setConfigPath(HostingEnvironment.MapPath("~/Data"),HostingEnvironment.MapPath("~/bin/Config"));

                GlobalConfiguration.Configuration.DependencyResolver = resolver;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}