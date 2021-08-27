using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Maintenance;
using Route.Api.Auth.Models.Entity;
using StarSg.Utils.Models.DatacenterResponse.Enterprise;
using System.Collections.Generic;
using Log;
using Route.DeviceServer;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin bảo trì của thiết bị
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class MaintenanceController : BaseController
    {
        [Import]
        private ClientCachePacket _cachePacket;

        /// <summary>
        ///     cập nhật thông tin bảo trì thiết bị
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id của record</param>
        /// <param name="companyModelSpecification">thông tin cần cập nhật</param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long companyId, long id,
            [FromBody] ModelSpecificationTranfer companyModelSpecification)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa thông tin bảo trì thiết bị" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Put<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/Update?id={id}", companyModelSpecification);
        }

        /// <summary>
        ///     lấy thông tin bảo trì theo công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public ModelSpecGetMulti GetByCompanyId(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new ModelSpecGetMulti { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<ModelSpecGetMulti>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/GetByCompanyId?companyId={companyId}");
        }

        /// <summary>
        ///     xóa thông tin bảo trì thiết bị
        /// </summary>
        /// <param name="id">id record</param>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Delete(long id, long companyId)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền xóa thông tin bảo trì thiết bị" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Del<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/Delete?id={id}");
        }


        /// <summary>
        ///     reset định mức bảo trì
        /// </summary>
        /// <param name="serial">id công ty</param>
        /// <param name="optionName">tên option cần reset lại (DaoLop,ThayVo,ThayNhot,ThayLocDau,ThayLocGio,ThayLocNhot)</param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse ResetMaintenanceBySerial([FromUri] long serial, [FromUri] string optionName)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new BaseResponse { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Put<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/ResetMaintenanceBySerial?serial={serial}&optionName={optionName}");
        }

        /// <summary>
        ///     lấy báo cáo bảo trì theo serial
        /// </summary>
        /// <param name="serial">serial thiêt bị</param>
        /// <returns></returns>
        [HttpGet]
        public MaintenanceReportGet MaintenanceReportBySerial([FromUri] long serial)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new MaintenanceReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<MaintenanceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/MaintenanceReportBySerial?serial={serial}");
        }


        /// <summary>
        ///     lấy báo cáo bảo trì theo công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public MaintenanceReportGet MaintenanceReportByCompany(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new MaintenanceReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<MaintenanceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/MaintenanceReportByCompany?companyId={companyId}");
        }

        /// <summary>
        ///     lấy báo cáo bảo trì thiết bị theo group
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="groupId">id nhóm xe</param>
        /// <returns></returns>
        [HttpGet]
        public MaintenanceReportGet MaintenanceReportByGroup(long companyId, long groupId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new MaintenanceReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<MaintenanceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/MaintenanceReportByGroup?companyId={companyId}&groupId={groupId}");
        }

        /// <summary>
        ///     lấy danh sách log lịch sử bảo trì
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="beginDate">thời gian bắt đầu</param>
        /// <param name="endDate">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public ChangeSpecModelGet GetMaintenanceHistory(long serial, DateTime beginDate,
            DateTime endDate)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new ChangeSpecModelGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<ChangeSpecModelGet>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/GetMaintenanceHistory?serial={serial}&beginDate={beginDate}&endDate={endDate}");
        }

        /// <summary>
        /// Cho phép ghi nhận thông tin dữ liệu thô của thiết bị
        /// </summary>
        /// <param name="serial">thiết bị cần ghi nhận</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse TrackRaw(long serial)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền" };

            //_cachePacket.TrackRawLogSerial(serial);

            var center = DeviceRoute.GetDataCenter(serial);
            var api = new ForwardApi();
            return
                api.Post<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/TrackRaw?serial={serial}");
        }

        /// <summary>
        /// Lấy dữ liệu thô từ thiết bị gửi lên
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceRawGet GetRaw(long serial, DateTime begin, DateTime end)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new DeviceRawGet { Description = "Không có quyền" };

            var center = DeviceRoute.GetDataCenter(serial);
            var api = new ForwardApi();
            return
                api.Get<DeviceRawGet>(
                    $"{center.Ip}:{center.Port}/api/Maintenance/GetRaw?serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        /// Lấy thông tin thống kê thiết bị gửi gói tin đồng bộ bị trùng
        /// </summary>
        [HttpGet]
        public SyncDuplicateResponse GetSyncDuplicate()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new SyncDuplicateResponse { Description = "Không có quyền" };

            var api = new ForwardApi();

            var datacenters = DataCenterStore.GetAll();
            SyncDuplicateResponse ret = new SyncDuplicateResponse() { Status = 1, Description = "OK", Data = new System.Collections.Generic.List<SerialCounter>() };
            foreach (var center in datacenters)
            {
                SyncDuplicateResponse cret = api.Get<SyncDuplicateResponse>($"{center.Ip}:{center.Port}/api/Maintenance/GetSyncDuplicate");
                if (cret.Status > 0) ret.Data.AddRange(cret.Data);
            }

            return ret;
        }

        /// <summary>
        /// Lấy thông tin thống kê thiết bị gửi gói tin kết thúc sự kiện mà không có gói tin bắt đầu tương ứng
        /// </summary>
        [HttpGet]
        public SyncDuplicateResponse GetEventWithoutStart()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new SyncDuplicateResponse { Description = "Không có quyền" };

            var api = new ForwardApi();

            var datacenters = DataCenterStore.GetAll();
            SyncDuplicateResponse ret = new SyncDuplicateResponse() { Status = 1, Description = "OK", Data = new System.Collections.Generic.List<SerialCounter>() };
            foreach (var center in datacenters)
            {
                SyncDuplicateResponse cret = api.Get<SyncDuplicateResponse>($"{center.Ip}:{center.Port}/api/Maintenance/GetEventWithoutStart");
                if (cret.Status > 0) ret.Data.AddRange(cret.Data);
            }

            return ret;
        }

        /// <summary>
        /// Lấy thông tin thống kê thiết bị gửi gói tin bắt đầu sự kiện mà không có gói tin kết thúc tương ứng
        /// </summary>
        [HttpGet]
        public SyncDuplicateResponse GetEventWithoutEnd()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new SyncDuplicateResponse { Description = "Không có quyền" };

            var api = new ForwardApi();

            var datacenters = DataCenterStore.GetAll();
            SyncDuplicateResponse ret = new SyncDuplicateResponse() { Status = 1, Description = "OK", Data = new System.Collections.Generic.List<SerialCounter>() };
            foreach (var center in datacenters)
            {
                SyncDuplicateResponse cret = api.Get<SyncDuplicateResponse>($"{center.Ip}:{center.Port}/api/Maintenance/GetEventWithoutEnd");
                if (cret.Status > 0) ret.Data.AddRange(cret.Data);
            }

            return ret;
        }

        /// <summary>
        /// Lấy danh sách serial , số lần gửi gói 0 khi thiết bị phát lên server
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SyncDuplicateResponse Get301CompressOpcode0()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new SyncDuplicateResponse { Description = "Không có quyền" };

            try
            {
                StatisticMemoryLog mlog = MefLoader.Container.GetExportedValue<StatisticMemoryLog>();
                IList<DeviceStatisticLog> alldevices = mlog.GetOpcode301ZeroList();

                SyncDuplicateResponse result = new SyncDuplicateResponse();
                result.Status = 1;
                result.Description = "OK";

                List<SerialCounter> ret = new List<SerialCounter>();
                foreach (var dev in alldevices)
                {
                    if (dev.Opcode301Zero <= 0) continue;

                    ret.Add(new SerialCounter()
                    {
                        Serial = dev.Serial,
                        Counter = dev.Opcode301Zero
                    });
                }


                result.Data = ret.OrderByDescending(m => m.Counter).ToList();
                return result;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "Get301CompressOpcode0");
                return new SyncDuplicateResponse { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }


        /// <summary>
        /// Lấy danh sách serial , số lần gửi gói 301 khi thiết bị phát lên server
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SyncDuplicateResponse GetCounter301CompressOpcode0()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new SyncDuplicateResponse { Description = "Không có quyền" };

            try
            {
                StatisticMemoryLog mlog = MefLoader.Container.GetExportedValue<StatisticMemoryLog>();
                IList<DeviceStatisticLog> alldevices = mlog.GetOpcode301ListOnly();

                SyncDuplicateResponse result = new SyncDuplicateResponse();
                result.Status = 1;
                result.Description = "OK";

                List<SerialCounter> ret = new List<SerialCounter>();
                foreach (var dev in alldevices)
                {
                    if (dev.Opcode301Zero <= 0) continue;

                    ret.Add(new SerialCounter()
                    {
                        Serial = dev.Serial,
                        Counter = dev.Opcode301Zero
                    });
                }


                result.Data = ret.OrderByDescending(m => m.Counter).ToList();
                return result;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "GetCounter301CompressOpcode0");
                return new SyncDuplicateResponse { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }

        /// <summary>
        /// GetGCCollect
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetGCCollect()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền" };

            try
            {
                long before = GC.GetTotalMemory(false);
                GC.Collect();
                long after = GC.GetTotalMemory(true);
                return new BaseResponse
                {
                    Status = 1,
                    Description = $"Before={before} After={after}"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Status = 0,
                    Description = $"[EXCEPTION] {ex.Message} TRACE {ex.StackTrace}"
                };
            }
        }

        /// <summary>
        /// Đánh dấu thiết bị thay thế
        /// </summary>
        /// <param name="serial">thiết bị cần ghi nhận</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse MarkReplaceSerial(long serial)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền" };

            BaseResponse ret = new BaseResponse { Description = "Không tìm thấy server" };
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/Maintenance/MarkReplaceSerial?serial={serial}");
            }
            return ret;
        }

        /// <summary>
        /// Hủy đánh dấu thiết bị thay thế
        /// </summary>
        /// <param name="serial">thiết bị cần ghi nhận</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse UnmarkReplaceSerial(long serial)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền" };

            BaseResponse ret = new BaseResponse { Description = "Không tìm thấy server" };
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/Maintenance/UnmarkReplaceSerial?serial={serial}");
            }
            return ret;
        }

        /// <summary>
        /// Danh sách thiết bị đánh dấu thay thế
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ReplaceSerialGet ReplaceSerialList()
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new ReplaceSerialGet { Description = "Không có quyền" };

            ReplaceSerialGet ret = new ReplaceSerialGet() { Status = 1 , Datas = new List<long>()};
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                ReplaceSerialGet localret = api.Get<ReplaceSerialGet>($"{center.Ip}:{center.Port}/api/Maintenance/ReplaceSerialList");
                if (localret.Status > 0 && localret.Datas != null) ret.Datas.AddRange(localret.Datas);
            }
            return ret;
        }



        /// <summary>
        /// Đánh dấu thiết bị thay thế
        /// </summary>
        /// <param name="newSerial"></param>
        /// <param name="originalSerial"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse FixDevice(long newSerial, long originalSerial)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền" };

            BaseResponse ret = new BaseResponse { Description = "Không tìm thấy server" };
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/Maintenance/FixDevice?newSerial={newSerial}&originalSerial={originalSerial}");
            }
            return ret;
        }


    }
}