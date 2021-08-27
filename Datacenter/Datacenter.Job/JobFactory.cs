#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Job
// TIME CREATE : 10:16 PM 25/10/2016
// FILENAME: JobFactory.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.ComponentModel.Composition;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;
using StarSg.Core;
using StarSg.PlanJob;

#endregion

namespace Datacenter.Job
{
    [Export(typeof (IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class JobFactory : IModuleFactory, IPartImportsSatisfiedNotification
    {

        [Import] private IDataStore _dataStore;

        [Import] private ILog _log;

        [Import] private IPlan _plan;

        [Import] private ReponsitoryFactory _reponsitory;

        public void OnImportsSatisfied()
        {
            _plan.PutObjectToContext("log", _log);
            _plan.PutObjectToContext("cache", _dataStore);
            _plan.PutObjectToContext("db", _reponsitory);
            _plan.CreatePlan<CronJobHandle0h>(new TimeSpan(0, 0, 0));
            _log.Debug("Zipdatafactory", "Khởi chạy plan làm việc ");
        }
    }
}