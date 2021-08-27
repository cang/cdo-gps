#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: StarSg.PlanJob
// TIME CREATE : 10:09 PM 25/10/2016
// FILENAME: Plan.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.ComponentModel.Composition;
using Quartz;
using Quartz.Impl;

#endregion

namespace StarSg.PlanJob
{
    [Export(typeof (IPlan))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Plan : IPlan, IPartImportsSatisfiedNotification
    {
        private IScheduler _scheduler;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.Start();
        }

        #endregion

        #region Implementation of IPlan

        /// <summary>
        ///     cài đặt 1 công việc vào thời gian dc chỉ định
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool CreatePlan<T>(TimeSpan time) where T : IJob
        {
            var name = typeof (T).Name + $"{time.Hours}-{time.Minutes}";
            var trigger =
                TriggerBuilder.Create()
                    .WithIdentity(name)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(time.Hours, time.Minutes)).Build();

            var job = JobBuilder.Create<T>().WithIdentity(name, "g").Build();
            _scheduler.ScheduleJob(job, trigger);

            return true;
        }

        /// <summary>
        ///     thêm các đối tượng sử dụng cho các job khi phát sinh
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool PutObjectToContext(string key, object obj)
        {
            _scheduler.Context.Put(key, obj);
            return true;
        }

        #endregion
    }
}