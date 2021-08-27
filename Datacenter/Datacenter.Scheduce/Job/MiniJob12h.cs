#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Scheduce
// TIME CREATE : 9:29 PM 18/12/2016
// FILENAME: MiniJob12h.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Datacenter.QueryRoute;
using Datacenter.Scheduce.Logic;
using DataCenter.Core;
using Log;
using Quartz;

namespace Datacenter.Scheduce.Job
{
    public class MiniJob12h: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var log = (ILog)context.Scheduler.Context.Get("log");
            var cache = (IDataStore)context.Scheduler.Context.Get("cache");
            var db = (ReponsitoryFactory)context.Scheduler.Context.Get("db");
            if (log == null || cache == null || db == null)
            {
                return; //todo: làm ji để admin biết job ko chạy dc ???
            }

            var logicType = (from t in Assembly.GetExecutingAssembly().GetTypes()
                             where t.GetInterfaces().Contains(typeof(IMiniJob))
                             select t).ToList();
            foreach (var logic in logicType.Select(Activator.CreateInstance).OfType<IMiniJob>())
            {
                try
                {
                    logic.Handle(db, cache, log);
                }
                catch (Exception e)
                {
                    log.Exception("MiniJob12h", e, logic.GetType().ToString());
                }
            }

        }
    }



}