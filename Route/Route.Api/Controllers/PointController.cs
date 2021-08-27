using System;
using System.ComponentModel.Composition;
using System.Web.Caching;
using System.Web.Http;
using Core.Models.Tranfer.GpsCheckPoint;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.PointGps;
using StarSg.Utils.Models.Tranfer;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     Quản lý thông tin điểm
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PointController : BaseController
    {
        /// <summary>
        ///     Thêm mới 1 điểm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPost]
        public PointGpsAdd Add(long companyId,GpsCheckPointTranfer tran)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointGpsAdd { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            PointGpsAdd ret = api.Post<PointGpsAdd>($"{center.Ip}:{center.Port}/api/Point/Add", tran);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Add, $"Thêm điểm {tran.Name}");
            return ret;
        }

        /// <summary>
        ///     Cập nhật thông tin điểm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long companyId, int id, GpsCheckPointTranfer tran)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/Point/Update?id={id}", tran);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Edit, $"Thay đổi điểm {id} tên {tran.Name}");
            return ret;
        }


        /// <summary>
        ///     xóa thông tin điểm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, int id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/Point/Del?id={id}");
            AddAccessHistory(ret, 0, AccessHistoryMethod.Edit, $"Xóa điểm {id}");
            return ret;
        }

        /// <summary>
        ///     lấy thông tin điểm theo id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public PointGpsGetSingle GetById(long companyId, int id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointGpsGetSingle { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            return api.Get<PointGpsGetSingle>($"{center.Ip}:{center.Port}/api/Point/GetById?id={id}");
        }

        /// <summary>
        ///     lấy thông tin vùng theo Điểm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <returns></returns>
        [HttpGet]
        public PointGpsGetMulti GetByGroup(long companyId, long groupId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointGpsGetMulti { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            return api.Get<PointGpsGetMulti>($"{center.Ip}:{center.Port}/api/Point/GetByGroup?companyId={companyId}&groupId={groupId}");
        }

        /// <summary>
        ///     Lấy thông tin điểm theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public PointGpsGetMulti GetByCompany(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new PointGpsGetMulti { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            return api.Get<PointGpsGetMulti>($"{center.Ip}:{center.Port}/api/Point/GetByCompany?companyId={companyId}");

        }

    }
}