using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Datacenter.QueryRoute;
using Datacenter.Scheduce.Job;
using DataCenter.Core;
using Log;
using StarSg.Core;
using StarSg.PlanJob;

namespace Datacenter.Scheduce
{
    [Export(typeof(IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ScheduceFactory : IModuleFactory, IPartImportsSatisfiedNotification
    {
        [Import]
        private ILog _log;
        [Import]
        private IDataStore _dataStore;
        [Import]
        private ReponsitoryFactory _reponsitory;
        [Import]
        private IPlan _plan;
        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            //THÊM CÁC DỮ LIỆU LÀM VIỆC CHO JOB
            _plan.PutObjectToContext("log", _log);
            _plan.PutObjectToContext("cache", _dataStore);
            _plan.PutObjectToContext("db", _reponsitory);

            //test chạy task
            //Thread de=new Thread(Start);
            //de.Start();
            // CÀI ĐẶT JOB LÀM VIỆC
            //if (Config.ZipLog != null)
            {

                _plan.CreatePlan<MiniJob12h>(new TimeSpan(0, 0, 0));
                _log.Debug("Scheduce", "Khởi chạy plan làm việc ");
            }
        }

        //private void Start()
        //{
        //    Thread.Sleep(60000);
        //    var run = new DeviceRawLogLogic();
        //    var r = _reponsitory.CreateQuery();
        //    //run.Handle(r, _dataStore, _log);
        //    run.test(r, _dataStore, _log);
        //    r.Dispose();
        //}

        #endregion

    }
}
