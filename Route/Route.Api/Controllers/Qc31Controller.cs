using System;
using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Qc31;

namespace Route.Api.Controllers
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
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DeviceTripLogGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceTripLogGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetTripLog?serial={serial}&begin={begin}&end={end}");
        }

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
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null)
                return new DeviceSpeedTraceLogGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceSpeedTraceLogGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetSpeedTrace?serial={serial}&begin={begin}&end={end}");
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
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null)
                return new DeviceOverSpeedLogGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceOverSpeedLogGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetOverSpeedLogByDevice?serial={serial}&begin={begin}&end={end}");
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
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceOverSpeedLogGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceOverSpeedLogGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetOverSpeedLogByCompnay?companyId={companyId}&begin={begin}&end={end}");
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
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DevicePauseLogGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DevicePauseLogGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetPauseLog?serial={serial}&begin={begin}&end={end}");
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
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DriverTime31LogGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DriverTime31LogGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetDriverTime?serial={serial}&begin={begin}&end={end}");
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
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new AllDeviceReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDeviceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDeviceReportBySerial?serial={serial}&begin={begin}&end={end}");
        }


        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp của 1 nhóm xe trong công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="list">danh sách các serial cần xem phân cách bằng dấu | . Ví dụ 1|2|3</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDeviceReportGet GetAllDeviceReportBySerials(long companyId, string list, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AllDeviceReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDeviceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDeviceReportBySerials?list={list}&begin={begin}&end={end}");
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
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AllDeviceReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDeviceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDeviceReportByCompany?companyId={companyId}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     lấy thông tin báo cáo tổng hợp  theo nhóm xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDeviceReportGet GetAllDeviceReportByGroupId(long companyId, long groupId, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AllDeviceReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDeviceReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDeviceReportByGroupId?groupId={groupId}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     Lấy thông tin báo cáo thổng hợp theo tài xế
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDriverReportGet GetAllDriverReportById(long companyId, long id, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AllDriverReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDriverReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDriverReportById?id={id}&begin={begin}&end={end}");
        }


        /// <summary>
        ///     Lấy thông tin báo cáo theo 1 nhóm lái xe cùng công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="ids">danh id của tài xế cần xem phân cách bới dấu | , VD : 1|2|3</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AllDriverReportGet GetAllDriverReportByIds(long companyId, string ids, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AllDriverReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDriverReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDriverReportByCompnayId?ids={ids}&begin={begin}&end={end}");
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
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AllDriverReportGet {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AllDriverReportGet>(
                    $"{center.Ip}:{center.Port}/api/Qc31/GetAllDriverReportByCompnayId?companyId={companyId}&begin={begin}&end={end}");
        }
    }
}