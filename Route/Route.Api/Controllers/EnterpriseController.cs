using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Enterprise;

namespace Route.Api.Controllers
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
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <param name="zip">nén dữ liệu hay không (mặc định không)</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceTripGet GetTrip(long serial, DateTime begin, DateTime end,bool zip = false)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            //if (center == null) return new DeviceTripGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();

            return
                api.Get<DeviceTripGet>(
                    //$"{center.Ip}:{center.Port}/api/Enterprise/GetTrip?serial={serial}&begin={begin}&end={end}&zip={zip}");
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetTripSQL?serial={serial}&begin={begin}&end={end}&zip={zip}");
        }

        /// <summary>
        ///     lấy thông tin cuốc xe
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceSessionGet GetSession(long serial, DateTime begin, DateTime end)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DeviceSessionGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceSessionGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetSession?serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     lấy thông tin dừng đỗ
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceStopLogGet GetStop(long serial, DateTime begin, DateTime end)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DeviceStopLogGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceStopLogGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetStop?serial={serial}&begin={begin}&end={end}");
        }


        /// <summary>
        ///     Lấy thông tin trạng thái máy
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceMachineGet GetMachine(long serial, DateTime begin, DateTime end)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DeviceMachineGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceMachineGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetMachine?serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     Lấy thông tin nhiệt độ
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceTemperLogGet GetTemper(long serial, DateTime begin, DateTime end)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DeviceTemperLogGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceTemperLogGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetTemper?serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     lấy thông tin nhiên liệu
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <param name="checkFlag">có kiểm tra theo cờ càm biến</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceFuelLogGet GetFuel(long serial, DateTime begin, DateTime end, bool checkFlag = false)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new DeviceFuelLogGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DeviceFuelLogGet>(
                    //$"{center.Ip}:{center.Port}/api/Enterprise/GetFuel?serial={serial}&begin={begin}&end={end}&checkFlag={checkFlag}");
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetFuelSQL?serial={serial}&begin={begin}&end={end}&checkFlag={checkFlag}");
        }

        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp theo id
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceAllReportGet GetAllReportBySerial(long serial, DateTime begin, DateTime end)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new EnterpriseDeviceAllReportGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceAllReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAllReportBySerial?serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp theo 1 nhóm xe trong công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="list">danh sách serial các thiết bị cần xem phân cách nhau bời dấu | . VD : 1|2|3</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceAllReportGet GetAllReportBySerials(long companyId, string list, DateTime begin,
            DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceAllReportGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceAllReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAllReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={list}&serial=0");
        }


        /// <summary>
        ///     Lấy thông tin báo cáo tổng hợp theo serial , nhóm hoặc toàn công ty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceAllReportGet GetAllReportByCompany(long companyId, DateTime begin, DateTime end, long groupId = 0,long serial=0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceAllReportGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceAllReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAllReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={""}&serial={serial}");
        }


        /// <summary>
        ///     lấy thông tin báo cáo tổng hợp theo nhóm và công ty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceAllReportGet GetAllReport(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceAllReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceAllReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAllReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");

        }

        /// <summary>
        ///     Thống kê xe Hiện Hành nằm trong Điểm theo id
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">ID Điểm</param>
        /// <returns></returns>        
        [HttpGet]
        public PointReportGet GetCurrentPointReportById(long companyId, long id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentPointReport?companyId={companyId}&groupId=0&ids={""}&id={id}");
        }

        /// <summary>
        ///     Thống kê Xe nằm trong Điểm theo id
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">ID Điểm</param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns>Lấy giá trị hiện hành nếu begin và end đều lớn hơn ngày giờ hiện tại </returns>        
        [HttpGet]
        public PointReportGet GetPointReportById(long companyId, long id, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetPointReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={""}&id={id}");
        }


        /// <summary>
        ///     Thống kê xe Hiện Hành nằm trong Điểm theo danh sách id điểm thuộc công ty
        /// </summary>
        /// <param name="companyId">Id công ty</param>        
        /// <param name="ids">Danh sách Id Vùng cách nhau | hoặc , hoặc ; hoặc kí tự trống </param>
        /// <returns></returns>
        [HttpGet]
        public PointReportGet GetCurrentPointReportByIds(long companyId, string ids)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentPointReport?companyId={companyId}&groupId=0&ids={ids}&id=0");
        }

        /// <summary>
        ///     Thống kê Xe nằm trong Điểm theo danh sách id Điểm thuộc công ty
        /// </summary>
        /// <param name="companyId">Id công ty</param>        
        /// <param name="ids">Danh sách Id Vùng cách nhau | hoặc , hoặc ; hoặc kí tự trống </param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns>Lấy giá trị hiện hành nếu begin và end đều lớn hơn ngày giờ hiện tại </returns>
        [HttpGet]
        public PointReportGet GetPointReportByIds(long companyId, string ids, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetPointReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={ids}&id=0");
        }


        /// <summary>
        ///     Thống kê xe Hiện Hành nằm trong Điểm theo nhóm
        /// </summary>
        /// <param name="companyId">Id Công ty</param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <returns></returns>
        [HttpGet]
        public PointReportGet GetCurrentPointReportByGroupId(long companyId, long groupId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentPointReport?companyId={companyId}&groupId={groupId}&ids={""}&id=0");
        }

        /// <summary>
        ///     Thống kê Xe nằm trong Điểm theo nhóm của công ty
        /// </summary>
        /// <param name="companyId">Id Công ty</param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns>Lấy giá trị hiện hành nếu begin và end đều lớn hơn ngày giờ hiện tại </returns>
        [HttpGet]
        public PointReportGet GetPointReportByGroupId(long companyId, long groupId, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetPointReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={""}&id=0");
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
        public PointReportGet GetCurrentPointReport(long companyId, long groupId =0 , string ids = "", int id = 0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentPointReport?companyId={companyId}&groupId={groupId}&ids={ids}&id={id}");
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
        public PointReportGet GetPointReport(long companyId, DateTime begin, DateTime end, long groupId=0, string ids="", int id=0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<PointReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetPointReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&id={id}");

        }

        /// <summary>
        ///     Thống kê xe Hiện Hành nằm trong Vùng theo id
        /// </summary>
        /// <param name="companyId">Id công ty</param>        
        /// <param name="id">Id Vùng</param>
        /// <returns></returns>
        [HttpGet]
        public AreaReportGet GetCurrentAreaReportById(long companyId, long id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentAreaReport?companyId={companyId}&groupId=0&ids={""}&id={id}");
        }

        /// <summary>
        ///     Thống kê Xe nằm trong Vùng theo id
        /// </summary>
        /// <param name="companyId">Id công ty</param>        
        /// <param name="id">Id Vùng</param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns>Lấy giá trị hiện hành nếu begin và end đều lớn hơn ngày giờ hiện tại </returns>
        [HttpGet]
        public AreaReportGet GetAreaReportById(long companyId, long id, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAreaReport?companyId={companyId}&begin={begin}&end={end}&ids={""}&id={id}");
        }

        /// <summary>
        ///     Thống kê xe Hiện Hành nằm trong Vùng danh sách id vung thuộc công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="ids">Danh sách Id Vùng cách nhau | hoặc , hoặc ; hoặc kí tự trống </param>
        /// <returns></returns>
        [HttpGet]
        public AreaReportGet GetCurrentAreaReportByIds(long companyId, string ids)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentAreaReport?companyId={companyId}&groupId=0&ids={ids}&id=0");
        }

        /// <summary>
        ///     Thống kê Xe nằm trong Vùng theo danh sách id Vùng thuộc công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="ids">Danh sách Id Vùng cách nhau | hoặc , hoặc ; hoặc kí tự trống </param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns>Lấy giá trị hiện hành nếu begin và end đều lớn hơn ngày giờ hiện tại </returns>
        [HttpGet]
        public AreaReportGet GetAreaReportByIds(long companyId, string ids, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAreaReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={ids}&id=0");
        }

        /// <summary>
        ///     Thống kê xe Hiện Hành nằm trong Vùng theo nhóm
        /// </summary>
        /// <param name="companyId">Id công ty</param>
        /// <param name="groupId">Id nhóm, không lọc nếu =0</param>
        /// <returns></returns>
        [HttpGet]
        public AreaReportGet GetCurrentAreaReportByGroupId(long companyId, long groupId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentAreaReport?companyId={companyId}&groupId={groupId}&ids={""}&id=0");
        }

        /// <summary>
        ///     Thống kê Xe nằm trong Vùng theo nhóm
        /// </summary>
        /// <param name="companyId">Id công ty</param>
        /// <param name="groupId">Id nhóm, không lọc nếu =0</param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns>Lấy giá trị hiện hành nếu begin và end đều lớn hơn ngày giờ hiện tại </returns>
        [HttpGet]
        public AreaReportGet GetAreaReportByGroupId(long companyId, long groupId, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet {Description = "Không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAreaReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={""}&id=0");
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
        public AreaReportGet GetCurrentAreaReport(long companyId, long groupId=0, string ids="", int id=0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetCurrentAreaReport?companyId={companyId}&groupId={groupId}&ids={ids}&id={id}");
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
        public AreaReportGet GetAreaReport(long companyId, DateTime begin, DateTime end, long groupId=0, string ids="", int id=0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<AreaReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAreaReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&id={id}");
        }

        /// <summary>
        /// Lấy thông tin chi tiết Xe trong Vùng
        /// </summary>
        /// <param name="companyId">Id công ty</param>
        /// <param name="groupId">Id nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="areaId">id Vùng, nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public AreaSessionGet GetAreaSession(long companyId, long groupId, long areaId, long serial, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaSessionGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<AreaSessionGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetAreaSession?companyId={companyId}&groupId={groupId}&areaId={areaId}&serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        /// Lấy thông tin chi tiết Xe trong phạm vi bán kính Điểm
        /// </summary>
        /// <param name="companyId">Id công ty</param>
        /// <param name="groupId">Id nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="pointId">id Điểm, nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public PointSessionGet GetPointSession(long companyId, long groupId, long pointId, long serial, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointSessionGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<PointSessionGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetPointSession?companyId={companyId}&groupId={groupId}&pointId={pointId}&serial={serial}&begin={begin}&end={end}");

        }

        /// <summary>
        /// Lấy thông tin chi tiết xe thay đổi nhiên liệu
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <returns></returns>
        [HttpGet]
        public FuelSessionGet GetFuelSession(long companyId, long groupId, long serial, DateTime begin, DateTime end, string ids = "")
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new FuelSessionGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<FuelSessionGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetFuelSession?companyId={companyId}&groupId={groupId}&ids={ids}&serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     Lấy thông tin báo cáo nhiên liệu theo 1 nhóm xe trong công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="ids">danh sách serial cách nhau '|', ',', ';', ' ' </param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceFuelReportGet GetFuelReportBySerials(long companyId, string ids, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceFuelReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceFuelReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetFuelReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={ids}&serial=0");
        }


        /// <summary>
        ///     Lấy thông tin báo cáo nhiên liệu theo xe trong công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">serial</param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceFuelReportGet GetFuelReportBySerial(long companyId, long id, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceFuelReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceFuelReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetFuelReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={""}&serial={id}");
        }

        /// <summary>
        /// Lấy thông tin báo cáo nhiên liệu theo serial, nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceFuelReportGet GetFuelReport(long companyId, long groupId, long serial, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceFuelReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceFuelReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetFuelReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={""}&serial={serial}");
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
        public EnterpriseDeviceFuelReportGet GetFuelsReport(long companyId, DateTime begin, DateTime end, long groupId=0, string ids="", long serial = 0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceFuelReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceFuelReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetFuelReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");
        }

        /// <summary>
        /// Lấy thông tin chi tiết cuốc taxi đón khách
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <returns></returns>
        [HttpGet]
        public GuestSessionGet GetGuestSession(long companyId, long groupId, long serial, DateTime begin, DateTime end, string ids = "")
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new GuestSessionGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<GuestSessionGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetGuestSession?companyId={companyId}&groupId={groupId}&ids={ids}&serial={serial}&begin={begin}&end={end}");
        }


        /// <summary>
        ///     Lấy thông tin báo cáo đón khách theo xe trong công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">serial</param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceGuestReportGet GetGuestReportBySerial(long companyId, long id, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceGuestReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceGuestReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetGuestReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={""}&serial={id}");
        }


        /// <summary>
        ///     Lấy thông tin báo cáo đón khách theo 1 nhóm xe trong công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="ids">danh sách serial cách nhau '|', ',', ';', ' ' </param>
        /// <param name="begin">ngày bắt đầu</param>
        /// <param name="end">ngày kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceGuestReportGet GetGuestReportBySerials(long companyId, string ids, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceGuestReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceGuestReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetGuestReport?companyId={companyId}&begin={begin}&end={end}&groupId=0&ids={ids}&serial=0");
        }

        /// <summary>
        /// Lấy thông tin báo cáo đón khách theo serial, nhóm , cty 
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public EnterpriseDeviceGuestReportGet GetGuestReport(long companyId, long groupId, long serial, DateTime begin, DateTime end)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceGuestReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceGuestReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetGuestReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={""}&serial={serial}");
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
        public EnterpriseDeviceGuestReportGet GetGuestsReport(long companyId, DateTime begin, DateTime end, long groupId=0, string ids="", long serial=0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new EnterpriseDeviceGuestReportGet { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<EnterpriseDeviceGuestReportGet>(
                    $"{center.Ip}:{center.Port}/api/Enterprise/GetGuestReport?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&ids={ids}&serial={serial}");

        }

        /// <summary>
        /// Cập nhật Ghi Chú cho báo cáo đón/trả khách
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="time"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse UpdateGuestNote(long serial, DateTime time, String note)
        {
            //if (UserPermision.GetLevel() > (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa thiết bị" };
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null)
                return new BaseResponse { Description = "không tìm thấy máy chủ xử lý" };

            var api = new ForwardApi();
            return api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/Enterprise/UpdateGuestNote?serial={serial}&time={time}&note={note}");
        }

    }
}