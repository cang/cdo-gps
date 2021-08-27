using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using ConfigFile;
using Route.Api.Models;
using StarSg.Core;
using System.Threading.Tasks;
using Log;

namespace Route.Api.Core
{
    [Export]
    [Export(typeof(IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Loader : IPartImportsSatisfiedNotification, IModuleFactory
    {
        //private const string Path = "Config/RequestAuthConfig.xml";
        //[Import] private IConfigManager _cfg;

        /// <summary>
        ///     Cấu hình load từ file
        /// </summary>
       // public RequestAuthConfig Config { get; private set; }

        public void OnImportsSatisfied()
        {
            try
            {
                //if (!Directory.Exists(HostingEnvironment.MapPath("~/bin/") + "Config"))
                //    Directory.CreateDirectory(HostingEnvironment.MapPath("~/bin/") + "Config");
                //Config = _cfg.Read<RequestAuthConfig>(HostingEnvironment.MapPath("~/bin/") + Path);

                Task.Factory.StartNew(ProcessCronJob);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw new Exception("", e);
            }
        }


        private DateTime LastMidnight;
        private async Task ProcessCronJob()
        {
            LastMidnight = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            //waithandleProcessCronJob = new EventWaitHandle(false, EventResetMode.AutoReset);
            while (true)
            {
                ProcessCronJobAtMidnight();
                await Task.Delay(1000*60);
            }
        }

        /// <summary>
        /// Process cron job at mid night
        /// </summary>
        private void ProcessCronJobAtMidnight()
        {
            if ((DateTime.Now - LastMidnight).TotalDays < 1) return;
            try
            {
                StatisticMemoryLog mlog = MefLoader.Container.GetExportedValue<StatisticMemoryLog>();
                mlog.Reset();
            }
            catch //(Exception e)
            {
                //_log.Exception("CRONJOB", e, "ProcessCronJobAtMidnight");
            }
            LastMidnight = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }


    }
}