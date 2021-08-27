//#define TIME_DEBUG

#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Routing;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Log.ZipLog;
using StarSg.Utils.Models.DatacenterResponse.Enterprise;
using StarSg.Utils.Models.Tranfer;
using StarSg.Utils.Models.Tranfer.DeviceManager;
using Datacenter.Model.Utils;
using Newtonsoft.Json;
using StarSg.Utils.Utils;
using StarSg.Core;
using DaoDatabase;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     thông tin báo cáo doanh nghiệp
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class EnterpriseController : BaseController
    {
        /// <summary>
        ///     Lấy thông tin hành trình
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        [HttpGet]

        public DeviceTripGet GetTrip([FromUri] long serial, [FromUri]DateTime begin, [FromUri]DateTime end, [FromUri]bool zip)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceTripGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceTripGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };
            var result = new DeviceTripGet();
            result.Datas = new List<DeviceTripTranfer>();
            var nowLog =
                DataContext.CreateQuery<DeviceLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.DeviceStatus.ClientSend >= begin
                                && m.DeviceStatus.ClientSend <= end)
                    //.Take(30000).Execute();
                    .Execute();

            var oldLog =
                GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);

            var allData = new List<DeviceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData.OrderBy(m => m.DeviceStatus.ClientSend))
            {
                var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverStatus?.DriverId);
                result.Datas.Add(new DeviceTripTranfer
                {
                    Bs = device.Bs,
                    DriverName = dr?.Name ?? "",
                    GplxCreateTime = dr?.CreateDateOfGplx ?? new DateTime(2000, 1, 1),
                    GplxEndTime = dr?.EndDateOfGplx ?? new DateTime(2000, 1, 1),
                    Distance = log.DeviceStatus.TotalCurrentGpsDistance,
                    TotalDistance = log.DeviceStatus.TotalGpsDistance,
                    Gplx = dr?.Gplx ?? "",
                    Location = new GpsPoint
                    {
                        Lat = log.DeviceStatus.GpsInfo.Lat,
                        Lng = log.DeviceStatus.GpsInfo.Lng,
                        Address = log.DeviceStatus.GpsInfo.Address
                    },
                    Speed = log.DeviceStatus.Speed,
                    AirMachineStatus = log.DeviceStatus.AirMachine,
                    Angle = log.DeviceStatus.Angle,
                    MachineStatus = log.DeviceStatus.Machine,
                    Power = log.DeviceStatus.Power,
                    TimeUpdate = log.DeviceStatus.ClientSend,
                    Temperature = log.DeviceStatus.Temperature,
                    GpsStatus = log.DeviceStatus.GpsStatus,
                    Fuel = log.DeviceStatus.Fuel,
                    DoorStatus = log.DeviceStatus.Door
                });
            }
            result.Status = 1;
            result.Description = "OK";

            if(zip && result.Status > 0)
            {
                result.ZipData = JsonConvert.SerializeObject(result.Datas).CompressDeflate();
                result.Datas = null;
            }

            return result;
        }

        /// <summary>
        ///     Lấy thông tin hành trình
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        [HttpGet]

        public DeviceTripGet GetTripSQL([FromUri] long serial, [FromUri]DateTime begin, [FromUri]DateTime end, [FromUri]bool zip)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceTripGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceTripGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };
            var result = new DeviceTripGet();
            result.Datas = new List<DeviceTripTranfer>();

            var nowLog = new List<DeviceLog>();
            String SQL = $@"SELECT * FROM DeviceLog WHERE Indentity = '{device.Indentity}' AND DeviceStatus_ClientSend >= '{begin}' AND DeviceStatus_ClientSend <= '{end}'";
            DataContext.CustomHandle<NHibernate.ISession>(m => {
                try
                {
                    NHibernate.ISQLQuery query = m.CreateSQLQuery(SQL);
                    IList<dynamic> list = query.DynamicList();
                    if (list != null)
                    {
                        foreach (var obj in list)
                        {
                            DeviceLog acc = new DeviceLog();
                            acc.Id = obj.Id;
                            acc.DbId = obj.DbId;
                            acc.Serial = obj.Serial;
                            acc.Indentity = obj.Indentity;
                            acc.GroupId = obj.GroupId;
                            acc.CompanyId = obj.CompanyId;

                            acc.DeviceStatus = new Model.Components.DeviceStatusInfo();
                            acc.DeviceStatus.ClientSend = obj.DeviceStatus_ClientSend;
                            acc.DeviceStatus.ServerRecv = obj.DeviceStatus_ServerRecv;
                            acc.DeviceStatus.GpsStatus = obj.DeviceStatus_GpsStatus;
                            acc.DeviceStatus.TotalGpsDistance = obj.DeviceStatus_TotalGpsDistance;
                            acc.DeviceStatus.TotalCurrentGpsDistance = obj.DeviceStatus_TotalCurrentGpsDistance;
                            acc.DeviceStatus.Fuel = obj.DeviceStatus_Fuel;
                            acc.DeviceStatus.Temperature = obj.DeviceStatus_Temperature;
                            acc.DeviceStatus.GsmSignal = obj.DeviceStatus_GsmSignal;
                            acc.DeviceStatus.Power = obj.DeviceStatus_Power;
                            acc.DeviceStatus.Machine = obj.DeviceStatus_Machine;
                            acc.DeviceStatus.AirMachine = obj.DeviceStatus_AirMachine;
                            acc.DeviceStatus.Sos = obj.DeviceStatus_Sos;
                            acc.DeviceStatus.UseTemperature = obj.DeviceStatus_UseTemperature;
                            acc.DeviceStatus.UseFuel = obj.DeviceStatus_UseFuel;
                            acc.DeviceStatus.UseRfid = obj.DeviceStatus_UseRfid;
                            acc.DeviceStatus.Door = obj.DeviceStatus_Door;
                            acc.DeviceStatus.SpeedTrace = obj.DeviceStatus_SpeedTrace;
                            acc.DeviceStatus.Angle = obj.DeviceStatus_Angle;
                            acc.DeviceStatus.Speed = obj.DeviceStatus_Speed;

                            acc.DeviceStatus.GpsInfo = new Model.Components.GpsLocation();
                            acc.DeviceStatus.GpsInfo.Lat = obj.GpsInfo_Lat;
                            acc.DeviceStatus.GpsInfo.Lng = obj.GpsInfo_Lng;
                            if (!Object.ReferenceEquals(null, obj.GpsInfo_Address))
                                acc.DeviceStatus.GpsInfo.Address = obj.GpsInfo_Address;

                            acc.DriverStatus = new Model.Components.DriverStatusInfo();
                            acc.DriverStatus.DriverId = obj.DriverStatus_DriverId;
                            acc.DriverStatus.Name = obj.DriverStatus_Name;
                            acc.DriverStatus.Gplx= obj.DriverStatus_Gplx;
                            acc.DriverStatus.TimeBeginWorkInSession = obj.DriverStatus_TimeBeginWorkInSession;
                            acc.DriverStatus.TimeWorkInDay = obj.DriverStatus_TimeWorkInDay;
                            acc.DriverStatus.TimeWork = obj.DriverStatus_TimeWork;

                            nowLog.Add(acc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("GetTripSQL", ex, SQL);
                }
            }, company.DbId);

            var oldLog =
                GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);

            var allData = new List<DeviceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData.OrderBy(m => m.DeviceStatus.ClientSend))
            {
                var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverStatus?.DriverId);
                result.Datas.Add(new DeviceTripTranfer
                {
                    Bs = device.Bs,
                    DriverName = dr?.Name ?? "",
                    GplxCreateTime = dr?.CreateDateOfGplx ?? new DateTime(2000, 1, 1),
                    GplxEndTime = dr?.EndDateOfGplx ?? new DateTime(2000, 1, 1),
                    Distance = log.DeviceStatus.TotalCurrentGpsDistance,
                    TotalDistance = log.DeviceStatus.TotalGpsDistance,
                    Gplx = dr?.Gplx ?? "",
                    Location = new GpsPoint
                    {
                        Lat = log.DeviceStatus.GpsInfo.Lat,
                        Lng = log.DeviceStatus.GpsInfo.Lng,
                        Address = log.DeviceStatus.GpsInfo.Address
                    },
                    Speed = log.DeviceStatus.Speed,
                    AirMachineStatus = log.DeviceStatus.AirMachine,
                    Angle = log.DeviceStatus.Angle,
                    MachineStatus = log.DeviceStatus.Machine,
                    Power = log.DeviceStatus.Power,
                    TimeUpdate = log.DeviceStatus.ClientSend,
                    Temperature = log.DeviceStatus.Temperature,
                    GpsStatus = log.DeviceStatus.GpsStatus,
                    Fuel = log.DeviceStatus.Fuel,
                    DoorStatus = log.DeviceStatus.Door
                });
            }
            result.Status = 1;
            result.Description = "OK";

            if (zip && result.Status > 0)
            {
                result.ZipData = JsonConvert.SerializeObject(result.Datas).CompressDeflate();
                result.Datas = null;
            }

            return result;
        }

        /// <summary>
        ///     lấy thông tin cuốc xe
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Enterprise/GetSession")]
        public DeviceSessionGet GetSession(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceSessionGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceSessionGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };

            var result = new DeviceSessionGet();
            result.Datas = new List<DeviceSessionLogTranfer>();

            var nowLog =
                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin
                                && m.BeginTime <= end && m.Type == TraceType.Machine)
                    //.Take(30000).Execute();
                    .Execute();

            var oldLog =
                GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
                    .Where(m => m.Type == TraceType.Machine);

            var allData = new List<DeviceTraceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);

            foreach (var log in allData.OrderBy(m => m.BeginTime))
            {
                //var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);
                result.Datas.Add(new DeviceSessionLogTranfer
                {
                    Id = log.Id,
                    BeginTime = log.BeginTime,
                    EndTime = log.EndTime,
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
                    },
                    Bs = device.Bs,
                    DistanceGps = log.Distance,
                    OverTime = log.EndTime - log.BeginTime,
                    Serial = device.Serial
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin dừng đỗ
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceStopLogGet GetStop(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceStopLogGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceStopLogGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };
            var result = new DeviceStopLogGet();
            result.Datas = new List<DeviceStopTranfer>();

            var nowLog =
                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin
                                && m.BeginTime <= end && m.Type == TraceType.Stop)
                    //.Take(30000).Execute();
                    .Execute();

            var oldLog =
                GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
                    .Where(m => m.Type == TraceType.Stop);

            var allData = new List<DeviceTraceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);

            foreach (var log in allData.OrderBy(m => m.BeginTime))
            {
                //todo: coi lại
                //var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);
                result.Datas.Add(new DeviceStopTranfer
                {
                    Begin = log.BeginTime,
                    End = log.EndTime,
                    Location = new GpsPoint
                    {
                        Lat = log.BeginLocation.Lat,
                        Lng = log.BeginLocation.Lng,
                        Address = log.BeginLocation.Address
                    },
                    Bs = device.Bs
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }


        /// <summary>
        ///     Lấy thông tin trạng thái máy
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceMachineGet GetMachine(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceMachineGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceMachineGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };

            var result = new DeviceMachineGet();
            result.Datas = new List<DeviceMachineLogTranfer>();

            var nowLog =
                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.BeginTime >= begin
                                && m.BeginTime <= end && m.Type == TraceType.Machine)
                    //.Take(30000).Execute();
                    .Execute();

            var oldLog =
                GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
                    .Where(m => m.Type == TraceType.Machine);

            var allData = new List<DeviceTraceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);

            foreach (var log in allData.OrderBy(m => m.BeginTime))
            {
                result.Datas.Add(new DeviceMachineLogTranfer
                {
                    BeginTime = log.BeginTime,
                    EndTime = log.EndTime,
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
                    },
                    Bs = device.Bs,
                    Over = log.EndTime - log.BeginTime,
                    Serial = device.Serial
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     Lấy thông tin nhiệt độ
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceTemperLogGet GetTemper(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceTemperLogGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceTemperLogGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };

            var result = new DeviceTemperLogGet();
            var tmp = new DeviceLog();
            result.Datas = new List<DeviceTemperLogTranfer>();
            var nowLog =
                DataContext.CreateQuery<DeviceLog>(company.DbId)
                    .Where(m => m.Indentity == device.Indentity && m.DeviceStatus.ClientSend >= begin
                                && m.DeviceStatus.ClientSend <= end).Select(new Expression<Func<DeviceLog, object>>[]
                                {
                                    m => m.DeviceStatus.ClientSend,
                                    m => m.DeviceStatus.Temperature,
                                    m => m.DeviceStatus.TotalGpsDistance
                                }, new Expression<Func<object>>[]
                                {
                                    () => tmp.DeviceStatus.ClientSend,
                                    () => tmp.DeviceStatus.Temperature,
                                    () => tmp.DeviceStatus.TotalGpsDistance
                                })
                    .Take(30000).Execute();
            var oldLog =
                GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);
            var allData = new List<DeviceLog>();
            allData.AddRange(oldLog);
            allData.AddRange(nowLog);
            foreach (var log in allData.OrderBy(m => m.DeviceStatus.ClientSend))
            {
                //var dr = Cache.GetQueryContext<Driver>().GetByKey(log.DriverId);
                result.Datas.Add(new DeviceTemperLogTranfer
                {
                    Distance = log.DeviceStatus.TotalCurrentGpsDistance,
                    Time = log.DeviceStatus.ClientSend,
                    Temper = log.DeviceStatus.Temperature
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin nhiên liệu
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="checkFlag"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceFuelLogGet GetFuel(long serial, DateTime begin, DateTime end, bool checkFlag=false)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceFuelLogGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceFuelLogGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };

            var result = new DeviceFuelLogGet();
            var tmp = new DeviceLog();
            result.Datas = new List<DeviceFuelLogTranfer>();

            var nowLog =
                DataContext.CreateQuery<DeviceLog>(company.DbId)
                .Where(m => m.Indentity == device.Indentity && m.DeviceStatus.ClientSend >= begin
                    && m.DeviceStatus.ClientSend <= end)
                    //.Take(30000).Execute();
                    .Execute();

            var oldLog =
                GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);

            var allData = new List<DeviceLog>();

            allData.AddRange(oldLog);
            allData.AddRange(nowLog);

            List<DeviceLog> ret = allData.OrderBy(m => m.DeviceStatus.ClientSend).ToList();
            if (ret.Count > 0)
            {
                DeviceLog devfirst = ret.FirstOrDefault();
                foreach (var log in allData.OrderBy(m => m.DeviceStatus.ClientSend))
                {
                    if(!checkFlag || log.DeviceStatus.UseFuel)
                        result.Datas.Add(new DeviceFuelLogTranfer
                        {
                            Time = log.DeviceStatus.ClientSend,
                            Distance = log.DeviceStatus.TotalGpsDistance - devfirst.DeviceStatus.TotalGpsDistance,
                            Fuel = log.DeviceStatus.Fuel
                        });
                }
            }

            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        /// <summary>
        ///     lấy thông tin nhiên liệu
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="checkFlag"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceFuelLogGet GetFuelSQL(long serial, DateTime begin, DateTime end, bool checkFlag = false)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new DeviceFuelLogGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new DeviceFuelLogGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };

            var result = new DeviceFuelLogGet();
            var tmp = new DeviceLog();
            result.Datas = new List<DeviceFuelLogTranfer>();

            var nowLog = new List<DeviceLog>();
            String SQL = $@"SELECT * FROM DeviceLog WHERE Indentity = '{device.Indentity}' AND DeviceStatus_ClientSend >= '{begin}' AND DeviceStatus_ClientSend <= '{end}'";
            DataContext.CustomHandle<NHibernate.ISession>(m => {
                try
                {
                    NHibernate.ISQLQuery query = m.CreateSQLQuery(SQL);
                    IList<dynamic> list = query.DynamicList();
                    if (list != null)
                    {
                        foreach (var obj in list)
                        {
                            DeviceLog acc = new DeviceLog();
                            acc.Id = obj.Id;
                            acc.DbId = obj.DbId;
                            acc.Serial = obj.Serial;
                            acc.Indentity = obj.Indentity;
                            acc.GroupId = obj.GroupId;
                            acc.CompanyId = obj.CompanyId;

                            acc.DeviceStatus = new Model.Components.DeviceStatusInfo();
                            acc.DeviceStatus.ClientSend = obj.DeviceStatus_ClientSend;
                            acc.DeviceStatus.ServerRecv = obj.DeviceStatus_ServerRecv;
                            acc.DeviceStatus.GpsStatus = obj.DeviceStatus_GpsStatus;
                            acc.DeviceStatus.TotalGpsDistance = obj.DeviceStatus_TotalGpsDistance;
                            acc.DeviceStatus.TotalCurrentGpsDistance = obj.DeviceStatus_TotalCurrentGpsDistance;
                            acc.DeviceStatus.Fuel = obj.DeviceStatus_Fuel;
                            acc.DeviceStatus.Temperature = obj.DeviceStatus_Temperature;
                            acc.DeviceStatus.GsmSignal = obj.DeviceStatus_GsmSignal;
                            acc.DeviceStatus.Power = obj.DeviceStatus_Power;
                            acc.DeviceStatus.Machine = obj.DeviceStatus_Machine;
                            acc.DeviceStatus.AirMachine = obj.DeviceStatus_AirMachine;
                            acc.DeviceStatus.Sos = obj.DeviceStatus_Sos;
                            acc.DeviceStatus.UseTemperature = obj.DeviceStatus_UseTemperature;
                            acc.DeviceStatus.UseFuel = obj.DeviceStatus_UseFuel;
                            acc.DeviceStatus.UseRfid = obj.DeviceStatus_UseRfid;
                            acc.DeviceStatus.Door = obj.DeviceStatus_Door;
                            acc.DeviceStatus.SpeedTrace = obj.DeviceStatus_SpeedTrace;
                            acc.DeviceStatus.Angle = obj.DeviceStatus_Angle;
                            acc.DeviceStatus.Speed = obj.DeviceStatus_Speed;

                            acc.DeviceStatus.GpsInfo = new Model.Components.GpsLocation();
                            acc.DeviceStatus.GpsInfo.Lat = obj.GpsInfo_Lat;
                            acc.DeviceStatus.GpsInfo.Lng = obj.GpsInfo_Lng;
                            if (!Object.ReferenceEquals(null, obj.GpsInfo_Address))
                                acc.DeviceStatus.GpsInfo.Address = obj.GpsInfo_Address;

                            acc.DriverStatus = new Model.Components.DriverStatusInfo();
                            acc.DriverStatus.DriverId = obj.DriverStatus_DriverId;
                            acc.DriverStatus.Name = obj.DriverStatus_Name;
                            acc.DriverStatus.Gplx = obj.DriverStatus_Gplx;
                            acc.DriverStatus.TimeBeginWorkInSession = obj.DriverStatus_TimeBeginWorkInSession;
                            acc.DriverStatus.TimeWorkInDay = obj.DriverStatus_TimeWorkInDay;
                            acc.DriverStatus.TimeWork = obj.DriverStatus_TimeWork;

                            nowLog.Add(acc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("GetTripSQL", ex, SQL);
                }
            }, company.DbId);

            var oldLog =
                GetZipLog<DeviceLog, DeviceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end);

            var allData = new List<DeviceLog>();

            allData.AddRange(oldLog);
            allData.AddRange(nowLog);

            List<DeviceLog> ret = allData.OrderBy(m => m.DeviceStatus.ClientSend).ToList();
            if (ret.Count > 0)
            {
                DeviceLog devfirst = ret.FirstOrDefault();
                foreach (var log in allData.OrderBy(m => m.DeviceStatus.ClientSend))
                {
                    if (!checkFlag || log.DeviceStatus.UseFuel)
                        result.Datas.Add(new DeviceFuelLogTranfer
                        {
                            Time = log.DeviceStatus.ClientSend,
                            Distance = log.DeviceStatus.TotalGpsDistance - devfirst.DeviceStatus.TotalGpsDistance,
                            Fuel = log.DeviceStatus.Fuel
                        });
                }
            }

            result.Status = 1;
            result.Description = "OK";
            return result;
        }

        #region Báo cáo tổng hợp

        //private List<EnterpriseDeviceAllReportTranfer> BuildAllReport(Device device, DateTime begin, DateTime end,
        //          IList<DeviceTraceLog> trace)
        //{
        //    // chia ngày ra
        //    var result = new List<EnterpriseDeviceAllReportTranfer>();

        //    Dictionary<DateTime, int> DistancebyDay = new Dictionary<DateTime, int>(0);
        //    double avgDistance = 0;
        //    for (DateTime beginday = begin.Date; beginday <= end.Date; beginday = beginday.AddDays(1))
        //    {
        //        var date = beginday;
        //        var en = beginday.AddDays(1).AddTicks(-1);
        //        var dairlyTotalMinutes = (int)Math.Round((en - date).TotalMinutes);//1440

        //        //nếu ngày đầu hoặc ngày cuối thì set lại ngày theo ngày chọn
        //        if (date.Date == begin.Date) date = begin;
        //        if (en.Date == end.Date) en = end;

        //        //lấy tất cả cuốc trong ngày, không phân biệt loại
        //        IList<DeviceTraceLog> onedaytrace = trace.Where(m => m.BeginTime >= date && m.BeginTime <= en).ToList();

        //        //khoảng cách đi được theo mở/tắt máy
        //        double TotalDistance = Math.Round(onedaytrace.Where(m => m.Type == TraceType.Machine).Sum(m => m.Distance) / 1000.0, 2);
        //        double Distance = TotalDistance;

        //        //tổng số phút chạy được trong ngày
        //        var run = (int)onedaytrace.Where(m => m.Type == TraceType.Machine && m.BeginTime.Date == m.EndTime.Date).Sum(m => (m.EndTime - m.BeginTime).TotalMinutes);

        //        //cộng với số phút của cuốc xuyên ngày trước đó (nếu có)
        //        if (DistancebyDay.ContainsKey(date))
        //        {
        //            run += DistancebyDay[date];
        //            DistancebyDay.Remove(date);

        //            //Tính lại khoảng cách đi được theo số phút
        //            Distance = Math.Round(avgDistance * run, 2);
        //        }

        //        //tìm cuốc xuyên ngày từ ngày này trỡ đi
        //        var more1daytrace = onedaytrace.FirstOrDefault(m => m.Type == TraceType.Machine && m.BeginTime.Date < m.EndTime.Date);

        //        if (more1daytrace != null)
        //        {
        //            //cộng với số phút của cuốc xuyên ngày trỡ đi
        //            run += (int)Math.Round((en - more1daytrace.BeginTime).TotalMinutes);

        //            //tìm khoãng cách trung bình theo phút
        //            var totalrun = (int)onedaytrace.Where(m => m.Type == TraceType.Machine).Sum(m => (m.EndTime - m.BeginTime).TotalMinutes);
        //            avgDistance = TotalDistance / totalrun;

        //            //Tính lại khoảng cách đi được theo số phút
        //            Distance = Math.Round(avgDistance * run, 2);

        //            //lưu thời gian cuốc xuyên ngày trỡ đi
        //            for (DateTime nextday = date.AddDays(1); nextday <= more1daytrace.EndTime.Date; nextday = nextday.AddDays(1))
        //            {
        //                int minutes = dairlyTotalMinutes;
        //                if (nextday == more1daytrace.EndTime.Date)
        //                    minutes = (int)Math.Round((more1daytrace.EndTime - more1daytrace.EndTime.Date).TotalMinutes);
        //                DistancebyDay[nextday] = minutes;
        //            }
        //        }

        //        EnterpriseDeviceAllReportTranfer obj = new EnterpriseDeviceAllReportTranfer
        //        {
        //            Bs = device.Bs,
        //            Date = date,
        //            Distance = Distance,
        //            OnAirMachineCount = onedaytrace.Count(m => m.Type == TraceType.AirMachine),
        //            PauseCount = onedaytrace.Count(m => m.Type == TraceType.Stop),
        //            DoorCount = onedaytrace.Count(m => m.Type == TraceType.Door),
        //            Type = (int)device.ActivityType,
        //            TimeRun = run,
        //            TimeStop = dairlyTotalMinutes - run,

        //            FuelQuotaKm = device.FuelQuotaKm,
        //            FuelQuoteHour = device.FuelQuoteHour
        //        };

        //        if (device.FuelQuotaKm > 0)
        //            obj.ValueQuotaKm = (float)(0.01 * device.FuelQuotaKm * obj.Distance);// /100

        //        if (device.FuelQuoteHour > 0)
        //            obj.ValueQuoteHour = obj.TimeRun * device.FuelQuoteHour / 60;

        //        result.Add(obj);

        //        onedaytrace.Clear(); onedaytrace = null;
        //    }

        //    return result;
        //}
        //
        //        /// <summary>
        //        /// Lấy báo cáo theo seriallist , nhóm hoặc cty
        //        /// </summary>
        //        /// <param name="companyId">Mã công ty</param>
        //        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        //        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        //        /// <param name="begin"></param>
        //        /// <param name="end"></param>
        //        /// <returns></returns>
        //        private EnterpriseDeviceAllReportGet GetAllReport(long companyId, long groupId,string seriallist, DateTime begin, DateTime end)
        //        {
        //            var company = Cache.GetCompanyById(companyId);
        //            if (company == null) return new EnterpriseDeviceAllReportGet { Description = "Công ty chưa thiết bị không tồn tại" };

        //            IList<Device> allDevice;
        //            List<DeviceTraceLog> alltrace = new List<DeviceTraceLog>(0);
        //            List<GeneralReportLog> allgeneral= new List<GeneralReportLog>();

        //#if TIME_DEBUG
        //            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //            sw.Start();
        //#endif

        //            //Tìm theo danh sách serial list
        //            if (!String.IsNullOrWhiteSpace(seriallist))
        //            {
        //                var allSeial = new List<long>();
        //                try
        //                {
        //                    allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
        //                }
        //                catch (Exception)
        //                {
        //                    return new EnterpriseDeviceAllReportGet { Description = "Thông tin seriallist truyền lên không hợp lệ" };
        //                }
        //                if (allSeial.Count == 0) return new EnterpriseDeviceAllReportGet { Description = "Danh sách serial rỗng" };
        //                allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();

        //                if (allDevice.Count == 1) //optimize performance 
        //                {
        //                    // lấy thông tin từ bảng zip 
        //                    var oldTrace = GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId
        //                            , m => m.Indentity == allDevice[0].Indentity
        //                            , begin, end);

        //                    //lấy thông tin bảng hiện hành
        //                    //var newTrace =
        //                    //    DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
        //                    //        .Where(m => m.BeginTime >= begin &&
        //                    //                m.EndTime <= end &&
        //                    //                m.Indentity == allDevice[0].Indentity).Execute();

        //                    //optimize from 9000ms to 333ms for this query. I don't know why 
        //                    var newTrace = new List<DeviceTraceLog>(0);
        //                    DataContext.CustomHandle<NHibernate.ISession>(m =>
        //                    {
        //                        try
        //                        {
        //                            String SQL = $"select * from DeviceTraceLog where BeginTime >= '{begin}' and BeginTime <= '{end}' and Indentity = '{allDevice[0].Indentity}' and ([Type] = 'Stop' OR [Type] = 'AirMachine' OR [Type] = 'Machine' OR [Type] = 'Door')";
        //                            NHibernate.ISQLQuery query = m.CreateSQLQuery(SQL);
        //                            //var rets = query.List<DeviceTraceLog>();
        //                            //if (rets != null) alltrace = rets.ToList();
        //                            var rets = query.DynamicList();
        //                            if (rets != null)
        //                            {
        //                                alltrace = new List<DeviceTraceLog>(rets.Count);
        //                                for (int i = 0; i < rets.Count; i++)
        //                                {
        //                                    DeviceTraceLog obj = new DeviceTraceLog();

        //                                    obj.Id = rets[i].Id;
        //                                    obj.BeginTime = rets[i].BeginTime;
        //                                    obj.EndTime = rets[i].EndTime;

        //                                    TraceType ttype;
        //                                    if (Enum.TryParse(rets[i].Type, out ttype))
        //                                        obj.Type = ttype;

        //                                    obj.Indentity = rets[i].Indentity;
        //                                    obj.Serial = rets[i].Serial;

        //                                    obj.DriverId = rets[i].DriverId;
        //                                    obj.Note = rets[i].Note;
        //                                    obj.DbId = rets[i].DbId;
        //                                    obj.Distance = rets[i].Distance;

        //                                    obj.GroupId = rets[i].GroupId;
        //                                    obj.CompanyId = rets[i].CompanyId;

        //                                    //obj.BeginLocation = rets[i].BeginLocation;
        //                                    //obj.EndLocation = rets[i].EndLocation;

        //                                    //alltrace.Add(obj);
        //                                    newTrace.Add(obj);
        //                                }
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Log.Exception("GetAllReport", ex, "query CustomHandle DeviceTraceLog");
        //                        }

        //                    }, company.DbId);

        //                    alltrace.AddRange(oldTrace);
        //                    alltrace.AddRange(newTrace);
        //                }
        //                else
        //                {
        //                    // lấy thông tin từ bảng zip
        //                    var oldTrace = GetZipLogWhereOr<DeviceTraceLog, DeviceTraceZip>(company.DbId,
        //                        allDevice.Select(m => (Expression<Func<DeviceTraceZip, bool>>)(x => x.Indentity == m.Indentity))
        //                            .ToArray()
        //                        , begin, end);

        //                    //lấy thông tin bảng hiện hành
        //                    var newTrace =
        //                        DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
        //                            .Where(m => m.BeginTime >= begin &&
        //                                        m.BeginTime <= end)
        //                            .WhereOr(
        //                                allDevice.Select(m => (Expression<Func<DeviceTraceLog, bool>>)(x => x.Indentity == m.Indentity))
        //                                    .ToArray()
        //                            ).Execute();

        //                    alltrace.AddRange(oldTrace);
        //                    alltrace.AddRange(newTrace);
        //                }

        //                //from GeneralReportLog
        //                allgeneral.AddRange(
        //                    DataContext.CreateQuery<GeneralReportLog>(company.DbId)
        //                   .Where(m => m.UpdateTime >= begin &&
        //                               m.UpdateTime <= end)
        //                   .WhereOr(
        //                       allDevice.Select(m => (Expression<Func<GeneralReportLog, bool>>)(x => x.GuidId == m.Indentity))
        //                           .ToArray()
        //                   ).Execute());

        //            }
        //            //Tìm theo nhóm
        //            else if(groupId>0)
        //            {
        //                allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);

        //                // lấy thông tin từ bảng zip 
        //                var oldTrace = GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId
        //                        , m => m.GroupId == groupId && m.CompanyId == companyId
        //                        , begin, end);

        //                //lấy thông tin bảng hiện hành
        //                var newTrace =
        //                   DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
        //                       .Where(m => m.BeginTime >= begin &&
        //                                   m.BeginTime <= end &&
        //                                   m.GroupId == groupId &&
        //                                   m.CompanyId == companyId
        //                                   ).Execute();

        //                alltrace.AddRange(oldTrace);
        //                alltrace.AddRange(newTrace);

        //                //from GeneralReportLog
        //                allgeneral.AddRange(
        //                   DataContext.CreateQuery<GeneralReportLog>(company.DbId)
        //                       .Where(m => m.UpdateTime >= begin &&
        //                                m.UpdateTime <= end &&
        //                                m.CompanyId == companyId &&
        //                                m.GroupId == groupId
        //                                ).Execute());
        //            }
        //            //Tìm theo cty
        //            else
        //            {
        //                allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);

        //                // lấy thông tin từ bảng zip 
        //                var oldTrace = GetZipLog<DeviceTraceLog, DeviceTraceZip>(company.DbId
        //                        , m => m.CompanyId == companyId
        //                        , begin, end);

        //                //lấy thông tin bảng hiện hành
        //                var tmp = new DeviceTraceLog();
        //                var newTrace =
        //                                DataContext.CreateQuery<DeviceTraceLog>(company.DbId)
        //                                    .Where(m => m.BeginTime >= begin &&
        //                                                m.BeginTime <= end &&
        //                                                m.CompanyId == companyId
        //                                                ).Select(new Expression<Func<DeviceTraceLog, object>>[] {
        //                                                    m => m.BeginTime,
        //                                                    m => m.EndTime,
        //                                                    m => m.Type,
        //                                                    m => m.Indentity,
        //                                                    m => m.Distance
        //                                                }
        //                                                , new Expression<Func<object>>[] {
        //                                                    () => tmp.BeginTime,
        //                                                    () => tmp.EndTime,
        //                                                    () => tmp.Type,
        //                                                    () => tmp.Indentity,
        //                                                    () => tmp.Distance
        //                                                }).Execute();

        //                alltrace.AddRange(oldTrace);
        //                alltrace.AddRange(newTrace);


        //                //from GeneralReportLog
        //                allgeneral.AddRange(
        //                  DataContext.CreateQuery<GeneralReportLog>(company.DbId)
        //                      .Where(m => m.UpdateTime >= begin &&
        //                               m.UpdateTime <= end &&
        //                               m.CompanyId == companyId
        //                               ).Execute());
        //            }

        //            //lấy GeneralReportLog trong ngày
        //            if (end.Date >= DateTime.Now.Date)
        //            {
        //                foreach (var device in allDevice)
        //                    if (device.Temp.GeneralReportLog != null)
        //                        allgeneral.Add(device.Temp.GeneralReportLog);
        //            }


        //#if TIME_DEBUG
        //            sw.Stop();
        //            Log.Warning("GetAllReport", $"DeviceTraceLog {sw.ElapsedMilliseconds} {alltrace.Count}");
        //#endif

        //            var result = new EnterpriseDeviceAllReportGet();
        //            result.Status = 1;
        //            result.Description = "OK";
        //            result.Datas = new List<EnterpriseDeviceAllReportTranfer>();

        //#if TIME_DEBUG
        //            sw.Reset();sw.Start();
        //#endif

        //            //foreach (var device in allDevice)
        //            //{
        //            //    result.Datas.AddRange(
        //            //        BuildAllReport(
        //            //            device, 
        //            //            begin, 
        //            //            end,
        //            //            alltrace.Where(m => m.Indentity == device.Indentity).ToList());
        //            //}


        //            foreach (var device in allDevice)
        //            {
        //                List<GeneralReportLog> logs = allgeneral.FindAll(m => m.GuidId == device.Indentity).OrderBy(m => m.UpdateTime).ToList();
        //                foreach (GeneralReportLog log in logs)
        //                {
        //                    EnterpriseDeviceAllReportTranfer obj = new EnterpriseDeviceAllReportTranfer()
        //                    {
        //                        Bs = device.Bs,
        //                        Date = log.UpdateTime.Date,
        //                        Distance = Math.Round(log.KmOnDay/1000.0,2),
        //                        Type = (int)device.ActivityType,
        //                        TimeRun = log.OverTimeInday,
        //                        TimeStop = 1440 - log.OverTimeInday,

        //                        FuelQuotaKm = device.FuelQuotaKm,
        //                        FuelQuoteHour = device.FuelQuoteHour,

        //                        OnAirMachineCount = log.OnAirMachineCount,//this is new field, you must collect for old data.
        //                        PauseCount = log.PauseCount,
        //                        DoorCount = log.OpenDoorCount
        //                    };

        //                    if (obj.TimeStop < 0) obj.TimeStop = 0;

        //                    if (device.FuelQuotaKm > 0)
        //                        obj.ValueQuotaKm = (float)(0.01 * device.FuelQuotaKm * obj.Distance);// /100

        //                    if (device.FuelQuoteHour > 0)
        //                        obj.ValueQuoteHour = obj.TimeRun * device.FuelQuoteHour / 60;

        //                    result.Datas.Add(obj);
        //                }
        //            }


        //#if TIME_DEBUG
        //            sw.Stop();
        //            Log.Warning("GetAllReport", "BuildAllReport " + sw.ElapsedMilliseconds.ToString());
        //#endif

        //            return result;
        //        }

        /// <summary>
        /// Lấy báo cáo theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private EnterpriseDeviceAllReportGet GetAllReport(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new EnterpriseDeviceAllReportGet { Description = "Công ty chưa thiết bị không tồn tại" };

            IList<Device> allDevice;
            List<GeneralReportLog> allgeneral = new List<GeneralReportLog>();

#if TIME_DEBUG
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif

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
                    return new EnterpriseDeviceAllReportGet { Description = "Thông tin seriallist truyền lên không hợp lệ" };
                }
                if (allSeial.Count == 0) return new EnterpriseDeviceAllReportGet { Description = "Danh sách serial rỗng" };
                allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();

                //from GeneralReportLog
                allgeneral.AddRange(
                    DataContext.CreateQuery<GeneralReportLog>(company.DbId)
                   .Where(m => m.UpdateTime >= begin &&
                               m.UpdateTime <= end)
                   .WhereOr(
                       allDevice.Select(m => (Expression<Func<GeneralReportLog, bool>>)(x => x.GuidId == m.Indentity))
                           .ToArray()
                   ).Execute());

            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                //from GeneralReportLog
                allgeneral.AddRange(
                   DataContext.CreateQuery<GeneralReportLog>(company.DbId)
                       .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end &&
                                m.CompanyId == companyId &&
                                m.GroupId == groupId
                                ).Execute());
            }
            //Tìm theo cty
            else
            {
                allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                //from GeneralReportLog
                allgeneral.AddRange(
                  DataContext.CreateQuery<GeneralReportLog>(company.DbId)
                      .Where(m => m.UpdateTime >= begin &&
                               m.UpdateTime <= end &&
                               m.CompanyId == companyId
                               ).Execute());
            }

            //lấy GeneralReportLog trong ngày
            if (end.Date >= DateTime.Now.Date)
            {
                foreach (var device in allDevice)
                    if (device.Temp.GeneralReportLog != null)
                        allgeneral.Add(device.Temp.GeneralReportLog);
            }


#if TIME_DEBUG
            sw.Stop();
            Log.Warning("GetAllReport", $"DeviceTraceLog {sw.ElapsedMilliseconds} {alltrace.Count}");
#endif

            var result = new EnterpriseDeviceAllReportGet();
            result.Status = 1;
            result.Description = "OK";
            result.Datas = new List<EnterpriseDeviceAllReportTranfer>();

#if TIME_DEBUG
            sw.Reset();sw.Start();
#endif

            foreach (var device in allDevice)
            {
                List<GeneralReportLog> logs = allgeneral.FindAll(m => m.GuidId == device.Indentity).OrderBy(m => m.UpdateTime).ToList();
                foreach (GeneralReportLog log in logs)
                {
                    EnterpriseDeviceAllReportTranfer obj = new EnterpriseDeviceAllReportTranfer()
                    {
                        Bs = device.Bs,
                        Date = log.UpdateTime.Date,
                        Distance = Math.Round((log.KmOnDay / 1000.0), 2),
                        Type = (int)device.ActivityType,
                        TimeRun = log.OverTimeInday,
                        TimeStop = 1440 - log.OverTimeInday,

                        FuelQuotaKm = device.FuelQuotaKm,
                        FuelQuoteHour = device.FuelQuoteHour,

                        OnAirMachineCount = log.OnAirMachineCount,//this is new field, you must collect for old data.
                        PauseCount = log.PauseCount,
                        DoorCount = log.OpenDoorCount
                    };

                    if (obj.TimeStop < 0) obj.TimeStop = 0;

                    if (device.FuelQuotaKm > 0)
                        obj.ValueQuotaKm = (float)(0.01 * device.FuelQuotaKm * obj.Distance);// /100

                    if (device.FuelQuoteHour > 0)
                        obj.ValueQuoteHour = obj.TimeRun * device.FuelQuoteHour / 60;

                    result.Datas.Add(obj);
                }
            }


#if TIME_DEBUG
            sw.Stop();
            Log.Warning("GetAllReport", "BuildAllReport " + sw.ElapsedMilliseconds.ToString());
#endif

            return result;
        }

        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp theo nhóm và công ty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceAllReportGet GetAllReport(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if(serial>0)
                return GetAllReport(companyId, groupId, serial.ToString(), begin, end);
            else if(!String.IsNullOrWhiteSpace(ids))
                return GetAllReport(companyId, groupId,ids, begin, end);
            else
                return GetAllReport(companyId, groupId,"", begin, end);
        }


        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp theo id
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceAllReportGet GetAllReportBySerial(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new EnterpriseDeviceAllReportGet { Description = "Không tồn tại thiết bị này" };
            return GetAllReport(device.CompanyId, 0, serial.ToString(), begin, end);
        }

#endregion Báo cáo tổng hợp


#region báo cáo theo điểm

        /// <summary>
        /// Lấy danh sách xe hiện hành đang trong Điểm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <param name="pointIds">nếu không có thì không lọc</param>
        /// <returns></returns>
        private List<PointTraceLog> GetCurrentPoints(long companyId, long groupId, params int[] pointIds)
        {
            IList<Device> devs = null;

            if (groupId == 0)
                devs = Cache.GetQueryContext<Device>().GetByCompany(companyId);
            else
                devs = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);

            if (devs != null)
            {
                if (pointIds.Length > 0)
                    return devs.Where(m => pointIds.Contains(m.Temp.IdPoint)).Select(m => new PointTraceLog()
                    {
                        PointId = m.Temp.IdPoint,
                        BeginTime = m.Temp.PointBeginTime,
                        EndTime = DateTimeFix.Max,
                        CompanyId = m.CompanyId,
                        GroupId = m.GroupId,
                        DeviceId = m.Serial,
                        Id = 0,
                        //DbId = m.
                        DriverId = m.Status?.DriverStatus?.DriverId ?? 0,
                    }).ToList();
                else
                    return devs.Where(m => m.Temp.IdPoint > 0).Select(m => new PointTraceLog()
                    {
                        PointId = m.Temp.IdPoint,
                        BeginTime = m.Temp.PointBeginTime,
                        EndTime = DateTimeFix.Max,
                        CompanyId = m.CompanyId,
                        GroupId = m.GroupId,
                        DeviceId = m.Serial,
                        Id = 0,
                        //DbId = m.
                        DriverId = m.Status?.DriverStatus?.DriverId ?? 0,
                    }).ToList();
            }
            return new List<PointTraceLog>(0);
        }


        /// <summary>
        /// Lấy báo cáo theo list , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="idlist">serial, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private PointReportGet GetPointReport(long companyId, long groupId, string idlist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new PointReportGet { Description = "Công ty chưa thiết bị không tồn tại" };

            IList<PointGps> points;
            List<PointTraceLog> alltrace = new List<PointTraceLog>();

            //Tìm theo danh sách serial list
            if (!String.IsNullOrWhiteSpace(idlist))
            {
                var allId = new List<int>();
                try
                {
                    allId = idlist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => int.Parse(m.Trim())).ToList();
                }
                catch (Exception)
                {
                    return new PointReportGet { Description = "Thông tin list truyền lên không hợp lệ" };
                }
                if (allId.Count == 0) return new PointReportGet { Description = "Danh sách id rỗng" };

                points = allId.Select(m => Cache.GetQueryContext<PointGps>().GetByKey(m)).Where(m => m != null).ToList();

                //đếm xe hiện hành
                if (end > DateTime.Now)
                    alltrace.AddRange(GetCurrentPoints(companyId, groupId, allId.ToArray()));

                //đếm xe trong sự kiện
                alltrace.AddRange(
                   DataContext.CreateQuery<PointTraceLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin && m.EndTime <= end)
                       .WhereOr(
                           points.Select(m => (Expression<Func<PointTraceLog, bool>>)(x => x.PointId == m.Id)).ToArray())
                       .Execute());

                //alltrace.AddRange(
                //    GetZipLogWhereOr<PointTraceLog, PointTraceLogZip>(company.DbId,
                //        point.Select(m => (Expression<Func<PointTraceLogZip, bool>>)(x => x.PointId == m.Id)).ToArray(),
                //        begin, end));
            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                points = Cache.GetQueryContext<PointGps>().GetByGroup(companyId, groupId);

                //đếm xe hiện hành
                if (end > DateTime.Now)
                    alltrace.AddRange(GetCurrentPoints(companyId, groupId));

                //đếm xe trong sự kiện
                alltrace.AddRange(
                   DataContext.CreateQuery<PointTraceLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin &&
                                   m.EndTime <= end &&
                                   m.GroupId == groupId &&
                                   m.CompanyId == companyId).Execute());
            }
            //Tìm theo cty
            else
            {
                points = Cache.GetQueryContext<PointGps>().GetByCompany(companyId);

                //đếm xe hiện hành
                if (end > DateTime.Now)
                    alltrace.AddRange(GetCurrentPoints(companyId, groupId));

                //đếm xe trong sự kiện
                alltrace.AddRange(
                   DataContext.CreateQuery<PointTraceLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin &&
                                   m.EndTime <= end &&
                                   m.CompanyId == companyId).Execute());
            }

            var result = new PointReportGet();
            result.Status = 1;
            result.Description = "OK";
            result.Datas = new List<PointReportTranfer>();
            foreach (var p in points)
            {
                result.Datas.Add(new PointReportTranfer
                {
                    Name = p.Name,
                    CreateTime = p.CreateTime,
                    Type = p.Type,
                    Radius = p.Radius,
                    Lat = p.Location?.Lat ?? 0,
                    Lng = p.Location?.Lng ?? 0,
                    Address = p.Location?.Address ?? "",
                    DeviceCount = alltrace.Where(m => m.PointId == p.Id).Count()
                });
            }

            return result;
        }

        /// <summary>
        ///     Thống kê Tổng hợp xe Hiện Hành nằm trong Điểm
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách mã điểm cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="id">mã điểm, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public PointReportGet GetCurrentPointReport(long companyId,long groupId, string ids, int id)
        {
            if (id > 0)
                return GetPointReport(companyId, groupId, id.ToString(), DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetPointReport(companyId, groupId, ids, DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
            else
                return GetPointReport(companyId, groupId, "", DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
        }


        /// <summary>
        ///     Thống kê Tổng hợp xe nằm trong Điểm
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách mã điểm cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="id">mã điểm, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public PointReportGet GetPointReport(long companyId, DateTime begin, DateTime end, long groupId, string ids, int id)
        {
            if (id > 0)
                return GetPointReport(companyId, groupId, id.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetPointReport(companyId, groupId, ids, begin, end);
            else
                return GetPointReport(companyId, groupId, "", begin, end);
        }

        /// <summary>
        /// Lấy thông tin chi tiết xe trong phạm vi bán kính Điểm
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="pointId">id Điểm, nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public PointSessionGet GetPointSession(long companyId, long groupId, long pointId, long serial, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null)
                return new PointSessionGet { Description = "Công ty này không tồn tại" };

            List<PointTraceLog> ret = new List<PointTraceLog>();

            if (pointId > 0 && serial > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<PointTraceLog>(company.DbId)
                                   .Where(m => m.BeginTime >= begin
                                               && m.EndTime <= end
                                               && m.PointId == pointId
                                               && m.DeviceId == serial
                                               )
                                   .Execute()
                                   );
            }
            else if (pointId > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<PointTraceLog>(company.DbId)
                                   .Where(m => m.BeginTime >= begin
                                               && m.EndTime <= end
                                               && m.PointId == pointId
                                               )
                                   .Execute()
                                   );
            }
            else if (serial > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<PointTraceLog>(company.DbId)
                                    .Where(m => m.BeginTime >= begin
                                                && m.EndTime <= end
                                                && m.DeviceId == serial
                                                )
                                    .Execute()
                                    );
            }
            else if (groupId > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<PointTraceLog>(company.DbId)
                                 .Where(m => m.BeginTime >= begin
                                             && m.EndTime <= end
                                             && m.GroupId == groupId
                                             )
                                 .Execute()
                                 );
            }
            else
            {
                ret.AddRange(
                    DataContext.CreateQuery<PointTraceLog>(company.DbId)
                    .Where(m => m.BeginTime >= begin
                        && m.EndTime <= end
                        && m.CompanyId == companyId
                                )
                    .Execute()
                    );
            }


            var result = new PointSessionGet();
            result.Datas = new List<PointSessionTranfer>();

            //var oldLog =
            //    GetZipLog<PointTraceLog, PointTraceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
            //        .Where(m => m.Type == TraceType.Machine);
            //allData.AddRange(oldLog);

            foreach (var log in ret.OrderBy(m => m.BeginTime))
            {
                var point = Cache.GetQueryContext<PointGps>().GetByKey(log.PointId);
                if (point == null) continue;
                var device = Cache.GetQueryContext<Device>().GetByKey(log.DeviceId);

                result.Datas.Add(new PointSessionTranfer
                {
                    Id = log.Id,
                    Bs = device?.Bs ?? "",
                    Name = point.Name,
                    Type = point.Type,
                    Lat = point.Location.Lat,
                    Lng = point.Location.Lng,
                    Address = point.Location.Address,
                    Note = point.Description,
                    BeginTime = log.BeginTime,
                    EndTime = log.EndTime,
                    TotalSeconds = (int)(log.EndTime - log.BeginTime).TotalSeconds
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }

#endregion báo cáo theo điểm


#region báo cáo theo vùng

        /// <summary>
        /// Lấy danh sách xe hiện hành đang trong Vùng
        /// </summary>
        /// <param name="companyId">Id Công ty</param>
        /// <param name="groupId"></param>
        /// <param name="areaIds">nếu không có thì không lọc</param>
        /// <returns></returns>
        private List<AreaTraceLog> GetCurrentArea(long companyId, long groupId,params int[] areaIds)
        {
            IList<Device> devs = null;
            if (groupId == 0)
                devs = Cache.GetQueryContext<Device>().GetByCompany(companyId);
            else
                devs = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);

            if (devs != null)
            {
                if(areaIds.Length>0)
                    return devs.Where(m => areaIds.Contains(m.Temp.IdArea) ).Select(m => new AreaTraceLog()
                    {
                        AreaId = m.Temp.IdArea,
                        BeginTime = m.Temp.AreaBeginTime,
                        EndTime = DateTimeFix.Max,
                        CompanyId = m.CompanyId,
                        GroupId = m.GroupId,
                        DeviceId = m.Serial,
                        Id = 0,
                        //DbId = m.
                        DriverId = m.Status?.DriverStatus?.DriverId ?? 0,
                    }).ToList();
                else
                    return devs.Where(m => m.Temp.IdArea > 0).Select(m => new AreaTraceLog()
                    {
                        AreaId = m.Temp.IdArea,
                        BeginTime = m.Temp.AreaBeginTime,
                        EndTime = DateTimeFix.Max,
                        CompanyId = m.CompanyId,
                        GroupId = m.GroupId,
                        DeviceId = m.Serial,
                        Id = 0,
                        //DbId = m.
                        DriverId = m.Status?.DriverStatus?.DriverId ?? 0,
                    }).ToList();

            }

            return new List<AreaTraceLog>(0);
        }


        /// <summary>
        /// Lấy báo cáo theo list , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="idlist">serial, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private AreaReportGet GetAreaReport(long companyId, long groupId, string idlist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new AreaReportGet { Description = "Công ty chưa thiết bị không tồn tại" };

            IList<Area> areas;
            List<AreaTraceLog> alltrace = new List<AreaTraceLog>();

            //Tìm theo danh sách serial list
            if (!String.IsNullOrWhiteSpace(idlist))
            {
                var allId = new List<int>();
                try
                {
                    allId = idlist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => int.Parse(m.Trim())).ToList();
                }
                catch (Exception)
                {
                    return new AreaReportGet { Description = "Thông tin list truyền lên không hợp lệ" };
                }
                if (allId.Count == 0) return new AreaReportGet { Description = "Danh sách id rỗng" };

                areas = allId.Select(m => Cache.GetQueryContext<Area>().GetByKey(m)).Where(m => m != null).ToList();

                //đếm xe hiện hành
                if (end > DateTime.Now)
                    alltrace.AddRange(GetCurrentArea(companyId, groupId, allId.ToArray()));

                //đếm xe trong sự kiện
                alltrace.AddRange(
                   DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin && m.EndTime <= end)
                       .WhereOr(
                           areas.Select(m => (Expression<Func<AreaTraceLog, bool>>)(x => x.AreaId == m.Id)).ToArray())
                       .Execute());
            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                areas = Cache.GetQueryContext<Area>().GetByGroup(companyId, groupId);

                //đếm xe hiện hành
                if (end > DateTime.Now)
                    alltrace.AddRange(GetCurrentArea(companyId, groupId));

                //đếm xe trong sự kiện
                alltrace.AddRange(
                   DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin &&
                                   m.EndTime <= end &&
                                   m.GroupId == groupId &&
                                   m.CompanyId == companyId).Execute());
            }
            //Tìm theo cty
            else
            {
                areas = Cache.GetQueryContext<Area>().GetByCompany(companyId);

                //đếm xe hiện hành
                if (end > DateTime.Now)
                    alltrace.AddRange(GetCurrentArea(companyId, groupId));

                //đếm xe trong sự kiện
                alltrace.AddRange(
                   DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                       .Where(m => m.BeginTime >= begin &&
                                   m.EndTime <= end &&
                                   m.CompanyId == companyId).Execute());
            }

            var result = new AreaReportGet();
            result.Status = 1;
            result.Description = "OK";
            result.Datas = new List<AreaReportTranfer>();
            foreach (var p in areas)
            {
                result.Datas.Add(new AreaReportTranfer
                {
                    Name = p.Name,
                    MaxSpeed = p.MaxSpeed,
                    MaxDevice = p.MaxDevice,
                    CreateTime = p.CreateTime,
                    Type = p.Type,
                    Points = p.Points,
                    DeviceCount = alltrace.Where(m => m.AreaId == p.Id).Count()
                });
            }

            return result;
        }


        /// <summary>
        ///     Thống kê Tổng hợp xe Hiện Hành nằm trong Vùng
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách mã điểm cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="id">mã điểm, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public AreaReportGet GetCurrentAreaReport(long companyId, long groupId, string ids, int id)
        {
            if (id > 0)
                return GetAreaReport(companyId, groupId, id.ToString(), DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetAreaReport(companyId, groupId, ids, DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
            else
                return GetAreaReport(companyId, groupId, "", DateTime.Now.AddDays(1), DateTime.Now.AddDays(1));
        }


        /// <summary>
        ///     Thống kê Tổng hợp xe nằm trong Vùng
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách mã điểm cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="id">mã điểm, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public AreaReportGet GetAreaReport(long companyId, DateTime begin, DateTime end, long groupId, string ids, int id)
        {
            if (id > 0)
                return GetAreaReport(companyId, groupId, id.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetAreaReport(companyId, groupId, ids, begin, end);
            else
                return GetAreaReport(companyId, groupId, "", begin, end);
        }


        /// <summary>
        /// Lấy thông tin chi tiết xe trong Vùng
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="areaId">id Vùng, nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AreaSessionGet GetAreaSession(long companyId,long groupId,long areaId,long serial, DateTime begin, DateTime end)
        {
            //var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            //if (device == null)
            //    return new AreaSessionGet { Description = "Không tồn tại thiết bị này" };
            var company = Cache.GetCompanyById(companyId);
            if (company == null)
                return new AreaSessionGet { Description = "Công ty này không tồn tại" };

            List<AreaTraceLog> ret = new List<AreaTraceLog>();

            if(areaId > 0 && serial>0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                                   .Where(m => m.BeginTime >= begin
                                               && m.EndTime <= end
                                               && m.AreaId == areaId
                                               && m.DeviceId == serial
                                               )
                                   //.Take(30000).Execute()
                                   .Execute()
                                   );
            }
            else if(areaId > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                                   .Where(m => m.BeginTime >= begin
                                               && m.EndTime <= end
                                               && m.AreaId == areaId
                                               )
                                   //.Take(30000).Execute()
                                   .Execute()
                                   );
            }
            else if (serial > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                                    .Where(m => m.BeginTime >= begin
                                                && m.EndTime <= end
                                                && m.DeviceId == serial
                                                )
                                    //.Take(30000).Execute()
                                    .Execute()
                                    );
            }
            else if(groupId>0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                                 .Where(m => m.BeginTime >= begin
                                             && m.EndTime <= end
                                             && m.GroupId == groupId
                                             )
                                 //.Take(30000).Execute()
                                 .Execute()
                                 );
            }
            else
            {
                ret.AddRange(
                    DataContext.CreateQuery<AreaTraceLog>(company.DbId)
                    .Where(m => m.BeginTime >= begin
                        && m.EndTime <= end
                        && m.CompanyId == companyId
                                )
                    //.Take(30000).Execute()
                    .Execute()
                    );
            }


            var result = new AreaSessionGet();
            result.Datas = new List<AreaSessionTranfer>();

            //var oldLog =
            //    GetZipLog<AreaTraceLog, AreaTraceLogZip>(company.DbId, m => m.Indentity == device.Indentity, begin, end)
            //        .Where(m => m.Type == TraceType.Machine);
            //allData.AddRange(oldLog);

            foreach (var log in ret.OrderBy(m => m.BeginTime))
            {
                var area = Cache.GetQueryContext<Area>().GetByKey(log.AreaId);
                if (area == null) continue;
                var device = Cache.GetQueryContext<Device>().GetByKey(log.DeviceId);

                result.Datas.Add(new AreaSessionTranfer
                {
                    Id = log.Id,
                    Bs = device?.Bs??"",
                    Name = area.Name,
                    Type = area.Type,
                    Address = area.Address,
                    Note = area.Description,
                    BeginTime = log.BeginTime,
                    EndTime = log.EndTime,
                    TotalSeconds = (int)(log.EndTime - log.BeginTime).TotalSeconds
                });
            }
            result.Status = 1;
            result.Description = "OK";
            return result;
        }


#endregion báo cáo theo vùng

#region báo cáo dầu

        /// <summary>
        /// Lấy thông tin chi tiết xe thay đổi nhiên liệu
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public FuelSessionGet GetFuelSession(long companyId, long groupId, string ids,long serial, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null)
                return new FuelSessionGet { Description = "Công ty này không tồn tại" };

            List<FuelTraceLog> ret = new List<FuelTraceLog>();

            if (serial > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<FuelTraceLog>(company.DbId)
                                    .Where(m => m.Time >= begin
                                                && m.Time <= end
                                                && m.DeviceId == serial
                                                )
                                    .Execute()
                                    );
            }
            else if (!String.IsNullOrWhiteSpace(ids))
            {
                var allSeial = new List<long>();
                try
                {
                    allSeial = ids.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                }
                catch (Exception)
                {
                    return new FuelSessionGet { Description = "Thông tin seriallist truyền lên không hợp lệ" };
                }
                if (allSeial.Count == 0) return new FuelSessionGet { Description = "Danh sách serial rỗng" };

                ret.AddRange(
                    DataContext.CreateQuery<FuelTraceLog>(company.DbId)
                    .Where(m => m.Time >= begin
                        && m.Time <= end)
                        .WhereOr(
                            allSeial.Select(m => (Expression<Func<FuelTraceLog, bool>>)(x => x.DeviceId == m))
                            .ToArray()
                    ).Execute());
            }
            else if (groupId > 0)
            {
                ret.AddRange(
                    DataContext.CreateQuery<FuelTraceLog>(company.DbId)
                                 .Where(m => m.Time >= begin
                                             && m.Time <= end
                                             && m.GroupId == groupId
                                             )
                                 .Execute()
                                 );
            }
            else
            {
                ret.AddRange(
                    DataContext.CreateQuery<FuelTraceLog>(company.DbId)
                    .Where(m => m.Time >= begin
                        && m.Time <= end
                        && m.CompanyId == companyId
                                )
                    .Execute()
                    );
            }


            var result = new FuelSessionGet();
            result.Datas = new List<FuelSessionTranfer>();

            foreach (var log in ret.OrderBy(m => m.Time))
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(log.DeviceId);

                var obj = new FuelSessionTranfer
                {
                    Id = log.Id,
                    Serial = device?.Serial ?? 0,
                    Bs = device?.Bs ?? "",
                    Type = log.Delta > 0 ? "Tăng" : "Giảm",
                    RemainValue = log.CurrentValue,
                    Lat = log.Location.Lat,
                    Lng = log.Location.Lng,
                    Address = log.Location.Address,
                    Time = log.Time
                };

                if(obj.RemainValue - log.Delta<0)
                {
                    obj.BeginValue = 0;
                    obj.ChangeValue = obj.RemainValue;
                }
                else
                {
                    obj.BeginValue = obj.RemainValue - log.Delta;
                    obj.ChangeValue = Math.Abs(log.Delta);
                }

                //obj.BeginValue = obj.RemainValue - log.Delta;
                //obj.ChangeValue = Math.Abs(log.Delta);

                result.Datas.Add(obj);
            }
            result.Status = 1;
            result.Description = "OK";


            return result;
        }

        /// <summary>
        /// Lấy báo cáo theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private EnterpriseDeviceFuelReportGet GetFuelReport(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new EnterpriseDeviceFuelReportGet { Description = "Công ty chưa thiết bị không tồn tại" };

            IList<Device> allDevice;
            List<GeneralReportLog> alltrace = new List<GeneralReportLog>();

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
                    return new EnterpriseDeviceFuelReportGet { Description = "Thông tin seriallist truyền lên không hợp lệ" };
                }
                if (allSeial.Count == 0) return new EnterpriseDeviceFuelReportGet { Description = "Danh sách serial rỗng" };
                allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();

                alltrace.AddRange(
                DataContext.CreateQuery<GeneralReportLog>(company.DbId)
                    .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end)
                    .WhereOr(
                        allDevice.Select(m => (Expression<Func<GeneralReportLog, bool>>)(x => x.GuidId == m.Indentity))
                            .ToArray()
                    ).Execute());
            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                alltrace.AddRange(
                   DataContext.CreateQuery<GeneralReportLog>(company.DbId)
                       .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end &&
                                m.CompanyId == companyId &&
                                m.GroupId == groupId
                                ).Execute());
            }
            //Tìm theo cty
            else
            {
                allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                alltrace.AddRange(
                   DataContext.CreateQuery<GeneralReportLog>(company.DbId)
                       .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end &&
                                m.CompanyId == companyId
                                ).Execute());
            }

            if (allDevice == null || allDevice.Count <= 0) return new EnterpriseDeviceFuelReportGet()
            {
                Status = 1,
                Description = "Không có dữ liệu",
                Datas = new List<EnterpriseDeviceFuelReportTranfer>(0)
            };

            //lấy thông tin trong ngày
            if (end.Date >= DateTime.Now.Date)
            {
                foreach (var device in allDevice)
                    if (device.Temp.GeneralReportLog != null)
                        alltrace.Add(device.Temp.GeneralReportLog);
            }

            var result = new EnterpriseDeviceFuelReportGet();
            result.Status = 1;
            result.Description = "OK";
            result.Datas = new List<EnterpriseDeviceFuelReportTranfer>();
            foreach (var device in allDevice)
            {
                List<GeneralReportLog> logs = alltrace.FindAll(m => m.GuidId == device.Indentity).OrderBy(m => m.UpdateTime).ToList();
                foreach (GeneralReportLog log in logs)
                {
                    EnterpriseDeviceFuelReportTranfer obj = new EnterpriseDeviceFuelReportTranfer()
                    {
                        Date = log.UpdateTime.Date,
                        Bs = device.Bs,
                        TimeRun = log.OverTimeInday,
                        Distance = log.KmOnDay,
                        BeginDateValue = log.BeginDateFuel,
                        AddValue = log.AddFuel,
                        LostValue = log.LostFuel,
                        RemainValue = log.RemainFuel,
                        //ConsumeValue = log.BeginDateFuel + log.AddFuel - log.LostFuel - log.RemainFuel,
                        //Value100KM = log.KmOnDay != 0 ? ((log.BeginDateFuel + log.AddFuel - log.LostFuel - log.RemainFuel) * 100 / log.KmOnDay) : 0,
                    };

                    obj.ConsumeValue = obj.BeginDateValue + obj.AddValue - obj.LostValue - obj.RemainValue;
                    if (obj.ConsumeValue < 0) obj.ConsumeValue = 0;
                    obj.Value100KM = (float)(obj.Distance > 0 ? (obj.ConsumeValue * 100 / obj.Distance) : 0);

                    if (obj.TimeRun > 0)
                        obj.ValueHour = obj.ConsumeValue * 60f / obj.TimeRun;

                    result.Datas.Add(obj);
                }
            }

            return result;
        }


        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp Nhiên Liệu
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceFuelReportGet GetFuelReport(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return GetFuelReport(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetFuelReport(companyId, groupId, ids, begin, end);
            else
                return GetFuelReport(companyId, groupId, "", begin, end);
        }

#endregion báo cáo dầu

#region báo cáo xe khách

        /// <summary>
        /// Lấy thông tin chi tiết cuốc taxi đón khách
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public GuestSessionGet GetGuestSession(long companyId, long groupId, string ids, long serial, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null)
                return new GuestSessionGet { Description = "Công ty này không tồn tại" };

            List<DeviceGuestLog> ret = new List<DeviceGuestLog>();

            Device checkdevice = null;
            if (serial > 0) checkdevice = Cache.GetQueryContext<Device>().GetByKey(serial);
            //if (serial > 0)
            if (checkdevice != null)
            {
                //current
                ret.AddRange(
                    DataContext.CreateQuery<DeviceGuestLog>(company.DbId)
                                    .Where( m => m.Indentity == checkdevice.Indentity
                                                && m.BeginTime >= begin
                                                && m.EndTime <= end
                                                //&& m.Serial == serial
                                                //&& (m.Type== TraceType.HasGuest || m.Type == TraceType.NoGuest)
                                                )
                                    .Execute()
                                    );
            }
            else if (!String.IsNullOrWhiteSpace(ids))
            {
                var allSeial = new List<long>();
                try
                {
                    allSeial = ids.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                }
                catch (Exception)
                {
                    return new GuestSessionGet { Description = "Thông tin seriallist truyền lên không hợp lệ" };
                }
                if (allSeial.Count == 0) return new GuestSessionGet { Description = "Danh sách serial rỗng" };

                //current
                ret.AddRange(
                    DataContext.CreateQuery<DeviceGuestLog>(company.DbId)
                    .Where(m => m.BeginTime >= begin 
                        && m.EndTime <= end
                        //&& (m.Type == TraceType.HasGuest || m.Type == TraceType.NoGuest )
                        )
                        .WhereOr(
                            allSeial.Select(m => (Expression<Func<DeviceGuestLog, bool>>)(x => x.Serial == m))
                            .ToArray()
                    ).Execute());

            }
            else if (groupId > 0)
            {
                //current
                ret.AddRange(
                    DataContext.CreateQuery<DeviceGuestLog>(company.DbId)
                                 .Where(m => m.BeginTime >= begin
                                                && m.EndTime <= end
                                                && m.GroupId == groupId
                                                //&& (m.Type == TraceType.HasGuest || m.Type == TraceType.NoGuest)
                                             )
                                 .Execute()
                                 );
            }
            else
            {
                //current
                ret.AddRange(
                    DataContext.CreateQuery<DeviceGuestLog>(company.DbId)
                    .Where(m => m.BeginTime >= begin
                                && m.EndTime <= end
                                && m.CompanyId == companyId
                                //&& (m.Type == TraceType.HasGuest || m.Type == TraceType.NoGuest)
                                )
                    .Execute()
                    );
            }


            var result = new GuestSessionGet();
            result.Datas = new List<GuestSessionTranfer>();

            //GuestSessionTranfer prevobj = null;
            foreach (var log in ret.OrderBy(m => m.BeginTime))
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(log.Serial);

                var obj = new GuestSessionTranfer
                {
                    Id = log.Id,
                    Serial = log.Serial,
                    Bs = device?.Bs ?? "",
                    HasGuest = log.Type == TraceType.HasGuest,
                    BeginTime = log.BeginTime,
                    BeginLocation = new GpsPoint() {
                        Lat = log.BeginLocation.Lat,
                        Lng = log.BeginLocation.Lng,
                        Address = log.BeginLocation.Address
                    },
                    EndTime = log.EndTime,
                    EndLocation = new GpsPoint() {
                        Lat = log.EndLocation.Lat,
                        Lng = log.EndLocation.Lng,
                        Address = log.EndLocation.Address
                    },

                    TimeSeconds = (int)Math.Round((log.EndTime - log.BeginTime).TotalSeconds),
                    DistanceKm = Math.Round(log.Distance / 1000.0,2),
                    Note = log.Note
                };

                if(device!=null && device.Status!=null && device.Status.DriverStatus!=null)
                {
                    var driver = Cache.GetQueryContext<Driver>().GetByKey(device.Status.DriverStatus.DriverId);
                    if(driver!=null)
                    {
                        obj.DriverId = driver.Id;
                        obj.DriverName = driver.Name;
                        obj.DriverPhone = driver.Phone;
                    }
                }

                ////kiểm tra và ghép cuốc < 60 giây
                //if (prevobj!=null && prevobj.HasGuest==obj.HasGuest
                //    && (obj.BeginTime - prevobj.EndTime).TotalSeconds < 60 
                //    )
                //{
                //    prevobj.EndLocation = obj.EndLocation;
                //    prevobj.EndTime = obj.EndTime;
                //    prevobj.TimeSeconds += obj.TimeSeconds;
                //    prevobj.DistanceKm += obj.DistanceKm;
                //    continue;
                //}

                result.Datas.Add(obj);
                //prevobj = obj;
            }
            result.Status = 1;
            result.Description = "OK";

            return result;
        }


        /// <summary>
        /// Lấy báo cáo theo seriallist , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private EnterpriseDeviceGuestReportGet GetGuestReport(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new EnterpriseDeviceGuestReportGet { Description = "Công ty chưa thiết bị không tồn tại" };

            IList<Device> allDevice;
            List<GeneralGuestLog> alltrace = new List<GeneralGuestLog>();

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
                    return new EnterpriseDeviceGuestReportGet { Description = "Thông tin seriallist truyền lên không hợp lệ" };
                }
                if (allSeial.Count == 0) return new EnterpriseDeviceGuestReportGet { Description = "Danh sách serial rỗng" };
                allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();

                alltrace.AddRange(
                DataContext.CreateQuery<GeneralGuestLog>(company.DbId)
                    .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end)
                    .WhereOr(
                        allDevice.Select(m => (Expression<Func<GeneralGuestLog, bool>>)(x => x.GuidId == m.Indentity))
                            .ToArray()
                    ).Execute());
            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                alltrace.AddRange(
                   DataContext.CreateQuery<GeneralGuestLog>(company.DbId)
                       .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end &&
                                m.CompanyId == companyId && 
                                m.GroupId==groupId
                                ).Execute());
            }
            //Tìm theo cty
            else
            {
                allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                alltrace.AddRange(
                   DataContext.CreateQuery<GeneralGuestLog>(company.DbId)
                       .Where(m => m.UpdateTime >= begin &&
                                m.UpdateTime <= end &&
                                m.CompanyId == companyId
                                ).Execute());
            }

            //lấy thông tin trong ngày
            if (end.Date >= DateTime.Now.Date)
            {
                foreach (var device in allDevice)
                    if (device.Temp.GeneralGuestLog != null)
                        alltrace.Add(device.Temp.GeneralGuestLog);
            }

            if (allDevice == null || allDevice.Count <= 0) return new EnterpriseDeviceGuestReportGet()
            {
                Status = 1,
                Description = "Không có dữ liệu",
                Datas = new List<EnterpriseDeviceGuestReportTranfer>(0)
            };

            var result = new EnterpriseDeviceGuestReportGet();
            result.Status = 1;
            result.Description = "OK";
            result.Datas = new List<EnterpriseDeviceGuestReportTranfer>();
            foreach (var device in allDevice)
            {
                List<GeneralGuestLog> logs = alltrace.FindAll(m => m.GuidId == device.Indentity).OrderBy(m => m.UpdateTime).ToList();
                foreach (GeneralGuestLog log in logs)
                {
                    var obj =
                        new EnterpriseDeviceGuestReportTranfer()
                        {
                            Serial = device.Serial,
                            Date = log.UpdateTime.Date,
                            Bs = device.Bs,
                            HasGuestKm = Math.Round(log.KmGuestOnDay / 1000.0, 2),
                            NoGuestKm = Math.Round(log.KmNoGuestOnDay / 1000.0, 2),
                            HasGuestMinutes = log.GuestTimeInday / 60,
                            NoGuestMinutes = log.NoGuestTimeInday / 60,
                            Note = log.Note
                        };

                    //Tìm trong cuốc đặc biệt
                    //SpecialTour specialobj = DataContext.GetWhere<SpecialTour>(m =>
                    //    m.Serial == device.Serial
                    //    && m.Date >= log.UpdateTime
                    //    && m.Date <= log.UpdateTime.AddDays(1)
                    //    , company.DbId).FirstOrDefault();
                    List<SpecialTour> specialobjs = DataContext.GetWhere<SpecialTour>(m =>
                        m.Serial == device.Serial
                        && m.Date >= log.UpdateTime
                        && m.Date <= log.UpdateTime.AddDays(1)
                        , company.DbId).ToList();

                    if (specialobjs != null && specialobjs.Count>0)
                    {
                        obj.HasSpecial = true;
                        obj.SpecialNote = String.Join(",", specialobjs.Select(m => m.Address));
                    }

                    if (device.Status.DriverStatus != null)
                    {
                        var driver = Cache.GetQueryContext<Driver>().GetByKey(device.Status.DriverStatus.DriverId);
                        if (driver != null)
                        {
                            obj.DriverId = driver.Id;
                            obj.DriverName = driver.Name;
                            obj.DriverPhone = driver.Phone;
                        }
                    }

                    result.Datas.Add(obj);

                }
            }

            return result;
        }


        /// <summary>
        ///     Lấy thông tin Tổng hợp báo cáo đón khách
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceGuestReportGet GetGuestReport(long companyId, DateTime begin, DateTime end, long groupId, string ids, long serial)
        {
            if (serial > 0)
                return GetGuestReport(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(ids))
                return GetGuestReport(companyId, groupId, ids, begin, end);
            else
                return GetGuestReport(companyId, groupId, "", begin, end);
        }

        /// <summary>
        /// Cập nhật Ghi Chú cho báo cáo đón/trả khách
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="time"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse UpdateGuestNote(long serial,DateTime time,String note)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new BaseResponse { Description = "Không tìm thấy thiết bị" };

            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null) return new BaseResponse { Description = "Thông tin công ty không chính xác" };

            GeneralGuestLog log = DataContext.GetWhere<GeneralGuestLog>(m => 
                m.GuidId == device.Indentity 
                && m.UpdateTime.Date == time.Date,company.DbId).FirstOrDefault();

            if (log == null) return new BaseResponse { Description = "Không có thông tin báo cáo này" };

            log.Note = note;

            try
            {
                DataContext.Update(log, MotherSqlId, m => m.Note);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Description = "Cập nhật ghi chú thành công", Status = 1 };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, "Cập nhật ghi chú ko thành công");
                return new BaseResponse { Description = "Cập nhật ghi chú ko thành công" };
            }
        }

#endregion báo cáo xe khách
    }
}