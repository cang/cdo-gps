//#region header
//// **********************************************************************
//// SOLUTION: StarSg
//// PROJECT: Datacenter.Scheduce
//// TIME CREATE : 10:03 PM 18/12/2016
//// FILENAME: TongHopBaoCao.cs
//// AUTHOR: Cang Do (dovancang@gmail.com)
//// -----------------------------------
//// Copyrights 2016  - All Rights Reserved.
//// **********************************************************************
//#endregion

//using System;
//using System.Linq;
//using Datacenter.Model.Entity;
//using Datacenter.Model.Log;
//using Datacenter.QueryRoute;
//using DataCenter.Core;
//using Log;

//namespace Datacenter.Scheduce.Logic
//{
//    public class TongHopBaoCao:IMiniJob
//    {
//        public void Handle(ReponsitoryFactory db, IDataStore cache, ILog log)
//        {
//            log.Debug("GeneralReportLogic", "Chạy tác vụ tạo báo cáo tổng hợp GeneralReportLogic");
//            // lấy toàn bộ xe ra
//            var allDevice = cache.GetQueryContext<Device>().GetAll();
//            //tạo dictionary danh sách device
//            var dictionayDevice = allDevice.GroupBy(m => m.CompanyId).ToDictionary(m => m.Key, m => m.ToList());
//            foreach (var listDevice in dictionayDevice)
//            {
//                var company = cache.GetCompanyById(listDevice.Key);
//                if (company != null && listDevice.Value.Count > 0)
//                {
//                    foreach (var device in listDevice.Value)
//                    {
//                        device.Temp?.Reset();
//                        using (var dataContext = db.CreateQuery())
//                        {
//                            if (device.Temp?.GeneralReportLog == null)
//                            {
//                                continue;
//                            }
//                            try
//                            {
//                                var tempReport = new GeneralReportLog
//                                {
//                                    DbId = device.Temp.GeneralReportLog.DbId,
//                                    GuidId = device.Temp.GeneralReportLog.GuidId,
//                                    Id = device.Temp.GeneralReportLog.Id,
//                                    CompanyId = device.Temp.GeneralReportLog.CompanyId,
//                                    UpdateTime = device.Temp.GeneralReportLog.UpdateTime,
//                                    OverTimeInday = device.Temp.GeneralReportLog.OverTimeInday,
//                                    KmOnDay = device.Temp.GeneralReportLog.KmOnDay,
//                                    KmCoOnDay = device.Temp.GeneralReportLog.KmCoOnDay,
//                                    PauseCount = device.Temp.GeneralReportLog.PauseCount,
//                                    InvalidOverTimeCount = device.Temp.GeneralReportLog.InvalidOverTimeCount,
//                                    InvalidSpeedCount = device.Temp.GeneralReportLog.InvalidSpeedCount,
//                                    OpenDoorCount = device.Temp.GeneralReportLog.OpenDoorCount,
//                                    OverTimeIndayCount = device.Temp.GeneralReportLog.OverTimeIndayCount,
//                                    NhienLieuTieuThu = device.Temp.GeneralReportLog.NhienLieuTieuThu,
//                                    NhienLieuDauNgay = device.Temp.GeneralReportLog.NhienLieuDauNgay
//                                };
//                                //cập nhật thông tin vào database
//                                var database =
//                                    dataContext.GetReponsitory().GetWhere<GeneralReportLog>(
//                                        m =>
//                                            m.GuidId == tempReport.GuidId &&
//                                            m.UpdateTime == tempReport.UpdateTime)
//                                        .FirstOrDefault();
//                                if (database == null)
//                                {
//                                    dataContext.GetReponsitory().Insert(tempReport);
//                                    dataContext.Commit();
//                                }
//                                else
//                                {
//                                    database.KmOnDay = tempReport.KmOnDay;
//                                    database.KmCoOnDay = tempReport.KmCoOnDay;
//                                    database.OpenDoorCount = tempReport.OpenDoorCount;
//                                    database.InvalidSpeedCount = tempReport.InvalidSpeedCount;
//                                    database.OverTimeInday = tempReport.OverTimeInday;
//                                    database.InvalidOverTimeCount = tempReport.InvalidOverTimeCount;
//                                    database.PauseCount = tempReport.PauseCount;
//                                    database.OverTimeIndayCount = tempReport.OverTimeInday / 600;
//                                    database.NhienLieuDauNgay = device.Temp.GeneralReportLog.NhienLieuDauNgay;
//                                    database.NhienLieuTieuThu = device.Temp.GeneralReportLog.NhienLieuTieuThu;
//                                    dataContext.GetReponsitory().Update(database, m => m.KmOnDay, m => m.OpenDoorCount,
//                                        m => m.InvalidOverTimeCount
//                                        , m => m.OverTimeInday, m => m.InvalidSpeedCount, m => m.PauseCount,
//                                        m => m.OverTimeIndayCount, m => m.NhienLieuDauNgay, m => m.NhienLieuTieuThu);
//                                    dataContext.GetReponsitory().Commit();
//                                }
//                            }
//                            catch (Exception e)
//                            {
//                                log.Exception("GeneralReportLogic", e,
//                                    $"Lỗi chạy tác vụ tạo báo cáo tổng hợp GeneralReportLogic: {device.Serial}");
//                            }
//                            //reset các thông số báo cáo
//                            //xung đột với tiến trình parser data từ device
//                            lock (device.Temp.GeneralReportLog)
//                            {
//                                device.Temp.GeneralReportLog = null;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}