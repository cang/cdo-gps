#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Api
// TIME CREATE : 12:45 PM 18/12/2016
// FILENAME: Qc09Controller.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using Core.Models;
using Core.Models.Tranfer;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Utils;
using DataCenter.Core;
using StarSg.Utils.Models.Tranfer;
using StarSg.Utils.Models.Tranfer.DeviceManager;
using StarSg.Utils.Models.Tranfer.Qc09;
using DeviceActivityType = Datacenter.Model.Entity.DeviceActivityType;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     tính toán qc 09
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Qc09Controller : BaseController
    {
        [Import] private ILocationQuery _locationQuery;
        [Import] private ISplitRequest _splitRequest;
        ////////////////////////////////thống kê chi tiết thời gian lái xe 10h////////////////
        /// <summary>
        ///     tính vi phạm thời gian lái xe 10h theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Session10HByCompany([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var devices = new List<Device>();
                    foreach (var device in Cache.GetQueryContext<Device>().GetByCompany(companyId))
                    {
                        devices.Add(device);
                    }

                    return Ok(Session10HFunction(devices, beginTime, endTime, company.DbId).ToList());
                }
                Log.Warning("Qc09Report", $"công ty {companyId} không tồn tại");
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, "hàm Session10HByCompany");
            }
            return Ok(false);
        }

        /// <summary>
        ///   tính vi phạm thời gian lái xe 10h theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Session10HEx(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return Session10HEx(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return Session10HEx(companyId, groupId, ids, begin, end);
            else
                return Session10HEx(companyId, groupId, "", begin, end);
        }


        /// <summary>
        /// tính vi phạm thời gian lái xe 10h theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private IHttpActionResult Session10HEx(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return Ok(new List<DriverSession10HTranfer>(0));

            var result = new List<DriverSession10HTranfer>();
            IList<Device> allDevice = new List<Device>(1);
            try
            {

                //Tìm theo danh sách serial list
                if (!String.IsNullOrWhiteSpace(seriallist))
                {
                    var allSeial = new List<long>();
                    try
                    {
                        allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                    }
                    catch (Exception)
                    {
                        return Ok(new List<DriverSession10HTranfer>(0));
                    }
                    if (allSeial.Count == 0) return Ok(new List<DriverSession10HTranfer>(0));

                    allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();
                }
                //Tìm theo nhóm
                else if (groupId > 0)
                {
                    allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                }
                //Tìm theo cty
                else
                {
                    allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                }
                result = Session10HFunction(allDevice, begin, end, company.DbId).ToList();
                allDevice.Clear(); allDevice = null;
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Controller", e, "GeneralReport09Ex");
            }
            return Ok(result);
        }

        /// <summary>
        ///     thống kê vi phạm lái xe 10h ngày theo serial
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="serialList"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Session10HBySerial([FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, [FromUri] long companyId, [FromBody] List<long> serialList)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var devices = new List<Device>();
                    foreach (var serial in serialList)
                    {
                        var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                        if (device != null)
                        {
                            devices.Add(device);
                        }
                    }
                    return Ok(Session10HFunction(devices, beginTime, endTime, company.DbId).ToList());
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, "hàm Session10HBySerial");
            }
            return Ok(false);
        }

        /// <summary>
        ///     hàm tính thời gian 10h trong ngày
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private List<DriverSession10HTranfer> Session10HFunction(IList<Device> devices, DateTime beginTime,
            DateTime endTime, int dbId)
        {
            try
            {
                List<Guid> guidList = devices.Select(m => m.Indentity).ToList();
                var result = _splitRequest.Split(guidList, gg =>
                {
                    DriverTraceDaily10HLog tmp = null;
                    var t = DataContext.CreateQuery<DriverTraceDaily10HLog>(dbId).WhereOr(
                            gg.Select(guid => (Expression<Func<DriverTraceDaily10HLog, bool>>) (m => m.Indentity == guid))
                                .ToArray()).Where(m => (m.BeginTime >= beginTime) &&
                                                       (m.BeginTime <= endTime))
                        .Select(new Expression<Func<DriverTraceDaily10HLog, object>>[]
                        {
                            m => m.Id,
                            m => m.Indentity,
                            m => m.BeginTime,
                            m => m.EndTime,
                            m => m.CompanyId,
                            m => m.DriverId,
                            m => m.BeginLocation,
                            m => m.EndLocation,
                            m => m.OverTime,
                            //m => m.PauseTime,
                            //m => m.RunTime,
                            m => m.DbId
                        }, new Expression<Func<object>>[]
                        {
                            () => tmp.Id,
                            () => tmp.Indentity,
                            () => tmp.BeginTime,
                            () => tmp.EndTime,
                            () => tmp.CompanyId,
                            () => tmp.DriverId,
                            () => tmp.BeginLocation,
                            () => tmp.EndLocation,
                            () => tmp.OverTime,
                            //() => tmp.PauseTime,
                            //() => tmp.RunTime,
                            () => tmp.DbId
                        }).Execute().ToList();
                    return t;
                }, false);

                //return result.Where(m => m.OverTime.TotalMinutes > 600).Select(
                return result.Select(
                    m =>
                    {
                        var driver = Cache.GetQueryContext<Driver>().GetByKey(m.DriverId);
                        var device =
                            Cache.GetQueryContext<Device>().GetWhere(k => k.Indentity == m.Indentity).FirstOrDefault();

                        var overTime = (int)m.OverTime.TotalMinutes;
                        if (overTime <= 0) overTime = (int)(m.EndTime - m.BeginTime).TotalMinutes;


                        return new DriverSession10HTranfer
                        {
                            BeginTime = m.BeginTime,
                            EndTime = m.EndTime,
                            CompanyId = m.CompanyId,
                            Bs = device?.Bs ?? "",
                            EndLocation = new GpsPoint
                            {
                                Lat = m.EndLocation.Lat,
                                Lng = m.EndLocation.Lng,
                                Address = m.EndLocation.Address
                            },
                            DriverId = driver?.Id ?? 0,
                            ActivityType = (int)(device?.ActivityType ?? DeviceActivityType.None),
                            Gplx = driver?.Gplx ?? "",
                            BeginLocation = new GpsPoint
                            {
                                Lat = m.BeginLocation.Lat,
                                Lng = m.BeginLocation.Lng,
                                Address = m.BeginLocation.Address
                            },
                            DeviceSerial = device?.Serial ?? 0,
                            DriverName = driver?.Name ?? "",
                            Note = "",
                            UpdateTime = m.BeginTime,
                            OverTime = overTime
                        };
                    }).ToList();
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Report", e, $"Lỗi hàm Session10HFunction");
            }
            return new List<DriverSession10HTranfer>();
        }


        ////////////////////////////////thông kê tình hình vi phạm theo đơn vị///////////////////

        /// <summary>
        ///     hàm tính thống kê tổng hợp quy chuẩn 09
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        private List<GeneralReport09Log> GeneralReport09Function(int dbId, DateTime beginTime,DateTime endTime,IList<Device> devices)
        {
            var result = new List<GeneralReport09Log>();

            List<Guid> guidList = devices.Select(m => m.Indentity).ToList();

            //lấy báo cáo quá tốc độ 09
            var generalOverSpeed09Report = GeneralOverSpeed09Report(beginTime, endTime, guidList, dbId);
            //lấy báo cáo lái xe 4h
            var report4H =
                Session4HFromDevice(devices, beginTime, endTime, dbId).Where(m => m.OverTime > 240).ToList();
            //lấy báo cáo lái xe 10h
            var report10H = Session10HFunction(devices, beginTime, endTime, dbId).ToList();
            //lấy báo cáo tổng hợp xe
            var generalReport = GetGeneralReportLog(guidList, beginTime, endTime, dbId);

            //lấy thêm dữ liệu hiện hành trong ngày trên memory
            if (endTime.Date >= DateTime.Now.Date)
            {
                foreach(var device in devices)
                {
                    if (device == null) continue;
                    if (device.Temp.GeneralReportLog != null)
                        generalReport.Add(device.Temp.GeneralReportLog);
                }
            }

            //chia nhỏ data theo từng device
            var reportOverSpeedBySerial = generalOverSpeed09Report.GroupBy(m => m.Serial).ToDictionary(m => m.Key, m =>
            {
                var tmp = m.GetEnumerator();
                var temp = new List<OverSpeedLog09Tranfer>();
                while (tmp.MoveNext())
                {
                    temp.Add(tmp.Current);
                }
                return temp;
            });
            var report4HBySerial = report4H.GroupBy(m => m.DeviceSerial).ToDictionary(m => m.Key, m =>
            {
                var tmp = m.GetEnumerator();
                var temp = new List<DriverSessionLogTranfer>();
                while (tmp.MoveNext())
                {
                    temp.Add(tmp.Current);
                }
                return temp;
            });
            var report10HBySerial = report10H.GroupBy(m => m.DeviceSerial).ToDictionary(m => m.Key, m =>
            {
                var tmp = m.GetEnumerator();
                var temp = new List<DriverSession10HTranfer>();
                while (tmp.MoveNext())
                {
                    temp.Add(tmp.Current);
                }
                return temp;
            });
            //duyệt từng device để tạo báo cáo tổng hợp
            foreach (var guid in guidList)
            {
                try
                {
                    var device = Cache.GetQueryContext<Device>().GetWhere(m => m.Indentity == guid).FirstOrDefault();
                    if (device != null)
                    {
                        // thống kê quá tốc độ xe
                        var overSpeedLog = reportOverSpeedBySerial.ContainsKey(device.Serial)
                            ? reportOverSpeedBySerial[device.Serial].ToList()
                            : new List<OverSpeedLog09Tranfer>();
                        //số lần vi phạm lái xe quá 4h
                        var report4Hdevice = report4HBySerial.ContainsKey(device.Serial)
                            ? report4HBySerial[device.Serial].ToList()
                            : new List<DriverSessionLogTranfer>();
                        var report4HCount = report4Hdevice.Count;
                        //số lần vi phạm lái xe quá 10h
                        var report10Hdevice = report10HBySerial.ContainsKey(device.Serial)
                            ? report10HBySerial[device.Serial].ToList()
                            : new List<DriverSession10HTranfer>();
                        var report10HCount = report10Hdevice.Count;
                        //tính số ngày hoạt động
                        var countDay = Math.Round(endTime.Subtract(beginTime).TotalDays, MidpointRounding.AwayFromZero);
                        //số ngày vi phạm lái xe quá 4h, 8h
                        double countDateTime = 0;
                        for (var i = 0; i < countDay; i++)
                        {
                            var dateTimeCheck = new DateTime(beginTime.Year, beginTime.Month, beginTime.Day).AddDays(i);
                            if ((report4Hdevice.Count(
                                     m => new DateTime(m.BeginTime.Year, m.BeginTime.Month, m.BeginTime.Day) ==
                                          dateTimeCheck) > 0) || (report10Hdevice.Count(
                                                                      m =>
                                                                          new DateTime(m.BeginTime.Year,
                                                                              m.BeginTime.Month, m.BeginTime.Day) ==
                                                                          dateTimeCheck) > 0))
                            {
                                countDateTime++;
                            }
                        }
                        var report = new GeneralReport09Log
                        {
                            Serial = device.Serial,
                            Bs = device.Bs,
                            KmTotal =
                                Math.Round(
                                    (double) generalReport.Where(m => m.GuidId == device.Indentity).Sum(m => m.KmOnDay)/
                                    1000, 1),
                            Speed10To20Count = overSpeedLog.Count(m => (m.OverSpeed >= 10) && (m.OverSpeed < 20)),
                            Speed5To10Count = overSpeedLog.Count(m => (m.OverSpeed >= 5) && (m.OverSpeed < 10)),
                            Speed20To35Count = overSpeedLog.Count(m => (m.OverSpeed >= 20) && (m.OverSpeed < 35)),
                            Speed35Count = overSpeedLog.Count(m => m.OverSpeed >= 35),
                            Report4HCount = report4HCount,
                            Report10HCount = report10HCount,
                            OverTimePercent = Math.Round(countDateTime/countDay*100, 1),
                            Note = ""
                        };
                        report.PercentKm = (int) report.KmTotal != 0
                            ? Math.Round(overSpeedLog.Sum(m => m.TotalDistance)/report.KmTotal*100, 1)
                            : 0;
                        //report.OverspeedCount = report.Speed5To10Count + report.Speed10To20Count +
                        //                        report.Speed20To35Count +
                        //                        report.Speed35Count;
                        report.OverspeedCount = overSpeedLog.Count;
                        report.Speed1000Count = report.KmTotal >= 1000
                            ? Math.Round((double) report.OverspeedCount*1000/report.KmTotal, 0)
                            : report.OverspeedCount;
                        result.Add(report);
                    }
                }
                catch (Exception e)
                {
                    Log.Exception("GeneralReport09Function", e, "GeneralReport09Function");
                }
            }
            return result;
        }

        /// <summary>
        ///     báo cáo tổng hợp thông tư 09 theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GeneralReport09Company([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            var result = new List<GeneralReport09Log>();
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    result = GeneralReport09Function(company.DbId, beginTime, endTime,
                        Cache.GetQueryContext<Device>().GetByCompany(companyId)
                        );
                }
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Controller", e, "GeneralReport09Company");
            }
            return Ok(result);
        }

        /// <summary>
        ///    báo cáo tổng hợp thông tư 09 theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GeneralReport09Ex(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return GeneralReport09Ex(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return GeneralReport09Ex(companyId, groupId, ids, begin, end);
            else
                return GeneralReport09Ex(companyId, groupId, "", begin, end);
        }

        /// <summary>
        /// báo cáo tổng hợp thông tư 09 theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private IHttpActionResult GeneralReport09Ex(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return Ok(new List<GeneralReport09Log>(0));

            var result = new List<GeneralReport09Log>(0);
            IList<Device> allDevice = new List<Device>(1);

            try { 

                //Tìm theo danh sách serial list
                if (!String.IsNullOrWhiteSpace(seriallist))
                {
                    var allSeial = new List<long>();
                    try
                    {
                        allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                    }
                    catch (Exception)
                    {
                        return Ok(new List<GeneralReport09Log>(0));
                    }
                    if (allSeial.Count == 0) return Ok(new List<GeneralReport09Log>(0));

                    allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();
                }
                //Tìm theo nhóm
                else if (groupId > 0)
                {
                    allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                }
                //Tìm theo cty
                else
                {
                    allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                }

                result = GeneralReport09Function(company.DbId, begin, end, allDevice);
                allDevice.Clear(); allDevice = null;
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Controller", e, "GeneralReport09Ex");
            }
            return Ok(result);
        }


        /// <summary>
        ///     báo cáo tổng hợp thông tư 09 theo serial list
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GeneralReport09Serial([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, [FromBody] List<long> serialList)
        {
            var result = new List<GeneralReport09Log>();
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    IList<Device> devices = new List<Device>(1);
                    foreach (var serial in serialList)
                    {
                        var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                        if (device != null)
                            devices.Add(device);
                    }
                    result = GeneralReport09Function(company.DbId, beginTime, endTime, devices);
                }
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Controller", e, "GeneralReport09Serial");
            }
            return Ok(result);
        }


        ////////////////////////////////thống kê quá tốc độ xe/////////////////////////////

        /// <summary>
        ///     lấy báo cáo tổng hợp theo ngày
        /// </summary>
        /// <param name="guidList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private List<GeneralReportLog> GetGeneralReportLog(List<Guid> guidList, DateTime beginTime, DateTime endTime,
            int dbId)
        {
            GeneralReportLog tmp = null;
            var result = DataContext.CreateQuery<GeneralReportLog>(dbId).WhereOr(
                    guidList.Select(guid => (Expression<Func<GeneralReportLog, bool>>) (m => m.GuidId == guid))
                        .ToArray())
                .Where(m => (m.UpdateTime >= new DateTime(beginTime.Year, beginTime.Month, beginTime.Day)) &&
                            (m.UpdateTime <= new DateTime(endTime.Year, endTime.Month, endTime.Day)))
                .Select(new Expression<Func<GeneralReportLog, object>>[]
                {
                    m => m.Id,
                    m => m.GuidId,
                    m => m.UpdateTime,
                    m => m.CompanyId,
                    m => m.DbId,
                    m => m.InvalidOverTimeCount,
                    m => m.InvalidSpeedCount,
                    m => m.KmOnDay,
                    m => m.OpenDoorCount,
                    m => m.OverTimeInday,
                    m => m.OverTimeIndayCount,
                    m => m.PauseCount
                }, new Expression<Func<object>>[]
                {
                    () => tmp.Id,
                    () => tmp.GuidId,
                    () => tmp.UpdateTime,
                    () => tmp.CompanyId,
                    () => tmp.DbId,
                    () => tmp.InvalidOverTimeCount,
                    () => tmp.InvalidSpeedCount,
                    () => tmp.KmOnDay,
                    () => tmp.OpenDoorCount,
                    () => tmp.OverTimeInday,
                    () => tmp.OverTimeIndayCount,
                    () => tmp.PauseCount
                }).Execute().ToList();

            //get current date data
            if(endTime.DayOfYear == DateTime.Now.DayOfYear)
            {

            }

            return result;
        }

        /// <summary>
        ///     hàm lấy thống kê xe quá tốc độ
        /// </summary>
        /// <param name="dbId"></param>
        /// <param name="guidList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private List<OverSpeedReport09Tranfer> OverSpeedReportFunction(int dbId, List<Guid> guidList, DateTime beginTime,
            DateTime endTime)
        {
            var result = new List<OverSpeedReport09Tranfer>();
            var generalOverSpeed09Report = GeneralOverSpeed09Report(beginTime, endTime, guidList, dbId);
            var dictionary = generalOverSpeed09Report.GroupBy(m => m.Guid)
                .ToDictionary(m => m.Key, m => m.ToList());
            var generalReport = GetGeneralReportLog(guidList, beginTime, endTime, dbId);
            foreach (var deviceLog in dictionary)
            {
                try
                {
                    if (deviceLog.Value.Count > 0)
                    {
                        var device =
                            Cache.GetQueryContext<Device>()
                                .GetWhere(m => m.Indentity == deviceLog.Key)
                                .FirstOrDefault();
                        if (device != null)
                        {
                            double percenTime = 0;
                            double percenKm = 0;
                            if (generalReport.Where(m => m.GuidId == device.Indentity).Sum(m => m.OverTimeInday) !=
                                0)
                            {
                                percenTime = deviceLog.Value.Select(m => m.TotalTime)
                                                 .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t))
                                                 .TotalMinutes/
                                             generalReport.Where(m => m.GuidId == device.Indentity)
                                                 .Sum(m => m.OverTimeInday)*100;
                            }
                            if (generalReport.Where(m => m.GuidId == device.Indentity).Sum(m => m.KmOnDay)/1000 !=
                                0)
                            {
                                percenKm = deviceLog.Value.Sum(m => m.TotalDistance)/
                                           generalReport.Where(m => m.GuidId == device.Indentity)
                                               .Sum(m => m.KmOnDay/1000)*100;
                            }
                            var item = new OverSpeedReport09Tranfer
                            {
                                Serial = device.Serial,
                                Bs = device.Bs,
                                CompanyId = device.CompanyId,
                                ActivityType = (int) device.ActivityType,
                                Speed5To10Count =
                                    deviceLog.Value.Count(m => (m.OverSpeed >= 5) && (m.OverSpeed < 10)),
                                Speed10To20Count =
                                    deviceLog.Value.Count(m => (m.OverSpeed >= 10) && (m.OverSpeed < 20)),
                                Speed20To35Count =
                                    deviceLog.Value.Count(m => (m.OverSpeed >= 20) && (m.OverSpeed < 35)),
                                Speed35Count = deviceLog.Value.Count(m => m.OverSpeed >= 35),
                                KmOverspeed = Math.Pow(deviceLog.Value.Sum(m => m.TotalDistance), 1),
                                KmTotal = Math.Round(
                                    generalReport.Where(m => m.GuidId == device.Indentity).Sum(m => m.KmOnDay)/
                                    1000.0, 1),
                                PercentKm = Math.Round(percenKm, 1),
                                TimeTotal =
                                    TimeSpan.FromMinutes(
                                        generalReport.Where(m => m.GuidId == device.Indentity)
                                            .Sum(m => m.OverTimeInday)),
                                TimeOverspeed =
                                    deviceLog.Value.Select(m => m.TotalTime)
                                        .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t)),
                                PercentTime = Math.Round(percenTime, 1)
                            };
                            //item.OverspeedCount = item.Speed5To10Count + item.Speed10To20Count + item.Speed20To35Count +
                            //                      item.Speed35Count;
                            item.OverspeedCount = deviceLog.Value.Count;
                            item.Speed1000Count = item.KmTotal >= 1000
                                ? Math.Round((double) item.OverspeedCount*1000/item.KmTotal, 0)
                                : item.OverspeedCount;
                            result.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("Qc09Controller", ex,
                        $"lỗi hàm tạo báo cáo tổng hợp quá tốc độ guid: {deviceLog.Key}");
                }
            }
            return result;
        }


        /// <summary>
        ///     thống kê quá tốc độ theo xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult OverSpeedReport09([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, [FromBody] List<long> serialList)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var guidList = new List<Guid>();
                    foreach (var serial in serialList)
                    {
                        var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                        if (device != null)
                        {
                            guidList.Add(device.Indentity);
                        }
                    }
                    return Ok(OverSpeedReportFunction(company.DbId, guidList, beginTime, endTime));
                }
                Log.Warning("Qc09Controller", $"công ty {companyId} không tồn tại");
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Controller", exception, "hàm OverSpeedReport09");
            }
            return Ok(false);
        }

        /// <summary>
        ///     lấy thống kê quá tốc độ theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult OverSpeedReport09Company([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var guidList = new List<Guid>();
                    var deviceList = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                    foreach (var device in deviceList)
                    {
                        guidList.Add(device.Indentity);
                    }
                    return Ok(OverSpeedReportFunction(company.DbId, guidList, beginTime, endTime));
                }
                Log.Warning("Qc09Controller", $"công ty {companyId} không tồn tại");
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Controller", exception, "hàm OverSpeedReport09Company");
            }
            return Ok(false);
        }

        /// <summary>
        ///     lấy thông tin quá tốc độ theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetOverSpeedLog09([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    //chứa data của ngày hiện tại
                    var currentData = DataContext.GetWhere<OverSpeedLog09>(
                        m => (m.CompanyId == companyId) && (m.BeginTime >= beginTime) && (m.BeginTime <= endTime),
                        company.DbId).ToList();
                    //get từ bảng data zip
                    //Log.Warning("Qc09ReportController",
                    //    $"Hàm OverSpeedLog09 Lấy từ bảng data zip và serialize thành object");
                    var result = new List<OverSpeedLog09>();
                    //LoadZip<OverSpeedLog09, ZipOverSpeed09Log>(null, company.DbId,
                    //    new DateTime(beginTime.Year, beginTime.Month, beginTime.Day),
                    //    new DateTime(endTime.Year, endTime.Month, endTime.Day))
                    //    .Where(m => m.CompanyId == companyId && m.BeginTime >= beginTime &&
                    //                m.BeginTime <= endTime).ToList();
                    //nếu ngày kết thúc là ngày hiện tại thì cộng thêm vào kết quả trả về
                   // if (endTime.CheckCurrentDate(DateTimeFix.CurrentDayCompare))
                    {
                        result.AddRange(currentData);
                    }
                    return Ok(ConvertOverSpeedLog09Tranfers(result.OrderByDescending(m => m.BeginTime).ToList()));
                }
                Log.Warning("Qc09Controller", $"công ty {companyId} không tồn tại");
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Controller", exception, "hàm GetOverSpeedLog09");
            }
            return Ok(false);
        }

        /// <summary>
        ///   lấy thông tin quá tốc độ theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetOverSpeedLog09Ex(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return GetOverSpeedLog09Ex(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetOverSpeedLog09Ex(companyId, groupId, ids, begin, end);
            else
                return GetOverSpeedLog09Ex(companyId, groupId, "", begin, end);
        }

        /// <summary>
        /// lấy thông tin quá tốc độ theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private IHttpActionResult GetOverSpeedLog09Ex(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return Ok(ConvertOverSpeedLog09Tranfers(new List<OverSpeedLog09>(0)));

            var result = new List<OverSpeedLog09>();

            try
            {
                //Tìm theo danh sách serial list
                if (!String.IsNullOrWhiteSpace(seriallist))
                {
                    var allSeial = new List<long>();
                    try
                    {
                        allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                    }
                    catch (Exception ex)
                    {
                        return Ok(ConvertOverSpeedLog09Tranfers(new List<OverSpeedLog09>(0)));
                    }
                    if (allSeial.Count == 0) return Ok(ConvertOverSpeedLog09Tranfers(new List<OverSpeedLog09>(0)));

                    IList<Device> allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();
                    result =
                        DataContext.CreateQuery<OverSpeedLog09>(company.DbId)
                            .Where(m => m.BeginTime >= begin &&
                                    m.BeginTime <= end)
                            .WhereOr(
                                allDevice.Select(m => (Expression<Func<OverSpeedLog09, bool>>)(x => x.Indentity == m.Indentity))
                                    .ToArray()
                            ).Execute().ToList();
                }
                //Tìm theo nhóm
                else if (groupId > 0)
                {
                    result = DataContext.GetWhere<OverSpeedLog09>(
                        m => m.CompanyId == companyId && m.GroupId==groupId && m.BeginTime >= begin && m.BeginTime <= end,
                        company.DbId).ToList();
                }
                //Tìm theo cty
                else
                {
                    result = DataContext.GetWhere<OverSpeedLog09>(
                        m => m.CompanyId == companyId && m.BeginTime >= begin && m.BeginTime <= end,
                        company.DbId).ToList();
                }
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Controller", e, "GetOverSpeedLog09Ex");
            }
            return Ok(ConvertOverSpeedLog09Tranfers(result.OrderByDescending(m => m.BeginTime).ToList()));
        }

        /// <summary>
        ///     lấy quá tốc độ theo serial thông tư 09
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serialList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetOverSpeed09BySerial([FromUri] long companyId, [FromBody] List<long> serialList,
            [FromUri] DateTime beginTime, [FromUri] DateTime endTime)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var guidList = new List<Guid>();
                    foreach (var serial in serialList)
                    {
                        var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                        if (device != null)
                        {
                            guidList.Add(device.Indentity);
                        }
                    }
                    var result = GeneralOverSpeed09Report(beginTime, endTime, guidList,
                        company.DbId);
                    return Ok(result.OrderByDescending(m => m.BeginTime).ToList());
                }
                Log.Warning("Qc09Report", $"công ty {companyId} không tồn tại");
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, "hàm GetOverSpeed09BySerial");
            }
            return Ok(false);
        }

        /// <summary>
        ///     lấy báo cáo theo guid list
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="guidIdList"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private List<OverSpeedLog09Tranfer> GeneralOverSpeed09Report(DateTime beginTime, DateTime endTime,
            List<Guid> guidIdList,
            int dbId)
        {
            //thực hiện truy vấn bảng quá tốc độ
            //chứa data của ngày hiện tại
            var result = new List<OverSpeedLog09>();
            OverSpeedLog09 tmp = null;
            var currentData = DataContext.CreateQuery<OverSpeedLog09>(dbId).WhereOr(
                    guidIdList.Select(guid => (Expression<Func<OverSpeedLog09, bool>>) (m => m.Indentity == guid))
                        .ToArray()).Where(m => (m.BeginTime >= beginTime) &&
                                               (m.BeginTime <= endTime))
                .Select(new Expression<Func<OverSpeedLog09, object>>[]
                {
                    m => m.Id,
                    m => m.Serial,
                    m => m.CompanyId,
                    m => m.BeginTime,
                    m => m.DriverId,
                    m => m.EndTime,
                    m => m.GroupId,
                    m => m.LimitSpeed,
                    m => m.MaxSpeed,
                    m => m.BeginPoint,
                    m => m.Indentity,
                    m => m.AverageSpeed,
                    m => m.EndTime,
                    m => m.EndPoint,
                    m => m.TotalTimeOver,
                    m => m.TotalDistance
                }, new Expression<Func<object>>[]
                {
                    () => tmp.Id,
                    () => tmp.Serial,
                    () => tmp.CompanyId,
                    () => tmp.BeginTime,
                    () => tmp.DriverId,
                    () => tmp.EndTime,
                    () => tmp.GroupId,
                    () => tmp.LimitSpeed,
                    () => tmp.MaxSpeed,
                    () => tmp.BeginPoint,
                    () => tmp.Indentity,
                    () => tmp.AverageSpeed,
                    () => tmp.EndTime,
                    () => tmp.EndPoint,
                    () => tmp.TotalTimeOver,
                    () => tmp.TotalDistance
                }).Execute().ToList();
            //get từ bảng data zip
            //Log.Warning("ReportOverSpeed09",
            //    $"Hàm GeneralOverSpeed09Report Lấy từ bảng data zip và serialize thành object");
            //var zipOverSpeedLog =
            //    DataContext.CreateQuery<ZipOverSpeed09Log>(dbId).WhereOr(
            //        guidIdList.Select(
            //            guid =>
            //                (Expression<Func<ZipOverSpeed09Log, bool>>)
            //                    (m => m.GuidId == guid)).ToArray())
            //        .Where(m => m.TimeUpdate >= new DateTime(beginTime.Year, beginTime.Month, beginTime.Day) &&
            //                    m.TimeUpdate <= new DateTime(endTime.Year, endTime.Month, endTime.Day))
            //        .Execute().ToList();
            //foreach (var overSpeedLog in zipOverSpeedLog)
            //{
            //    //giải nén và serialize data thành object lại
            //    result.AddRange(
            //        overSpeedLog.Data.UnZip(Log)
            //            .ByteArrayToObject<List<OverSpeedLog09>>(Log)
            //            .Where(m => m.BeginTime >= beginTime &&
            //                        m.BeginTime <= endTime).ToList());
            //}
            //nếu ngày kết thúc là ngày hiện tại thì cộng thêm vào kết quả trả về
           // if (endTime.CheckCurrentDate(DateTimeFix.CurrentDayCompare))
            {
                result.AddRange(currentData);
            }
            return ConvertOverSpeedLog09Tranfers(result);
        }

        /// <summary>
        ///     chuyển đổi object quá tốc độ theo thông tư 09
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        private List<OverSpeedLog09Tranfer> ConvertOverSpeedLog09Tranfers(List<OverSpeedLog09> inputList)
        {
            return inputList.Select(k =>
            {
                //lấy device theo guidId
                var device = Cache.GetQueryContext<Device>().GetWhere(m => m.Indentity == k.Indentity).FirstOrDefault();
                var driver = Cache.GetQueryContext<Driver>().GetByKey(k.DriverId);
                return new OverSpeedLog09Tranfer
                {
                    Id = k.Id,
                    OverSpeed = k.AverageSpeed - k.LimitSpeed,
                    Serial = device != null ? device.Serial : 0,
                    CompanyId = k.CompanyId,
                    BeginTime = k.BeginTime,
                    DriverId = k.DriverId,
                    EndTime = k.EndTime,
                    GroupId = k.GroupId,
                    LimitSpeed = k.LimitSpeed,
                    MaxSpeed = k.MaxSpeed,
                    BeginPoint = new GpsPoint
                    {
                        Lat = k.BeginPoint.Lat,
                        Lng = k.BeginPoint.Lng,
                        Address = k.BeginPoint.Address
                    },
                    EndPoint = new GpsPoint
                    {
                        Lat = k.EndPoint.Lat,
                        Lng = k.EndPoint.Lng,
                        Address = k.EndPoint.Address
                    },
                    Bs = device != null ? device.Bs : "",
                    DriverName = driver != null ? driver.Name : "",
                    Gplx = driver != null ? driver.Gplx : "",
                    ActivityType = (int) (device?.ActivityType ?? DeviceActivityType.Taxi),
                    Guid = k.Indentity,
                    AverageSpeed = k.AverageSpeed != 0 ? k.AverageSpeed : k.MaxSpeed,
                    TotalTime = TimeSpan.FromSeconds(k.TotalTimeOver),
                    TotalDistance = Math.Round(k.TotalDistance/1000.0f, 1),
                    TimeUpdate = new DateTime(k.BeginTime.Year, k.BeginTime.Month, k.BeginTime.Day)
                };
            }).ToList();
        }

        ////////////////////////////tính thời gian lái xe liên tục 4h//////////////////////////////////

        /// <summary>
        ///     tính thời gian lái xe liên tục 4h theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Session4HByCompany([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var guidList = new List<Device>();
                    foreach (var device in Cache.GetQueryContext<Device>().GetByCompany(companyId))
                    {
                        guidList.Add(device);
                    }
                    return
                        Ok(
                            Session4HFromDevice(guidList, beginTime, endTime, company.DbId)
                                .Where(m => m.OverTime > 240
                                && m.OverTime <= 480 //a Phi bổ sung ngày 2018-11-13
                                )
                                .ToList());
                }
                Log.Warning("Qc09Report", $"công ty {companyId} không tồn tại");
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, "hàm GetOverSpeedLog");
            }
            return Ok(false);
        }


        /// <summary>
        ///   tính vi phạm thời gian lái xe 4h theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Session4HEx(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return Session4HEx(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return Session4HEx(companyId, groupId, ids, begin, end);
            else
                return Session4HEx(companyId, groupId, "", begin, end);
        }

        /// <summary>
        /// tính vi phạm thời gian lái xe 4h theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private IHttpActionResult Session4HEx(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return Ok(new List<DriverSessionLogTranfer>(0));

            var result = new List<DriverSessionLogTranfer>();
            IList<Device> allDevice = new List<Device>(1);
            try
            {

                //Tìm theo danh sách serial list
                if (!String.IsNullOrWhiteSpace(seriallist))
                {
                    var allSeial = new List<long>();
                    try
                    {
                        allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                    }
                    catch (Exception)
                    {
                        return Ok(new List<DriverSessionLogTranfer>(0));
                    }
                    if (allSeial.Count == 0) return Ok(new List<DriverSessionLogTranfer>(0));

                    allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();
                }
                //Tìm theo nhóm
                else if (groupId > 0)
                {
                    allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                }
                //Tìm theo cty
                else
                {
                    allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                }
                result = Session4HFromDevice(allDevice, begin, end, company.DbId).ToList();
                allDevice.Clear(); allDevice = null;
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Controller", e, "GeneralReport09Ex");
            }
            return Ok(result);
        }


        /// <summary>
        ///     lấy lái xe liên tục 4h theo serial
        /// </summary>
        /// <param name="serialList"></param>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpPost]
        //[Route("Qc09Report/Session4HBySerial")]
        public IHttpActionResult Session4HBySerial([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, [FromBody] List<long> serialList)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company != null)
                {
                    var guidList = new List<Device>();
                    foreach (var serial in serialList)
                    {
                        var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                        if (device != null)
                        {
                            guidList.Add(device);
                        }
                    }
                    return
                        Ok(
                            Session4HFromDevice(guidList, beginTime, endTime, company.DbId)
                                .Where(m => m.OverTime > 240
                                && m.OverTime <= 480 //a Phi bổ sung ngày 2018-11-13
                                )
                                .ToList());
                }
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, "hàm GetOverSpeedLog");
            }
            return Ok(false);
        }

        /// <summary>
        ///     tính các thời gian liên tục chạy xe, data được lấy từ thiết bị gửi lên
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public List<DriverSessionLogTranfer> Session4HFromDevice(IList<Device> devices, DateTime beginTime,
            DateTime endTime, int dbId)
        {
            try
            {
                List<Guid> listGuid = devices.Select(m => m.Indentity).ToList();
                var result = _splitRequest.Split(listGuid, gg =>
                {
                    DriverTraceSessionLog tmp = null;
                    var t = DataContext.CreateQuery<DriverTraceSessionLog>(dbId).WhereOr(
                            gg.Select(guid => (Expression<Func<DriverTraceSessionLog, bool>>) (m => m.Indentity == guid))
                                .ToArray()).Where(m => (m.BeginTime >= beginTime) &&
                                                       (m.BeginTime <= endTime))
                        .Select(new Expression<Func<DriverTraceSessionLog, object>>[]
                        {
                            m => m.Id,
                            m => m.Indentity,
                            m => m.BeginTime,
                            m => m.EndTime,
                            m => m.CompanyId,
                            m => m.DriverId,
                            m => m.BeginLocation,
                            m => m.EndLocation,
                            m => m.OverTime,
                            m => m.Id
                        }, new Expression<Func<object>>[]
                        {
                            () => tmp.Id,
                            () => tmp.Indentity,
                            () => tmp.BeginTime,
                            () => tmp.EndTime,
                            () => tmp.CompanyId,
                            () => tmp.DriverId,
                            () => tmp.BeginLocation,
                            () => tmp.EndLocation,
                            () => tmp.OverTime,
                            () => tmp.Id
                        }).Execute().ToList();
                    return t;
                }, false);
                return result.Select(
                    m =>
                    {
                        var device =
                            Cache.GetQueryContext<Device>().GetWhere(k => k.Indentity == m.Indentity).FirstOrDefault();
                        var driver = Cache.GetQueryContext<Driver>().GetByKey(m.DriverId);

                        var overTime = (int)m.OverTime.TotalMinutes;
                        if(overTime<=0) overTime = (int)(m.EndTime - m.BeginTime).TotalMinutes;
                        return new DriverSessionLogTranfer
                        {
                            BeginTime = m.BeginTime,
                            EndTime = m.EndTime,
                            CompanyId = m.CompanyId,
                            Id = m.Id,
                            Bs = device?.Bs ?? "",
                            EndLocation = new GpsPoint
                            {
                                Lat = m.EndLocation.Lat,
                                Lng = m.EndLocation.Lng,
                                Address = m.EndLocation.Address
                            },
                            OverTime = overTime,
                            DriverId = driver?.Id ?? 0,
                            Gplx = driver?.Gplx ?? "",
                            BeginLocation = new GpsPoint
                            {
                                Lat = m.BeginLocation.Lat,
                                Lng = m.BeginLocation.Lng,
                                Address = m.BeginLocation.Address
                            },
                            ActivityType = (int) (device?.ActivityType ?? DeviceActivityType.None),
                            DeviceSerial = device?.Serial ?? 0,
                            DriverName = driver?.Name ?? "",
                            GroupId = device?.GroupId ?? 0,
                            Note =
                                m.OverTime.TotalMinutes >= 600
                                    ? "Lái xe quá 10 tiếng"
                                    : m.OverTime.TotalMinutes >= 240 ? "Lái xe quá 4 tiếng" : "",
                            DateTime = m.BeginTime
                        };
                    }).ToList();
            }
            catch (Exception e)
            {
                Log.Exception("Qc09Report", e, $"Lỗi hàm Session4HFromDevice");
            }
            return new List<DriverSessionLogTranfer>();
        }

        /////////////////////////////////báo cáo vi phạm không truyền dữ liệu cho bộ //////////////////////////////////////////

        /// <summary>
        ///     lấy báo cáo chi tiết không truyền dữ liệu lên bộ
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LostDataDetail09([FromUri] long serial,
            [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime)
        {
            try
            {
                var result = new List<DeviceTraceLogTranfer>();
                var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                if (device == null)
                    return Ok(false);
                var company = Cache.GetCompanyById(device.CompanyId);
                if (company == null)
                    return Ok(false);

                result.AddRange(
                    FunctionLostData(new List<Guid> {device.Indentity}, beginTime, endTime, company.DbId).Select(m =>
                    {
                        var timeSpan = m.EndTime.Subtract(m.BeginTime);
                        var end = (m.EndLocation != null) && ((int) m.EndLocation.Lat != 0) &&
                                  ((int) m.EndLocation.Lng != 0)
                            ? m.EndLocation
                            : m.BeginLocation;

                        return new DeviceTraceLogTranfer
                        {
                            Serial = device.Serial,
                            EndTime = m.EndTime,
                            BeginTime = m.BeginTime,
                            BeginLocation = new GpsPoint
                            {
                                Lat = m.BeginLocation.Lat,
                                Lng = m.BeginLocation.Lng,
                                Address = m.BeginLocation.Address
                            },
                            OverTime = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds),
                            DeviceStatusType = (int)TraceType.LostGsm,
                            Bs = device.Bs,
                            ActivityType = (int) device.ActivityType,
                            EndLocation = new GpsPoint
                            {
                                Lat = end.Lat,
                                Lng = end.Lng,
                                Address = end.Address
                            },
                            Note = "Xe mất liên lạc"
                        };
                    }));

                //lấy trong ngày
                if (endTime >= DateTime.Now && device.Temp.DeviceLostGsmLog != null
                    && device.Status != null && device.Status.BasicStatus != null
                    && (device.Status.BasicStatus.ServerRecv - device.Temp.DeviceLostGsmLog.BeginTime).TotalMinutes >= 60
                    )
                {
                    DeviceTraceLog trace = device.Temp.DeviceLostGsmLog;

                    trace.DbId = company.DbId;
                    trace.EndLocation = device.Status.BasicStatus.GpsInfo;
                    trace.EndTime = device.Status.BasicStatus.ServerRecv;
                    //tính khoảng cách theo thông tin thiết bị gửi lên
                    trace.Distance = device.Status.BasicStatus.TotalGpsDistance - trace.Distance;
                    if (trace.Distance < 0) trace.Distance = 0;

                    //Lấy địa chỉ begin location
                    _locationQuery.GetAddress(trace.BeginLocation);
                    //Nếu như vị trí kết thúc gần với vị trí ban đầu thì lấy địa chỉ ban đầu
                    if (trace.BeginLocation != null
                        && StarSg.Utils.Geos.GeoUtil.Distance(trace.BeginLocation.Lat, trace.BeginLocation.Lng
                            , trace.EndLocation.Lat, trace.EndLocation.Lng) < 100)
                        trace.EndLocation.Address = trace.BeginLocation.Address;
                    else
                        _locationQuery.GetAddress(trace.EndLocation);

                    result.Add(new DeviceTraceLogTranfer
                    {
                        Serial = trace.Serial,
                        EndTime = device.EndTime,
                        BeginTime = trace.BeginTime,
                        BeginLocation = new GpsPoint
                        {
                            Lat = trace.BeginLocation.Lat,
                            Lng = trace.BeginLocation.Lng,
                            Address = trace.BeginLocation.Address
                        },
                        OverTime = trace.EndTime - trace.BeginTime,
                        DeviceStatusType = (int)TraceType.LostGsm,
                        Bs = device.Bs,
                        ActivityType = (int)device.ActivityType,
                        EndLocation = new GpsPoint
                        {
                            Lat = trace.EndLocation.Lat,
                            Lng = trace.EndLocation.Lng,
                            Address = trace.EndLocation.Address
                        },
                        Note = "Xe đang mất liên lạc"
                    });
                }

                //tính các xe đang mất liên lạc
                return Ok(result.OrderByDescending(m => m.EndTime).ToList());
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, "hàm LostDataDetail09");
            }
            return Ok(false);
        }


        /// <summary>
        ///     thống kê mất truyền data lên bộ
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="serialList"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult LostDataReport09([FromUri] long companyId, [FromUri] DateTime beginTime,
            [FromUri] DateTime endTime, [FromBody] List<long> serialList)
        {
            var result = new List<LostDataReport09Tranfer>();
            try
            {
                var traceLogs = new List<DeviceTraceLog>();
                var company = Cache.GetCompanyById(companyId);
                if (company == null)
                    return Ok(false);
                var guidList = new List<Guid>();
                foreach (var serial in serialList)
                {
                    var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                    if (device != null)
                    {
                        guidList.Add(device.Indentity);

                        //lấy trong ngày
                        if (endTime >= DateTime.Now && device.Temp.DeviceLostGsmLog!=null
                                && device.Status!=null && device.Status.BasicStatus!=null
                                 && (device.Status.BasicStatus.ServerRecv - device.Temp.DeviceLostGsmLog.BeginTime).TotalMinutes >= 60
                                )
                        {
                            DeviceTraceLog trace = device.Temp.DeviceLostGsmLog;

                            trace.DbId = company.DbId;
                            trace.EndLocation = device.Status.BasicStatus.GpsInfo;
                            trace.EndTime = device.Status.BasicStatus.ServerRecv;
                            //tính khoảng cách theo thông tin thiết bị gửi lên
                            trace.Distance = device.Status.BasicStatus.TotalGpsDistance - trace.Distance;
                            if (trace.Distance < 0) trace.Distance = 0;

                            traceLogs.Add(trace);
                        }
                    }
                }
                traceLogs.AddRange(FunctionLostData(guidList, beginTime, endTime, company.DbId));
               
                //chia theo từng cụm device
                var resultDictionary = traceLogs.GroupBy(m => m.Indentity).ToDictionary(m => m.Key, m => m.ToList());
                foreach (var deviceLog in resultDictionary)
                {
                    if (deviceLog.Value.Count > 0)
                    {
                        var device =
                            Cache.GetQueryContext<Device>()
                                .GetWhere(m => m.Indentity == deviceLog.Key)
                                .FirstOrDefault();
                        if (device != null)
                        {
                            ////tính các xe đang mất liên lạc
                            //var isLostingData = false;
                            //var overTime = DateTime.Now.Subtract(device.Status.BasicStatus.ClientSend).TotalMinutes;
                            //if (overTime > company.Setting.TimeoutLostDevice)
                            //{
                            //    isLostingData = true;
                            //    deviceLog.Value.Add(new DeviceTraceLog
                            //    {
                            //        Indentity = device.Indentity,
                            //        BeginLocation = device.Status.BasicStatus.GpsInfo,
                            //        Serial = device.Serial,
                            //        EndTime = DateTime.Now,
                            //        BeginTime = device.Status.BasicStatus.ClientSend,
                            //        Type = TraceType.LostGsm,
                            //        CompanyId = device.CompanyId
                            //    });
                            //}
                            var timeTotal = deviceLog.Value.Select(m => m.EndTime.Subtract(m.BeginTime))
                                .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

                            var reportDevice = new LostDataReport09Tranfer
                            {
                                Bs = device.Bs,
                                Serial = device.Serial,
                                CompanyTool = "Ngôi Sao Sài Gòn",
                                Type = (int)device.ActivityType,
                                KhongTruyenDataCount = deviceLog.Value.Count,
                                TimeTotal =
                                    new TimeSpan(timeTotal.Days, timeTotal.Hours, timeTotal.Minutes, timeTotal.Seconds),
                                CompanyId = device.CompanyId,
                                //Note = isLostingData ? "Xe đang mất liên lạc" : ""
                                Note = "Xe mất liên lạc",
                                TimeUpdate = deviceLog.Value.Select(m => m.EndTime).Aggregate(DateTime.MinValue, (val, t) => val = val > t ? val : t)
                            };
                            result.Add(reportDevice);
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception exception)
            {
                Log.Exception("Qc09Report", exception, $"Hàm LostDataReport09");
            }
            return Ok(true);
        }

        /// <summary>
        ///     Thống kê mất truyền data lên bộ
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LostDataReport09Ex(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return Ok(LostDataReport09Ex(companyId, groupId, serial.ToString(), begin, end));
            else if (!String.IsNullOrWhiteSpace(ids))
                return Ok(LostDataReport09Ex(companyId, groupId, ids, begin, end));
            else
                return Ok(LostDataReport09Ex(companyId, groupId, "", begin, end));
        }

        private List<LostDataReport09Tranfer> LostDataReport09Ex(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new List<LostDataReport09Tranfer>(0);

            IList<Device> allDevice;
            List<DeviceTraceLog> alltrace = new List<DeviceTraceLog>();

            //Tìm theo danh sách serial list
            if (!String.IsNullOrWhiteSpace(seriallist))
            {
                var allSeial = new List<long>();
                try
                {
                    allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                }
                catch (Exception)
                {
                    return new List<LostDataReport09Tranfer>(0);
                }
                if (allSeial.Count == 0) return new List<LostDataReport09Tranfer>(0);
                allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();

                alltrace.AddRange(
                DataContext.CreateQuery<DeviceLostGsmLog>(company.DbId)
                    .Where(m => m.BeginTime >= begin &&
                                m.BeginTime <= end)
                    .WhereOr(
                        allDevice.Select(m => (Expression<Func<DeviceLostGsmLog, bool>>)(x => x.Indentity == m.Indentity))
                            .ToArray()
                    ).Execute().Select(m => m.CopyTo<DeviceTraceLog>()));
            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                alltrace.AddRange(
                   DataContext.CreateQuery<DeviceLostGsmLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin &&
                                m.BeginTime <= end &&
                                m.CompanyId == companyId &&
                                m.GroupId == groupId
                                ).Execute().Select(m => m.CopyTo<DeviceTraceLog>()));
            }
            //Tìm theo cty
            else
            {
                allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                alltrace.AddRange(
                   DataContext.CreateQuery<DeviceLostGsmLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin &&
                                m.BeginTime <= end &&
                                m.CompanyId == companyId
                                ).Execute().Select(m => m.CopyTo<DeviceTraceLog>()));
            }

            //lấy thông tin trong ngày
            if (end.Date >= DateTime.Now.Date)
            {
                foreach (var device in allDevice)
                {
                    if (end >= DateTime.Now && device.Temp.DeviceLostGsmLog != null
                            && device.Status != null && device.Status.BasicStatus != null
                             && (device.Status.BasicStatus.ServerRecv - device.Temp.DeviceLostGsmLog.BeginTime).TotalMinutes >= 60
                            )
                    {
                        DeviceTraceLog trace = device.Temp.DeviceLostGsmLog;

                        trace.DbId = company.DbId;
                        trace.EndLocation = device.Status.BasicStatus.GpsInfo;
                        trace.EndTime = device.Status.BasicStatus.ServerRecv;
                        //tính khoảng cách theo thông tin thiết bị gửi lên
                        trace.Distance = device.Status.BasicStatus.TotalGpsDistance - trace.Distance;
                        if (trace.Distance < 0) trace.Distance = 0;

                        alltrace.Add(trace);
                    }
                }
            }

            if (allDevice == null || allDevice.Count <= 0) return new List<LostDataReport09Tranfer>(0);

            //chia theo từng cụm device
            var result = new List<LostDataReport09Tranfer>();

            var resultDictionary = alltrace.GroupBy(m => m.Indentity).ToDictionary(m => m.Key, m => m.ToList());
            foreach (var deviceLog in resultDictionary)
            {
                if (deviceLog.Value.Count > 0)
                {
                    var device =
                        Cache.GetQueryContext<Device>()
                            .GetWhere(m => m.Indentity == deviceLog.Key)
                            .FirstOrDefault();
                    if (device != null)
                    {
                        var timeTotal = deviceLog.Value.Select(m => m.EndTime.Subtract(m.BeginTime))
                            .Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

                        var reportDevice = new LostDataReport09Tranfer
                        {
                            Bs = device.Bs,
                            Serial = device.Serial,
                            CompanyTool = "Ngôi Sao Sài Gòn",
                            Type = (int)device.ActivityType,
                            KhongTruyenDataCount = deviceLog.Value.Count,
                            TimeTotal =
                                new TimeSpan(timeTotal.Days, timeTotal.Hours, timeTotal.Minutes, timeTotal.Seconds),
                            CompanyId = device.CompanyId,
                            Note = "Xe mất liên lạc",
                            TimeUpdate = deviceLog.Value.Select(m => m.EndTime).Aggregate(DateTime.MinValue, (val, t) => val = val > t ? val : t)
                        };
                        result.Add(reportDevice);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     function tính xe mất liên lạc theo guid
        /// </summary>
        /// <param name="guidList"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private List<DeviceTraceLog> FunctionLostData(List<Guid> guidList, DateTime beginTime, DateTime endTime,
            int dbId)
        {
            var traceLogs = new List<DeviceTraceLog>();
            try
            {
                DeviceLostGsmLog tmp = null;
                var currentData = DataContext.CreateQuery<DeviceLostGsmLog>(dbId).WhereOr(
                        guidList.Select(
                                guid =>
                                    (Expression<Func<DeviceLostGsmLog, bool>>)
                                    (m => (m.Indentity == guid)))
                            .ToArray())
                    .Where(m => (m.BeginTime >= beginTime) &&
                                (m.BeginTime <= endTime))
                    .Select(new Expression<Func<DeviceLostGsmLog, object>>[]
                    {
                        m => m.Indentity,
                        m => m.BeginTime,
                        m => m.BeginLocation,
                        m => m.CompanyId,
                        m => m.Type,
                        m => m.DbId,
                        m => m.DriverId,
                        m => m.EndLocation,
                        m => m.EndTime,
                        m => m.Id,
                        m => m.Serial,
                        m => m.GroupId,
                        m => m.Distance
                    }, new Expression<Func<object>>[]
                    {
                        () => tmp.Indentity,
                        () => tmp.BeginTime,
                        () => tmp.BeginLocation,
                        () => tmp.CompanyId,
                        () => tmp.Type,
                        () => tmp.DbId,
                        () => tmp.DriverId,
                        () => tmp.EndLocation,
                        () => tmp.EndTime,
                        () => tmp.Id,
                        () => tmp.Serial,
                        () => tmp.GroupId,
                        () => tmp.Distance
                    //}).Execute();//.Where(m => m.BeginTime.IsValidLostData() && m.EndTime.IsValidLostData()).ToList();
                    }).Execute().Select(m => m.CopyTo<DeviceTraceLog>());

                //get từ bảng data zip
                //var devicePauseLogs =
                //    DataContext.CreateQuery<ZipDeviceTraceLog>(dbId).WhereOr(
                //            guidList.Select(
                //                guid =>
                //                    (Expression<Func<ZipDeviceTraceLog, bool>>)
                //                    (m => m.GuidId == guid)).ToArray())
                //        .Where(m => (m.TimeUpdate >= new DateTime(beginTime.Year, beginTime.Month, beginTime.Day)) &&
                //                    (m.TimeUpdate <= new DateTime(endTime.Year, endTime.Month, endTime.Day)))
                //        .Execute().ToList();
                //foreach (var deviceLog in devicePauseLogs)
                //{
                //    //giải nén và serialize data thành object lại
                //    traceLogs.AddRange(
                //        deviceLog.Data.UnZip()
                //            .ByteArrayToObject<List<DeviceTraceLog>>()
                //            .Where(
                //                m =>
                //                    (m.EndTime >= beginTime) && (m.EndTime <= endTime) &&
                //                    (m.DeviceStatusType == DeviceStatusType.LostGsm) && m.BeginTime.IsValidLostData() &&
                //                    m.EndTime.IsValidLostData()).ToList());
                //}


                //nếu ngày kết thúc là ngày hiện tại thì cộng thêm vào kết quả trả về
                //if (endTime.CheckCurrentDate(DateTimeFix.CurrentDayCompare))
                {
                    traceLogs.AddRange(currentData);
                }
            }
            catch (Exception e)
            {
                Log.Exception("FunctionLostData", e, "FunctionLostData");
            }
            return traceLogs;
        }
    }
}