using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Area;
using StarSg.Utils.Models.DatacenterResponse.SpecialTour;
using StarSg.Utils.Models.Tranfer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Route.Api.Controllers
{
    /// <summary>
    /// quản lý thông tin cuốc đặc biệt
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SpecialTourController : BaseController
    {
        /// <summary>
        ///     Thêm cuốc đặc biệt
        /// </summary>
        /// <param name="companyId"></param>        
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPost]
        public AreaAdd Add(long companyId, SpecialTourTranfer tran)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaAdd { Description = "Không xác định được máy chủ quản lý" };
            var api = new ForwardApi();
            return api.Post<AreaAdd>($"{center.Ip}:{center.Port}/api/SpecialTour/Add", tran);
        }


        /// <summary>
        ///     Cập nhật thông tin cuốc đặc biệt
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long companyId, long id, SpecialTourTranfer tran)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không xác định được máy chủ quản lý" };
            var api = new ForwardApi();
            return api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/SpecialTour/Update?id={id}", tran);
        }

        /// <summary>
        ///     xóa cuốc đặc biệt
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, long id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không xác định được máy chủ quản lý" };
            var api = new ForwardApi();
            return api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/SpecialTour/Del?companyId={companyId}&id={id}");
        }

        /// <summary>
        /// Lấy cuốc đặc biệt theo id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGet GetById(long companyId, long id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new SpecialTourGet { Description = "Không xác định được máy chủ quản lý" };
            var api = new ForwardApi();
            return
                api.Get<SpecialTourGet>(
                    $"{center.Ip}:{center.Port}/api/SpecialTour/GetById?companyId={companyId}&id={id}");
        }

        /// <summary>
        /// Lấy cuốc đặc biệt theo serial và ngày
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGetMulti GetBySerial(long serial, DateTime begin, DateTime end)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null) return new SpecialTourGetMulti { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<SpecialTourGetMulti>(
                    $"{center.Ip}:{center.Port}/api/SpecialTour/GetBySerial?serial={serial}&begin={begin}&end={end}");
        }


        /// <summary>
        ///     Lấy cuốc đặc biệt tổng quát
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo seriallist</param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGetMulti GetReports(long companyId, DateTime begin, DateTime end, long groupId = 0, string seriallist = "", long serial = 0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new SpecialTourGetMulti { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<SpecialTourGetMulti>(
                    $"{center.Ip}:{center.Port}/api/SpecialTour/GetReports?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&seriallist={seriallist}&serial={serial}");
        }

        /// <summary>
        ///     Lấy cuốc đặc biệt tổng quát theo serials list ids (theo yêu cầu bên Web, api này tương tự GetReports chỉ khác ids chính là seriallist)
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="ids">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo seriallist</param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGetMulti GetReportBySerials(long companyId, DateTime begin, DateTime end, long groupId = 0, string ids = "", long serial = 0)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new SpecialTourGetMulti { Description = "Không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<SpecialTourGetMulti>(
                    $"{center.Ip}:{center.Port}/api/SpecialTour/GetReports?companyId={companyId}&begin={begin}&end={end}&groupId={groupId}&seriallist={ids}&serial={serial}");
        }


    }

}