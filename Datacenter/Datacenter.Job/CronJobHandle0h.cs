using System;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;
using Quartz;

namespace Datacenter.Job
{
    public class CronJobHandle0h : IJob
    {
        #region Implementation of IJob

        /// <summary>
        ///     Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
        ///     fires that is associated with the <see cref="T:Quartz.IJob" />.
        /// </summary>
        /// <remarks>
        ///     The implementation may wish to set a  result object on the
        ///     JobExecutionContext before this method exits.  The result itself
        ///     is meaningless to Quartz, but may be informative to
        ///     <see cref="T:Quartz.IJobListener" />s or
        ///     <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
        ///     execution.
        /// </remarks>
        /// <param name="context">The execution context.</param>
        public void Execute(IJobExecutionContext context)
        {
            // lấy các thông tin cần thiết
            var log = (ILog)context.Scheduler.Context.Get("log");
            var cache = (IDataStore)context.Scheduler.Context.Get("cache");
            var db = (ReponsitoryFactory)context.Scheduler.Context.Get("db");
            if (log == null || cache == null || db == null)
            {
                return; 
            }
            //nén data trước đó 1 ngày
            new ExecuteJob().Run(log, cache, db, DateTime.Now.AddDays(-1), () =>
            {
                log.Success("CronJobZipLog", $"Hoàn thành quá trình nén dữ liệu: {DateTime.Now.AddDays(-1)}");
            });
        }

        #endregion
    }
}