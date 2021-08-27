#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Job
// TIME CREATE : 10:22 PM 25/10/2016
// FILENAME: ExecuteJobZip.cs
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
using Datacenter.Job.Logics;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;

namespace Datacenter.Job
{
    public class ExecuteJob
    {
        public void Run(ILog log, IDataStore cache, ReponsitoryFactory db, DateTime time, Action oncomplete)
        {
            log.Debug("CronJob", "Khởi dộng thành tiến trình 0h");
            //
            //return;
            // lấy danh sách logic xử lý
            var logicType = (from t in Assembly.GetExecutingAssembly().GetTypes()
                             where t.GetInterfaces().Contains(typeof(IZipLogic))
                             select t).ToList();
            // chạy logic xử lý tương ứng với từng bảng
            var allTask = new List<Task>();
            foreach (var logic in logicType.Select(Activator.CreateInstance).OfType<IZipLogic>())
            {
                allTask.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // tạo ra 1 kênh kết nối tới cơ sở dữ liệu cho logic
                        var r = db.CreateQuery();
                        logic.Handle(r, cache, log, time);
                        r.Dispose();
                        log.Success("CronJob", $"Hoàn thành task logic {logic.GetType().Name}");
                    }
                    catch (Exception e)
                    {
                        log.Exception("CronJob", e, $"Chạy logic {logic.GetType().Name} lỗi");
                    }
                }
                    ));

                log.Debug("CronJob", $"Khởi động logic {logic.GetType().Name} thành công");
            }

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (allTask.Count(task => task.IsCompleted) == allTask.Count)
                    {
                        oncomplete?.Invoke();
                        return;
                    }
                    Task.Delay(100);

                }
            });
        }
    }
}