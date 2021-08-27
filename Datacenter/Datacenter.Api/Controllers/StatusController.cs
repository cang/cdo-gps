#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : StatusController.cs
// Time Create : 9:47 AM 14/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using StarSg.Utils.Models.DatacenterResponse.Status;
using StarSg.Utils.Models.Tranfer;
using System.Collections.Generic;
using Datacenter.Model.Utils;
using StarSg.Core;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý các trạng thái hiện hành của thiết bị
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StatusController : BaseController
    {
        private const int EXPIRE_DELAY_DAYS = 93;

        /// <summary>
        ///     Lấy thông tin trạng thái các xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusByCompanyId(long companyId, int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new BaseResponse { Description = "Không tồn tại công ty này" };

            if (bare)
            {
                var result = new StatusDeviceGetBare { Status = 1, Description = "OK" };
                result.Datas =
                    Cache.GetQueryContext<Device>()
                        .GetByCompany(companyId)
                        .Where(m => m.Status != null && m.EndTime.AddDays(expireDay) >= DateTime.Now)
                        //.Where(m => m.Status != null)
                        //.Where(m=>m.Status.BasicStatus.ClientSend>= DateTime.Now.AddMinutes(-company.Setting.TimeoutHidenDevice))
                        .Select(m => { return GetStatusDeviceBare(m); }).ToList();

                return result;
            }
            else
            {
                var result = new StatusDeviceGet { Status = 1, Description = "OK" };
                result.Datas =
                    Cache.GetQueryContext<Device>()
                        .GetByCompany(companyId)
                        .Where(m => m.Status != null && m.EndTime.AddDays(expireDay) >= DateTime.Now)
                        //.Where(m => m.Status != null)
                        //.Where(m=>m.Status.BasicStatus.ClientSend>= DateTime.Now.AddMinutes(-company.Setting.TimeoutHidenDevice))
                        .Select(m => { return GetStatusDevice(m);}).ToList();

                return result;
            }
        }


        /// <summary>
        ///     Lấy thông tin trạng thái các xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId"></param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusByGroupId(long companyId, long groupId, int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new BaseResponse { Description = "Không tồn tại công ty này" };

            if (bare)
            {
                var result = new StatusDeviceGetBare { Status = 1, Description = "OK" };
                result.Datas =
                    Cache.GetQueryContext<Device>()
                        .GetByGroup(companyId, groupId)
                        .Where(m => m.Status != null && m.EndTime.AddDays(expireDay) >= DateTime.Now)
                        //.Where(m => m.Status != null)
                        //.Where(m => m.Status.BasicStatus.ClientSend >= DateTime.Now.AddMinutes(-company.Setting.TimeoutHidenDevice))
                        .Select(m => { return GetStatusDeviceBare(m); }).ToList();
                return result;
            }
            else
            {
                var result = new StatusDeviceGet { Status = 1, Description = "OK" };
                result.Datas =
                    Cache.GetQueryContext<Device>()
                        .GetByGroup(companyId, groupId)
                        .Where(m => m.Status != null && m.EndTime.AddDays(expireDay) >= DateTime.Now)
                        //.Where(m => m.Status != null)
                        //.Where(m => m.Status.BasicStatus.ClientSend >= DateTime.Now.AddMinutes(-company.Setting.TimeoutHidenDevice))
                        .Select(m => { return GetStatusDevice(m); }).ToList();
                return result;
            }
           
        }

        /// <summary>
        /// Lấy danh sách theo serials list (dành cho khách lẽ)
        /// </summary>
        /// <param name="serials"></param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusBySerials(String serials, int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)
        {
            if(bare)
            {
                var result = new StatusDeviceGetBare { Status = 1, Description = "OK" };
                result.Datas = new List<StatusDeviceTranferBare>();

                foreach (var item in serials.Split(','))
                {
                    var m = Cache.GetQueryContext<Device>().GetByKey(long.Parse(item));
                    if (m == null) continue;
                    if (m.Status == null) continue;
                    if (m.EndTime.AddDays(expireDay) < DateTime.Now) continue;
                    result.Datas.Add(GetStatusDeviceBare(m));
                }

                return result;
            }
            else
            {
                var result = new StatusDeviceGet { Status = 1, Description = "OK" };
                result.Datas = new List<StatusDeviceTranfer>();

                foreach (var item in serials.Split(','))
                {
                    var m = Cache.GetQueryContext<Device>().GetByKey(long.Parse(item));
                    if (m == null) continue;
                    if (m.Status == null) continue;
                    if (m.EndTime.AddDays(expireDay) < DateTime.Now) continue;
                    result.Datas.Add(GetStatusDevice(m));
                }

                return result;
            }
        }


        private StatusDeviceTranfer GetStatusDevice(Device m)
        {
            return new StatusDeviceTranfer
            {
                Bs = m.Bs,
                AirMachine = m.Status.BasicStatus.AirMachine,
                LostSingnal = (DateTime.Now - m.Status.BasicStatus.ClientSend).TotalMinutes > 90,
                Door = m.Status.BasicStatus.Door,
                Fuel = m.Status.BasicStatus.Fuel,
                Gplx = m.Status.DriverStatus.Gplx,
                Gps = m.Status.BasicStatus.GpsStatus,
                Gsm = m.Status.BasicStatus.GsmSignal,
                Machine = m.Status.BasicStatus.Machine,
                Name = m.Status.DriverStatus.Name,
                OverTime = m.Status.DriverStatus.TimeWork,
                OverTimeInDay = m.Status.DriverStatus.TimeWorkInDay,
                OverSpeedCount = m.Status.DriverStatus.OverSpeedCount,
                TotalGpsDistance = m.Status.BasicStatus.TotalGpsDistance,
                TotalCurrentGpsDistance = m.Status.BasicStatus.TotalCurrentGpsDistance,
                KmOnDay = (int)(m.Status.BasicStatus.TotalGpsDistance - m.Status.LastTotalKmUsingOnDay),
                Serial = m.Serial,
                Speed = m.Status.BasicStatus.Speed,
                Power = m.Status.BasicStatus.Power,
                Time = m.Status.BasicStatus.ClientSend,
                Point = new GpsPoint
                {
                    Lat = m.Status.BasicStatus.GpsInfo?.Lat ?? 0,
                    Lng = m.Status.BasicStatus.GpsInfo?.Lng ?? 0,
                    Address = m.Status.BasicStatus.GpsInfo?.Address
                },
                PauseCount = m.Status.PauseCount,
                Model = m.ModelName,
                GroupId = m.GroupId,
                EndTime = m.EndTime,

                //useGuest = m.Temp?.HasGuestSensor??false,
                useGuest = m.DeviceType == DeviceType.TaxiVehicle,
                Guest = m.Status.BasicStatus.UseTemperature,

                PauseTime = m.Temp?.TimePause ?? DateTimeFix.Min,

                CameraId = m.CameraId
            };
        }

        private StatusDeviceTranferBare GetStatusDeviceBare(Device m)
        {
            var ret = new StatusDeviceTranferBare
            {
                Bs = m.Bs,
                Serial = m.Serial,
                Speed = m.Status.BasicStatus.Speed,
                Time = m.Status.BasicStatus.ClientSend,
                Point = new GpsPoint
                {
                    Lat = m.Status.BasicStatus.GpsInfo?.Lat ?? 0,
                    Lng = m.Status.BasicStatus.GpsInfo?.Lng ?? 0
                },
                EndTime = m.EndTime,
                status= "STOP"
            };

            if ((DateTime.Now - ret.Time).TotalHours > 12) ret.status = "LOST_POWER";
            if (!m.Status.BasicStatus.Machine) ret.status = "OFF";
            if (ret.Speed >= 7) ret.status = "RUNNING";
            if (!(m.Status.BasicStatus.GsmSignal > 0)) ret.status = "LOST_POWER";
            if (!m.Status.BasicStatus.GpsStatus) ret.status = "NO_GPS";

            return ret;
        }

    }
}