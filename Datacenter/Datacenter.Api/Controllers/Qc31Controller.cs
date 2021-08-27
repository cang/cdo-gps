#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : Qc31Controller.cs
// Time Create : 11:06 AM 28/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using Datacenter.Api.Core;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Log.ZipLog;
using DataCenter.Core;
using StarSg.Utils.Geos;
using StarSg.Utils.Models.DatacenterResponse.Qc31;
using StarSg.Utils.Models.Tranfer;
using StarSg.Utils.Models.Tranfer.Qc31;

using DaoDatabase;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     báo cáo quy chuẩn 31
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Qc31Controller : BaseController
    {
        /// <summary>
        ///     lấy thông tin hành trình xe chạy
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceTripLogGet GetTripLog(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceTripLogGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceTripLogGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };
            var result = new DeviceTripLogGet();
            result.Datas = new List<DeviceTripLogTranfer>();
            var nowLog =
                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end)
                    .Take(10000).Execute();
            var oldLog = GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);
            var allData = new List<DeviceTraceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData.OrderBy(m => m.BeginTime))
            {
                result.Datas.Add(new DeviceTripLogTranfer
                {
                    TimeUpdate = log.BeginTime,
                    Location = new GpsPoint
                    {
                        Lat = log.BeginLocation.Lat,
                        Lng = log.BeginLocation.Lng,
                        Address = log.BeginLocation.Address
                    },
                    Bs = device.Bs,
                    Serial = device.Serial,
                    Type = (int)log.Type
                });
                if (log.Type != TraceType.None && log.Type != TraceType.Stop15 && log.Type != TraceType.Run5)
                    result.Datas.Add(new DeviceTripLogTranfer
                    {
                        TimeUpdate = log.EndTime,
                        Location = new GpsPoint
                        {
                            Lat = log.EndLocation.Lat,
                            Lng = log.EndLocation.Lng,
                            Address = log.EndLocation.Address
                        },
                        Bs = device.Bs,
                        Serial = device.Serial,
                        Type = (int)log.Type + 1000
                    });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }


        //[Import]
        //private ILocationQuery _locationQuery;
        ///// <summary>
        /////     lấy thông tin hành trình xe chạy
        ///// </summary>
        ///// <param name="serial"></param>
        ///// <param name="begin"></param>
        ///// <param name="end"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public DeviceTripLogGet GetTripLog(long serial, DateTime begin, DateTime end)
        //{
        //    var device = Cache.GetQueryContext<Device>().GetByKey(serial);
        //    if (device == null)
        //        return new DeviceTripLogGet { Description = "Không tồn tại thiết bị này" };
        //    var company = Cache.GetCompanyById(device.CompanyId);
        //    if (company == null)
        //        return new DeviceTripLogGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };
        //    var result = new DeviceTripLogGet();
        //    result.Datas = new List<DeviceTripLogTranfer>();
        //    var nowLog =
        //        DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
        //            .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end)
        //            .Take(10000).Execute();
        //    var oldLog = GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);
        //    var allData = new List<DeviceTraceLog>();
        //    allData.AddRange(oldLog);
        //    allData.AddRange(nowLog);

        //    //get from devicelog
        //    var nowDeviceLog =
        //        DataContext.CreateQuery<DeviceLog>(company.DbId)
        //        .Where(m => m.Indentity == device.Indentity && m.DeviceStatus.ClientSend >= begin
        //        && m.DeviceStatus.ClientSend <= end)
        //        .Execute();
        //    var oldDeviceLog = GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);
        //    List<DeviceLog> devlog = new List<DeviceLog>(oldDeviceLog);
        //    devlog.AddRange(nowDeviceLog);

        //    GpsLocation lastloc = null;
        //    bool lastMachine = false;
        //    bool lastAirMachine = false;
        //    bool lastDoor = false;
        //    bool lastRun = false;
        //    long lastDriver = 0;

        //    foreach (var obj in devlog)
        //    {
        //        if (obj.DeviceStatus == null) continue;

        //        String note = "";

        //        if (obj.DeviceStatus.Machine != lastMachine)
        //        {
        //            note += ", " + (obj.DeviceStatus.Machine ? "mở máy" : "tắt máy");
        //            lastMachine = obj.DeviceStatus.Machine;
        //        }

        //        if (obj.DeviceStatus.AirMachine != lastAirMachine)
        //        {
        //            note += ", " + (obj.DeviceStatus.AirMachine ? "mở máy lạnh" : "tắt máy lạnh");
        //            lastAirMachine = obj.DeviceStatus.AirMachine;
        //        }

        //        if (obj.DeviceStatus.Door != lastDoor)
        //        {
        //            note += ", " + (obj.DeviceStatus.Door ? "mở cửa" : "đóng cửa");
        //            lastDoor = obj.DeviceStatus.Door;
        //        }

        //        if (lastDriver > 0 && obj.DriverStatus != null && obj.DriverStatus.DriverId != lastDriver)
        //        {
        //            note += ", đổi tài";
        //            lastDriver = obj.DriverStatus.DriverId;
        //        }

        //        bool isrun = obj.DeviceStatus.Speed >= 5;
        //        if (isrun != lastRun)
        //        {
        //            note += ", " + (isrun ? "đang chạy tốc độ " + obj.DeviceStatus.Speed : "đang dừng");
        //            lastRun = isrun;
        //        }

        //        if (!String.IsNullOrEmpty(note)) note = note.Substring(2);
        //        else
        //        {
        //            if (lastRun)
        //                note = "xe đang chạy";
        //            else
        //                note = "xe đang dừng";
        //        }

        //        var traceobj =
        //        new DeviceTraceLog()
        //        {
        //            //CompanyId = obj.CompanyId,
        //            //GroupId= obj.GroupId,
        //            //DbId = obj.DbId,
        //            //Serial = obj.Serial,
        //            Type = TraceType.None,
        //            //Id = obj.Id,
        //            //Indentity= obj.Indentity,
        //            //DriverId = obj?.DriverStatus?.DriverId ?? 0,
        //            Distance = 0,
        //            BeginTime = obj.DeviceStatus.ClientSend,
        //            BeginLocation = obj.DeviceStatus.GpsInfo,
        //            Note = note
        //        };

        //        if (String.IsNullOrWhiteSpace(traceobj.BeginLocation.Address))
        //        {
        //            if (lastloc == null || GeoUtil.Distance(lastloc.Lat, lastloc.Lng, traceobj.BeginLocation.Lat, traceobj.BeginLocation.Lng) > 5)
        //            {
        //                _locationQuery.GetAddress(traceobj.BeginLocation);
        //                if (!String.IsNullOrWhiteSpace(traceobj.BeginLocation.Address))
        //                    lastloc = traceobj.BeginLocation;
        //                else //lấy của trên
        //                {
        //                    int idx = lastloc.Address.IndexOf(" ");
        //                    if (idx >= 0)
        //                        traceobj.BeginLocation.Address = lastloc.Address.Substring(idx);
        //                }
        //            }
        //            else if (lastloc != null)
        //                traceobj.BeginLocation.Address = lastloc.Address;
        //        }

        //        allData.Add(traceobj);
        //    }

        //    var ret = new List<DeviceTripLogTranfer>();

        //    //foreach (var log in allData.OrderBy(m => m.BeginTime))
        //    foreach (var log in allData)
        //    {
        //        ret.Add(new DeviceTripLogTranfer
        //        {
        //            TimeUpdate = log.BeginTime,
        //            Location = new GpsPoint
        //            {
        //                Lat = log.BeginLocation.Lat,
        //                Lng = log.BeginLocation.Lng,
        //                Address = log.BeginLocation.Address
        //            },
        //            Bs = device.Bs,
        //            Serial = device.Serial,
        //            Type = (int)log.Type,
        //            Note = log.Note
        //        });

        //        if (log.Type != TraceType.None && log.Type != TraceType.Stop15 && log.Type != TraceType.Run5)
        //            ret.Add(new DeviceTripLogTranfer
        //            {
        //                TimeUpdate = log.EndTime,
        //                Location = new GpsPoint
        //                {
        //                    Lat = log.EndLocation.Lat,
        //                    Lng = log.EndLocation.Lng,
        //                    Address = log.EndLocation.Address
        //                },
        //                Bs = device.Bs,
        //                Serial = device.Serial,
        //                Type = (int)log.Type + 1000
        //            });
        //    }

        //    result.Datas = ret.OrderBy(m => m.TimeUpdate).ToList();
        //    result.Status = 1;
        //    result.Description = "OK";
        //    return result;
        //}



        /// <summary>
        ///     lấy thông tin vận tốc
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceSpeedTraceLogGet GetSpeedTrace(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceSpeedTraceLogGet {Description = "Không tồn tại thiết bị này"};
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceSpeedTraceLogGet {Description = "Thiết bị không chứa trong 1 công ty nào cả"};

            var result = new DeviceSpeedTraceLogGet();
            result.Datas = new List<DeviceSpeedLogTranfer>();
            var tmp = new DeviceLog();
            var nowLog =
                DataContext.CreateQuery<DeviceLog>(company.DbId)
                    .Where(
                        m =>
                            m.Indentity == device.Indentity && m.DeviceStatus.ClientSend >= begin &&
                            m.DeviceStatus.ClientSend <= end)
                    .Take(10000)
                    //.Select(new Expression<Func<DeviceLog, object>>[]
                    //{
                    //    m => m.DeviceStatus.SpeedTrace,
                    //    m => m.DeviceStatus.ClientSend
                    //}, new Expression<Func<object>>[]
                    //{
                    //    () => tmp.DeviceStatus.SpeedTrace,
                    //    () => tmp.DeviceStatus.ClientSend
                    //})
                    .Execute();
            var oldLog = GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);
            var allData = new List<DeviceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData.OrderBy(m => m.DeviceStatus.ClientSend))
            {
                result.Datas.Add(new DeviceSpeedLogTranfer
                {
                    Bs = device.Bs,
                    ClientSend = log.DeviceStatus.ClientSend,
                    SpeedTrace = log.DeviceStatus.SpeedTrace
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin quá vận tốc theo xe
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceOverSpeedLogGet GetOverSpeedLogByDevice(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceOverSpeedLogGet {Description = "Không tồn tại thiết bị này"};
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceOverSpeedLogGet {Description = "Thiết bị không chứa trong 1 công ty nào cả"};
            var result = new DeviceOverSpeedLogGet();
            result.Datas = new List<OverSpeedLogTranfer>();
            var nowLog =
                DataContext.CreateQuery<DeviceOverSpeedLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end)
                    .Take(10000).Execute();
            var oldLog = GetZipLog<DeviceOverSpeedLog, DeviceOverSpeedLogZip>(company.DbId, m => m.Indentity == device.Indentity,
                begin, end);
            var allData = new List<DeviceOverSpeedLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData)
            {
                var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);
                result.Datas.Add(new OverSpeedLogTranfer
                {
                    BeginTime = log.BeginTime,
                    Bs = device.Bs,
                    CompanyId = company.Id,
                    DriverId = log.DriverId,
                    DriverName = dr?.Name ?? "",
                    EndTime = log.EndTime,
                    Gplx = dr?.Gplx ?? "",
                    GroupId = device.GroupId,
                    Id = log.Id,
                    LimitSpeed = log.LimitSpeed,
                    Serial = log.Serial,
                    MaxSpeed = log.MaxSpeed,
                    Point = new GpsPoint
                    {
                        Lat = log.Point.Lat,
                        Lng = log.Point.Lng,
                        Address = log.Point.Address
                    },
                    Type = (int) device.ActivityType
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin quá vận tốc theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceOverSpeedLogGet GetOverSpeedLogByCompnay(long companyId, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null)
                return new DeviceOverSpeedLogGet {Description = "Thiết bị không chứa trong 1 công ty nào cả"};

            var result = new DeviceOverSpeedLogGet();
            result.Datas = new List<OverSpeedLogTranfer>();
            var nowLog =
                DataContext.CreateQuery<DeviceOverSpeedLog>(company.DbId)
                    .Where(m => m.CompanyId == companyId && m.BeginTime >= begin && m.EndTime <= end)
                    .Take(10000).Execute();
            var oldLog = GetZipLog<DeviceOverSpeedLog, DeviceOverSpeedLogZip>(company.DbId, m => m.CompanyId == companyId,
                begin, end);
            var allData = new List<DeviceOverSpeedLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData)
            {
                var device =
                    Cache.GetQueryContext<Device>().GetWhere(m => m.Indentity == log.Indentity).FirstOrDefault();
                if (device == null) continue;
                var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);
                result.Datas.Add(new OverSpeedLogTranfer
                {
                    BeginTime = log.BeginTime,
                    Bs = device.Bs,
                    CompanyId = company.Id,
                    DriverId = log.DriverId,
                    DriverName = dr?.Name ?? "",
                    EndTime = log.EndTime,
                    Gplx = dr?.Gplx ?? "",
                    GroupId = device.GroupId,
                    Id = log.Id,
                    LimitSpeed = log.LimitSpeed,
                    Serial = log.Serial,
                    MaxSpeed = log.MaxSpeed,
                    Point = new GpsPoint
                    {
                        Lat = log.Point.Lat,
                        Lng = log.Point.Lng,
                        Address = log.Point.Address
                    },
                    Type = (int) device.ActivityType
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin dừng dỗ theo serial
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DevicePauseLogGet GetPauseLog(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DevicePauseLogGet {Description = "Không tồn tại thiết bị này"};
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DevicePauseLogGet {Description = "Thiết bị không chứa trong 1 công ty nào cả"};

            var result = new DevicePauseLogGet();
            result.Datas = new List<DevicePauseLogTranfer>();
            var nowLog =
                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
                    .Where(
                        m =>
                            m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end &&
                            m.Type == TraceType.Stop)
                    .Take(10000).Execute();
            var oldLog =
                GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
                    .Where(m => m.Type == TraceType.Stop);
            var allData = new List<DeviceTraceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData.OrderBy(m => m.BeginTime))
            {
                var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);
                result.Datas.Add(new DevicePauseLogTranfer
                {
                    ActivityType = (int) device.ActivityType,
                    BeginTime = log.BeginTime,
                    Bs = device.Bs,
                    DriverId = log.DriverId,
                    DriverName = dr?.Name ?? "",
                    EndTime = log.EndTime,
                    Gplx = dr?.Gplx ?? "",
                    Id = log.Id,
                    PauseTime = log.EndTime - log.BeginTime,
                    Point = new GpsPoint
                    {
                        Lat = log.BeginLocation.Lat,
                        Lng = log.BeginLocation.Lng,
                        Address = log.BeginLocation.Address
                    },
                    Serial = device.Serial
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin thời gian lái xe liên tục
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverTime31LogGet GetDriverTime(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DriverTime31LogGet {Description = "Không tồn tại thiết bị này"};
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DriverTime31LogGet {Description = "Thiết bị không chứa trong 1 công ty nào cả"};

            var result = new DriverTime31LogGet();
            result.Datas = new List<DriverTime31Tranfer>();

            //var nowLog =
            //    DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
            //        .Where(
            //            m =>
            //                m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end &&
            //                m.Type == TraceType.Machine)
            //        .Take(10000).Execute();
            //var oldLog =
            //    GetZipLog<DeviceTraceLog, DeviceTraceZip>(m => m.Indentity == device.Indentity, begin, end)
            //        .Where(m => m.Type == TraceType.Machine);

            
            //XAI CAI NAY BI LOI TU NHIEN XUAT HIEN THEM 1 DONG DU LIEU THUA
            //var nowLog =
            //    DataContext.CreateQuery<DriverTraceSessionLog>(company.DbId)
            //        .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end)
            //        .Execute();

            //FIX LOI CUA NHibernate
            var nowLog = new List<DriverTraceSessionLog>();
            String SQL = $@"SELECT * FROM DriverTraceSessionLog WHERE Indentity = '{device.Indentity}' AND BeginTime >= '{begin}' AND EndTime <= '{end}'";
            DataContext.CustomHandle<NHibernate.ISession>(m => {
                try
                {
                    NHibernate.ISQLQuery query = m.CreateSQLQuery(SQL);
                    IList<dynamic> list = query.DynamicList();
                    if (list != null)
                    {
                        foreach (var obj in list)
                        {
                            DriverTraceSessionLog acc = new DriverTraceSessionLog();
                            acc.Id = obj.Id;
                            acc.DriverId = obj.DriverId;
                            acc.BeginTime = obj.BeginTime;
                            acc.EndTime = obj.EndTime;

                            if (!Object.ReferenceEquals(null, obj.OverTime))
                                acc.OverTime = TimeSpan.FromTicks((long)obj.OverTime);

                            acc.Distance = obj.Distance;
                            acc.DbId = obj.DbId;

                            acc.BeginLocation = new GpsLocation();
                            acc.BeginLocation.Lat = obj.BeginLocation_Lat;
                            acc.BeginLocation.Lng = obj.BeginLocation_Lng;
                            if (!Object.ReferenceEquals(null, obj.BeginLocation_Address))
                                acc.BeginLocation.Address = obj.BeginLocation_Address;

                            acc.EndLocation = new GpsLocation();
                            acc.EndLocation.Lat = obj.EndLocation_Lat1;
                            acc.EndLocation.Lng = obj.EndLocation_Lng1;
                            if (!Object.ReferenceEquals(null, obj.EndLocation_Address1))
                                acc.EndLocation.Address = obj.EndLocation_Address1;

                            acc.Serial = obj.Serial;
                            acc.Indentity = obj.Indentity;
                            acc.CompanyId = obj.CompanyId;

                            nowLog.Add(acc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("DriverTraceSessionLogSQL", ex, SQL);
                }
            }, company.DbId);


            Log.Info("Qc31Controller", $"GetDriverTime {serial} {device.Indentity} {begin} {end} SQL {SQL} COUNT {nowLog?.Count}");

            //var allData = new List<DeviceTraceLog>();
            //allData.AddRange(oldLog);
            //allData.AddRange(nowLog);
            foreach (var log in nowLog.OrderBy(m => m.BeginTime))
            {
                var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);

                //fix loi item.OverTime =0 khi test tren may test cho bo kiem tra
                if (log.OverTime.TotalMinutes <= 0)
                    log.OverTime = log.EndTime - log.BeginTime;

                result.Datas.Add(new DriverTime31Tranfer
                {
                    ActivityType = (int) device.ActivityType,
                    BeginTime = log.BeginTime,
                    Bs = device.Bs,
                    Name = dr?.Name ?? "",
                    EndTime = log.EndTime,
                    Gplx = dr?.Gplx ?? "",
                    Over = log.OverTime.ToString(),
                    BeginLocation = new GpsPoint
                    {
                        Lat = log.BeginLocation.Lat,
                        Lng = log.BeginLocation.Lng,
                        Address = log.BeginLocation.Address
                    },
                    EndLocation = new GpsPoint
                    {
                        Lat = log.EndLocation.Lat,
                        Lng = log.EndLocation.Lng,
                        Address = log.EndLocation.Address
                    }
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp theo serial thiết bị
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDeviceReportGet GetAllDeviceReportBySerial(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new AllDeviceReportGet {Description = "Không tồn tại thiết bị này"};
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new AllDeviceReportGet {Description = "Thiết bị không chứa trong 1 công ty nào cả"};

            var result = new AllDeviceReportGet();
            result.Datas = new List<GeneralReport>();

            /*Lấy thông tin quá vận tốc*/
            var overSpeedLognow =
                DataContext.CreateQuery<DeviceOverSpeedLog>(company.DbId)
                    .Where(
                        m =>
                            m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end)
                    .Take(10000).Execute();
            //var overSpeedLognowold =
            //    GetZipLog<DeviceOverSpeedLog, DeviceOverSpeedLogZip>(m => m.Indentity == device.Indentity, begin, end);
            var allOverSpeed = new List<DeviceOverSpeedLog>();
            allOverSpeed.AddRange(overSpeedLognow);
            //allOverSpeed.AddRange(overSpeedLognowold);

            /*Lấy thông tin số lần dừng dỗ*/

            var paucount = DataContext.CreateQuery<DeviceTraceLog>(company.DbId).Where(m =>
                m.Indentity == device.Indentity && m.BeginTime >= begin && m.EndTime <= end &&
                m.Type == TraceType.Stop).Count();

            paucount +=
                GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
                    .Count(m => m.Type == TraceType.Stop);

            /*lấy tổng số km*/

            var tmpDistance = DataContext.CreateQuery<DeviceLog>(company.DbId)
                .WhereMinMax<Guid, long>(m => m.DeviceStatus.TotalGpsDistance, m => m.DeviceStatus.TotalGpsDistance
                    , m =>
                        m.Indentity == device.Indentity && m.DeviceStatus.ClientSend >= begin &&
                        m.DeviceStatus.ClientSend <= end,
                    m => m.Indentity).FirstOrDefault();

            var tmpOldDistance = GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m =>
                m.Indentity == device.Indentity, begin, end).OrderBy(m => m.DeviceStatus.ClientSend).ToList();

            result.Datas.Add(BuildAllReportByDevice(device, tmpDistance, tmpOldDistance, allOverSpeed, paucount));

            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="device"></param>
        /// <param name="tmpDistance"></param>
        /// <param name="tmpOldDistance"></param>
        /// <param name="allOverSpeed"></param>
        /// <param name="pausecount"></param>
        /// <returns></returns>
        private GeneralReport BuildAllReportByDevice(Device device, Tuple<Guid, long, long> tmpDistance,
            IList<DeviceLog> tmpOldDistance, IList<DeviceOverSpeedLog> allOverSpeed, int pausecount)
        {
            var distance = tmpDistance == null ? 1L : tmpDistance.Item3 - tmpDistance.Item2;
            distance += Math.Abs( (tmpOldDistance.FirstOrDefault()?.DeviceStatus.TotalGpsDistance ?? 0) -
                                 (tmpOldDistance.LastOrDefault()?.DeviceStatus.TotalGpsDistance ?? 0) );

            if (distance == 0) distance = 1;

            var over5to10 =
                allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 5 && m.MaxSpeed < m.LimitSpeed + 10).ToList();
            var over10to20 =
                allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 10 && m.MaxSpeed < m.LimitSpeed + 20).ToList();
            var over20to35 =
                allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 20 && m.MaxSpeed < m.LimitSpeed + 35).ToList();
            var over35 = allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 35).ToList();
            return new GeneralReport
            {
                ActivityType = (int) device.ActivityType,
                Bs = device.Bs,
                Distance = Math.Round(distance/1000.0, 2),
                Note = "",
                PauseSum = pausecount,
                Speed10To20 = over10to20.Count,
                Speed5To10 = over5to10.Count,
                Speed20To35 = over20to35.Count,
                Speed35 = over35.Count,
                Speed5To10Percent = Math.Round(over5to10.Sum(m => m.Distance) * 100.0 / distance, 2),
                Speed10To20Percent = Math.Round(over10to20.Sum(m => m.Distance) * 100.0 / distance, 2),
                Speed20To35Percent = Math.Round(over20to35.Sum(m => m.Distance) * 100.0 / distance, 2),
                Speed35Percent = Math.Round(over35.Sum(m => m.Distance) * 100.0 / distance, 2)
            };
        }

        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp của 1 nhóm xe trong công ty
        /// </summary>
        /// <param name="list"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDeviceReportGet GetAllDeviceReportBySerials(string list, DateTime begin, DateTime end)
        {
            var allSeial = new List<long>();
            try
            {
                allSeial =
                    list.Split('|').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
            }
            catch (Exception)
            {
                return new AllDeviceReportGet {Description = "Thông tin serial truyền lên không hợp lệ"};
            }
            if (allSeial.Count == 0) return new AllDeviceReportGet {Description = "Danh sách serial rỗng"};
            var tmpDevice = Cache.GetQueryContext<Device>().GetByKey(allSeial.First());
            if (tmpDevice == null)
                return new AllDeviceReportGet {Description = "Danh sách có 1 serial không hợp lệ (ko tồn tại)"};
            var company = Cache.GetCompanyById(tmpDevice.CompanyId);
            if (company == null) return new AllDeviceReportGet {Description = "Công ty chưa thiết bị không tồn tại"};

            var allDevice =
                allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();
            /*Lấy thông tin quá vận tốc*/
            var overSpeedLognow =
                DataContext.CreateQuery<DeviceOverSpeedLog>(company.DbId)
                    .Where(
                        m => m.BeginTime >= begin && m.EndTime <= end)
                    .WhereOr(
                        allDevice.Select(
                            m => (Expression<Func<DeviceOverSpeedLog, bool>>) (x => x.Indentity == m.Indentity))
                            .ToArray())
                    .Take(20000).Execute();
            //var overSpeedLognowold =
            //    GetZipLogWhereOr<DeviceOverSpeedLog, DeviceOverSpeedLogZip>(allDevice.Select(
            //                m => (Expression<Func<DeviceOverSpeedLogZip, bool>>)(x => x.Indentity == m.Indentity))
            //                .ToArray(), begin, end);
            var allOverSpeed = new List<DeviceOverSpeedLog>();
            allOverSpeed.AddRange(overSpeedLognow);
            //allOverSpeed.AddRange(overSpeedLognowold);

            /*lấy tổng số km*/

            var tmpDistance = DataContext.CreateQuery<DeviceLog>(company.DbId)
                .Where(m => m.DeviceStatus.ClientSend >= begin &&
                            m.DeviceStatus.ClientSend <= end)
                .WhereMinMax<Guid, long>(m => m.DeviceStatus.TotalGpsDistance, m => m.DeviceStatus.TotalGpsDistance
                    , BuildWhereOr(allDevice.Select(
                        m => (Expression<Func<DeviceLog, bool>>) (x => x.Indentity == m.Indentity))
                        .ToArray()),
                    m => m.Indentity);

            var tmpOldDistance = GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, BuildWhereOr(allDevice.Select(
                m => (Expression<Func<DeviceLogZip, bool>>) (x => x.Indentity == m.Indentity))
                .ToArray()), begin, end).OrderBy(m => m.DeviceStatus.ClientSend);

            /*Lấy thông tin dừng đỗ*/
            var allPause = new List<DeviceTraceLog>();
            allPause.AddRange(
                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
                    .Where(m => m.BeginTime >= begin && m.EndTime <= end &&
                                m.Type == TraceType.Stop).Where(BuildWhereOr(allDevice.Select(
                                    m => (Expression<Func<DeviceTraceLog, bool>>) (x => x.Indentity == m.Indentity))
                                    .ToArray())).Execute());

            allPause.AddRange(
                GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, BuildWhereOr(allDevice.Select(
                    m => (Expression<Func<DeviceTraceZip, bool>>) (x => x.Indentity == m.Indentity))
                    .ToArray()), begin, end)
                    .Where(m => m.Type == TraceType.Stop));
            // build result
            var result = new AllDeviceReportGet();
            result.Datas = new List<GeneralReport>();

            foreach (var device in allDevice)
            {
                result.Datas.Add(BuildAllReportByDevice(device,
                    tmpDistance.FirstOrDefault(m => m.Item1 == device.Indentity),
                    tmpOldDistance.Where(m => m.Indentity == device.Indentity).ToList()
                    , allOverSpeed.Where(m => m.Indentity == device.Indentity).ToList(),
                    allPause.Count(m => m.Indentity == device.Indentity)));
            }

            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin báo cáo theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDeviceReportGet GetAllDeviceReportByCompany(long companyId, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new AllDeviceReportGet {Description = "Không tìm thấy công ty"};
            var serial = "";
            var devices = Cache.GetQueryContext<Device>().GetByCompany(companyId);
            foreach (var device in devices)
            {
                if (serial != "")
                    serial += "|";
                serial += device.Serial;
            }

            if (serial == "") return new AllDeviceReportGet {Description = "Công ty không có xe"};
            return GetAllDeviceReportBySerials(serial, begin, end);
        }

        /// <summary>
        ///     lấy thông tin báo cáo tổng hợp  theo nhóm xe
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDeviceReportGet GetAllDeviceReportByGroupId(long groupId, DateTime begin, DateTime end)
        {
            var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(groupId);
            if (group == null) return new AllDeviceReportGet {Description = "Đội xe không tồn tại"};
            var serial = "";
            var devices = Cache.GetQueryContext<Device>().GetByGroup(group.CompnayId, groupId);
            foreach (var device in devices)
            {
                if (serial != "")
                    serial += "|";
                serial += device.Serial;
            }

            if (serial == "") return new AllDeviceReportGet {Description = "Đội xe không có xe"};
            return GetAllDeviceReportBySerials(serial, begin, end);
        }

        /// <summary>
        ///     Lấy thông tin báo cáo thổng hợp theo tài xế
        /// </summary>
        /// <param name="id"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDriverReportGet GetAllDriverReportById(long id, DateTime begin, DateTime end)
        {
            var driver = Cache.GetQueryContext<Driver>().GetByKey(id);
            if (driver == null)
                return new AllDriverReportGet {Description = "Không tồn tại lái xe này"};
            var company = Cache.GetCompanyById(driver.CompanyId);
            if (company == null)
                return new AllDriverReportGet {Description = "Lái xe không chứa trong 1 công ty nào cả"};

            var result = new AllDriverReportGet();
            result.Datas = new List<Driver31Report>();

            /*Lấy thông tin quá vận tốc*/
            var overSpeedLog =
                DataContext.CreateQuery<DeviceOverSpeedLog>(company.DbId)
                    .Where(
                        m =>
                            m.DriverId == id && m.BeginTime >= begin && m.EndTime <= end)
                    .Take(10000).Execute();


            /*lấy tổng số km*/
            var allTrace = new List<DriverTraceSessionLog>();
            var allTraceNow = DataContext.CreateQuery<DriverTraceSessionLog>(company.DbId).Where(
                m =>
                    m.DriverId == id && m.BeginTime >= begin && m.EndTime <= end)
                .Take(10000).Execute();
            var allTraceOld = GetZipLog<DriverTraceSessionLog, DriverTraceSessionLogZip>(company.DbId, m => m.DriverId == driver.Id,
                begin, end);
            allTrace.AddRange(allTraceNow);
            allTrace.AddRange(allTraceOld);

            result.Datas.Add(BuildAllReportByDriver(driver, overSpeedLog, allTrace));

            result.Status = 1;
            result.Description = "OK";
            return result;
        }


        /// <summary>
        ///     Lấy thông tin báo cáo theo 1 nhóm lái xe cùng công ty
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDriverReportGet GetAllDriverReportByIds(string ids, DateTime begin, DateTime end)
        {
            var allId = new List<long>();
            try
            {
                allId =
                    ids.Split('|').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
            }
            catch (Exception)
            {
                return new AllDriverReportGet {Description = "Thông tin id truyền lên không hợp lệ"};
            }
            if (allId.Count == 0) return new AllDriverReportGet {Description = "Danh sách serial rỗng"};

            var driver = Cache.GetQueryContext<Driver>().GetByKey(allId.First());
            if (driver == null)
                return new AllDriverReportGet {Description = "Không tồn tại lái xe này"};
            var company = Cache.GetCompanyById(driver.CompanyId);
            if (company == null)
                return new AllDriverReportGet {Description = "Lái xe không chứa trong 1 công ty nào cả"};

            var allDriver =
                allId.Select(m => Cache.GetQueryContext<Driver>().GetByKey(m)).Where(m => m != null).ToList();

            var result = new AllDriverReportGet();
            result.Datas = new List<Driver31Report>();

            /*Lấy thông tin quá vận tốc*/
            var overSpeedLog =
                DataContext.CreateQuery<DeviceOverSpeedLog>(company.DbId)
                    .Where(
                        m => m.BeginTime >= begin && m.EndTime <= end)
                    .WhereOr(
                        allDriver.Select(m => (Expression<Func<DeviceOverSpeedLog, bool>>) (x => x.DriverId == m.Id))
                            .ToArray())
                    .Take(100000).Execute();


            /*lấy tổng số km*/
            var allTrace = new List<DriverTraceSessionLog>();
            var allTraceNow = DataContext.CreateQuery<DriverTraceSessionLog>(company.DbId).Where(
                m => m.BeginTime >= begin && m.EndTime <= end)
                .WhereOr(
                    allDriver.Select(m => (Expression<Func<DriverTraceSessionLog, bool>>) (x => x.DriverId == m.Id))
                        .ToArray())
                .Take(100000).Execute();
            var allTraceOld =
                GetZipLogWhereOr<DriverTraceSessionLog, DriverTraceSessionLogZip>(company.DbId,
                    allDriver.Select(m => (Expression<Func<DriverTraceSessionLogZip, bool>>) (x => x.DriverId == m.Id))
                        .ToArray(), begin, end);
            allTrace.AddRange(allTraceNow);
            allTrace.AddRange(allTraceOld);

            foreach (var dr in allDriver)
            {
                result.Datas.Add(BuildAllReportByDriver(dr, overSpeedLog.Where(m => m.DriverId == dr.Id).ToList(),
                    allTrace.Where(m => m.DriverId == dr.Id).ToList()));
            }

            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     Lấy thông tin báo cáo theo lái xe của 1 công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDriverReportGet GetAllDriverReportByCompnayId(long companyId, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new AllDriverReportGet {Description = "Không tìm thấy công ty"};
            var ids = "";
            var drs = Cache.GetQueryContext<Driver>().GetByCompany(companyId);
            foreach (var device in drs)
            {
                if (ids != "")
                    ids += "|";
                ids += device.Id;
            }

            if (ids == "") return new AllDriverReportGet {Description = "Công ty không có lái xe"};
            return GetAllDriverReportByIds(ids, begin, end);
        }

        /// <summary>
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="allOverSpeed"></param>
        /// <param name="allTrace"></param>
        /// <returns></returns>
        private Driver31Report BuildAllReportByDriver(Driver driver, ICollection<DeviceOverSpeedLog> allOverSpeed,
            List<DriverTraceSessionLog> allTrace)
        {
            var distance = allTrace.Sum(m => m.Distance);

            if (distance == 0) distance = 1;

            var over5to10 =
                allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 5 && m.MaxSpeed < m.LimitSpeed + 10).ToList();
            var over10to20 =
                allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 10 && m.MaxSpeed < m.LimitSpeed + 20).ToList();
            var over20to35 =
                allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 20 && m.MaxSpeed < m.LimitSpeed + 35).ToList();
            var over35 = allOverSpeed.Where(m => m.MaxSpeed >= m.LimitSpeed + 35).ToList();

            //fix loi item.OverTime =0 khi test tren may test cho bo kiem tra
            foreach (var item in allTrace)
            {
                if (item.OverTime.TotalMinutes <= 0)
                    item.OverTime = item.EndTime - item.BeginTime;
            }

            return new Driver31Report
            {
                Distance = Math.Round(distance/1000.0, 2),
                Speed10To20 = over10to20.Count,
                Speed5To10 = over5to10.Count,
                Speed20To35 = over20to35.Count,
                Speed35 = over35.Count,
                Speed5To10Percent = Math.Round(over5to10.Sum(m => m.Distance)*100.0/distance, 2),
                Speed20To35Percent = Math.Round(over5to10.Sum(m => m.Distance)*100.0/distance, 2),
                Speed10To20Percent = Math.Round(over20to35.Sum(m => m.Distance)*100.0/distance, 2),
                Speed35Percent = Math.Round(over35.Sum(m => m.Distance)*100.0/distance, 2),
                Name = driver.Name,
                Gplx = driver.Gplx,
                OverTime4hCount = allTrace.Count(m => m.OverTime.TotalMinutes > 240)
            };
        }
    }
}