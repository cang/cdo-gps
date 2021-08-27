using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Status;
using Route.DeviceServer;
using System;
using System.Collections.Generic;
using DevicePacketModels;
using System.Linq;
using StarSg.Utils.Models.Tranfer;
using CorePacket;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
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

        [Import] private ClientCachePacket _cachePacket;

        /// <summary>
        ///     Lấy thông tin trạng thái các xe theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusByCompanyId(long companyId,int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)
        {
            //Nếu companyId = 0 thì nghĩa là lấy danh sách thiết bị chưa xác định
            if (companyId<=0) return GetUnknownDevice();

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();

            #region dành cho khách lẻ
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer)
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if (serials != null && serials.Count > 0)
                {
                    if (!bare) return api.Get<StatusDeviceGet>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}&bare={bare}");
                    else return api.Get<StatusDeviceGetBare>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}&bare={bare}");
                }
            }
            #endregion dành cho khách lẻ

            long groupId = UserPermision.GetUserGroupId(companyId);

            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster && groupId > 0)
            {
                if (!bare)
                    return
                        api.Get<StatusDeviceGet>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByGroupId?companyId={companyId}&groupId={groupId}&expireDay={expireDay}&bare={bare}");
                else
                    return
                        api.Get<StatusDeviceGetBare>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByGroupId?companyId={companyId}&groupId={groupId}&expireDay={expireDay}&bare={bare}");
            }
            else
            {
                if(!bare)
                    return
                        api.Get<StatusDeviceGet>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByCompanyId?companyId={companyId}&expireDay={expireDay}&bare={bare}");
                else
                    return
                        api.Get<StatusDeviceGetBare>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByCompanyId?companyId={companyId}&expireDay={expireDay}&bare={bare}");
            }
        }

        /// <summary>
        ///     Lấy thông tin trạng thái các xe theo đội
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">chỉ sử dụng cho user level 0 1 quyền admin saigonstar, 0 là không lọc</param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusByGroupId(long companyId,long groupId, int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)    
        {
            //Nếu companyId = 0 thì nghĩa là lấy danh sách thiết bị chưa xác định
            if (companyId <= 0) return GetUnknownDevice();

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();

            //tự tìm nhóm cho user quản lý nhóm và admin đại lý
            long group = UserPermision.GetUserGroupId(companyId);

            #region dành cho khách lẻ và quản lý nhóm
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer)
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if (serials != null && serials.Count > 0)
                {
                    if(!bare)
                        return api.Get<StatusDeviceGet>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}&bare={bare}");
                    else
                        return api.Get<StatusDeviceGetBare>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}&bare={bare}");
                }

                //lọc cho quản lý nhóm (không sử dụng tham số groupId truyền vô)
                if (group >= 0) groupId = group;
            }
            #endregion

            //CHƯA CẦN THIẾT LỌC CHỖ NÀY
            ////admin của đại lý
            //if (UserPermision.GetLevel() == (int)AccountLevel.CustomerMaster)
            //    if(group>-1) return new StatusDeviceGet { Description = "Người dùng này không quản lý cty nào cả" };

            //chỉ có saigonstar mới có quyền coi tất cả

            if(!bare)
                return
                    api.Get<StatusDeviceGet>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByGroupId?companyId={companyId}&groupId={groupId}&expireDay={expireDay}&bare={bare}");
            else
                return
                    api.Get<StatusDeviceGetBare>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByGroupId?companyId={companyId}&groupId={groupId}&expireDay={expireDay}&bare={bare}");
        }


        /// <summary>
        /// Lấy danh sách theo serials list ( cach nhau dau phay , )
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serials">serials list cach nhau dau , </param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusBySerials(long companyId, String serials, int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            if (!bare)
                return new ForwardApi().Get<StatusDeviceGet>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}&bare={bare}");
            else
                return new ForwardApi().Get<StatusDeviceGetBare>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}&bare={bare}");
        }

        /// <summary>
        /// Lấy danh sách theo serial 
        /// </summary>
        /// <param name="serial">serial</param>
        /// <param name="expireDay">cho phép hiển thị quá expireDay ngày</param>
        /// <param name="bare">chỉ lấy thông tin cơ bản</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceStatusBySerial(long serial, int expireDay = EXPIRE_DELAY_DAYS, bool bare = false)
        {
            var center = GetDataCenter(serial);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            if (!bare)
                return new ForwardApi().Get<StatusDeviceGet>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={serial}&expireDay={expireDay}&bare={bare}");
            else
                return new ForwardApi().Get<StatusDeviceGetBare>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={serial}&expireDay={expireDay}&bare={bare}");
        }

        //sua loi khong biet vi sao serial nay khong ton tai trong bo key
        private Route.Core.DataCenterInfo GetDataCenter(long serial)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center != null) return center;

            //try to add if only one server
            Log.Error("DeviceController", $"Không thấy serial {serial} trong DeviceRoute");
            var centers = DataCenterStore.GetAll();
            if (centers.Count == 1)
            {
                foreach (var item in centers)
                {
                    return item;
                }
            }

            return null;
        }

        private StatusDeviceGet GetUnknownDevice()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster)
                return new StatusDeviceGet { Description = "Không có quyền xem các thiết bị chưa khai báo" };

            var result = new StatusDeviceGet { Status = 1, Description = "OK" };
            IList<PBaseSyncPacket> rets = _cachePacket.GetAllUnknownDevices();
            result.Datas =
                rets.Select(m =>
                {
                    return new StatusDeviceTranfer
                    {
                        Bs = m.Serial.ToString(),
                        AirMachine = m?.StatusIo.AirMachine ?? false,
                        LostSingnal = false,
                        Door = m?.StatusIo.Door ?? false,
                        //Gplx = "",
                        Gps = m.GpsStatus,
                        Gsm = m.GsmSignal,
                        Machine = m?.StatusIo.Key ?? false,
                        //Name = m.DriverId.ToString(),
                        OverTime = 0,
                        OverTimeInDay = 0,
                        OverSpeedCount = 0,
                        TotalGpsDistance = m.TotalGpsDistance,
                        TotalCurrentGpsDistance = m.TotalCurrentGpsDistance,
                        KmOnDay = 0,
                        Serial = m.Serial,
                        Speed = m.GpsInfo.Speed,
                        Power = m.Power,
                        Time = m.Time,
                        Point = new GpsPoint
                        {
                            Lat = m.GpsInfo.Lat,
                            Lng = m.GpsInfo.Lng
                        },
                        Model = "",
                        GroupId = 0,
                        EndTime = DateTime.Now.AddDays(365)
                    };
                }
                    ).ToList();
            return result;
        }

    }

}