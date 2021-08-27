//#region header
//// /*********************************************************************************************/
//// Project :Datacenter.Scheduce
//// FileName : ZipDeviceLog.cs
//// Time Create : 8:43 AM 25/02/2017
//// Author:  Cang Do (dovancang@gmail.com)
//// /********************************************************************************************/
//#endregion

//using System;
//using System.Linq;
//using Core.Utils;
//using Datacenter.Model.Entity;
//using Datacenter.Model.Log;
//using Datacenter.Model.Log.ZipLog;
//using Datacenter.QueryRoute;
//using DataCenter.Core;
//using Log;
//using StarSg.Utils.Utils;

//namespace Datacenter.Scheduce.Logic
//{
//    public class ZipDeviceLogLogic:IMiniJob
//    {
//        #region Implementation of IMiniJob

//        public void Handle(ReponsitoryFactory dbFactory, IDataStore cache, ILog log)
//        {
//            var allDevice = cache.GetQueryContext<Device>().GetAll();
//            var dictionayDevice = allDevice.GroupBy(m => m.CompanyId).ToDictionary(m => m.Key, m => m.ToList());

//            // chuyển thời gian về ngày hôm qua , task này luôn chạy sau 0h nên dữ liệu phải đọc của ngày hôm trước
//            var now = DateTime.Now.AddDays(-1);
//            var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
//            var end = begin.AddDays(1); // cuối ngày
//            foreach (var listDevice in dictionayDevice)
//            {
//                var company = cache.GetCompanyById(listDevice.Key);
//                if (company != null && listDevice.Value.Count > 0)
//                {
//                    foreach (var device in listDevice.Value)
//                    {
//                        using (var dataContext = dbFactory.CreateQuery())
//                        {
//                            try
//                            {
//                                // đọc log trong 1 ngày ra
//                                var allLog =
//                                    dataContext.GetWhere<DeviceLog>(
//                                        m =>
//                                            m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend < end &&
//                                            m.Indentity == device.Indentity, 0);
//                                //check data nén
//                                if (allLog.Count == 0)
//                                    continue;

//                                // nén dữ liệu lại 
//                                //todo: do một số trường trong record ko cần thiết nên tạo thêm 1 lớp chuyển đổi nữa
//                                //var data = allLog.ObjectToByteArray().Zip();
//                                DeviceLogCollection sellog = new DeviceLogCollection(allLog);
//                                var data = sellog.Serializer().Zip();
//                                //DeviceLogCollection sellog1 = new DeviceLogCollection();
//                                //sellog1.Deserializer(data.UnZip());

//                                // lưu vào bảng mới
//                                dataContext
//                                    .Insert(new DeviceLogZip
//                                    {
//                                        Data = data,
//                                        Id = 0,
//                                        Serial = device.Serial,
//                                        TimeUpdate = now,
//                                        DbId = company.DbId,
//                                        CompanyId = company.Id,
//                                        GroupId = device.GroupId,
//                                        Indentity = device.Indentity
//                                    }, 0);
//                                dataContext.Commit();

//                                // xóa dữ liệu trong bảng cũ ra ( xóa trên dữ liệu chính)
//                                //foreach (var deviceRawLog in allLog)
//                                //{
//                                //    dataContext.GetReponsitory().Delete(deviceRawLog);
//                                //}
//                                //// commit dữ liệu
//                                //dataContext.Commit();

//                                dataContext.GetReponsitory().DeleteWhere<DeviceLog>(
//                                    o => o.Indentity == device.Indentity
//                                    && o.DeviceStatus.ClientSend >= begin
//                                    && o.DeviceStatus.ClientSend < end
//                                    );
//                                dataContext.Commit();

//                            }
//                            catch (Exception e)
//                            {
//                                log.Exception("ZipDeviceLogLogic", e, $"Lỗi nén data device: {device.Serial}");
//                            }

//                            return;
//                        }
//                    }
//                }
//            }
//        }
//    }

//        #endregion
    
//}