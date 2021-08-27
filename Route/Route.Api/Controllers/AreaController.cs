using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Area;
using StarSg.Utils.Models.Tranfer.CheckZone;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    /// quản lý vùng
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class AreaController : BaseController
    {
        /// <summary>
        ///     thêm vùng
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="tran">thông tin vùng</param>
        /// <returns></returns>
        [HttpPost]
        public AreaAdd Add(long companyId, CheckZoneTranfer tran)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaAdd {Description = "Không xác định được máy chủ quản lý"};

            var api = new ForwardApi();
            AreaAdd ret = api.Post<AreaAdd>($"{center.Ip}:{center.Port}/api/Area/Add", tran);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Add, $"Thêm vùng {tran.Name}");
            return ret;
        }

        /// <summary>
        ///     Cập nhật thông tin Vùng
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long companyId, int id, CheckZoneTranfer tran)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/Area/Update?id={id}", tran);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Edit, $"Thay đổi vùng {id} tên {tran.Name}");
            return ret;
        }

        /// <summary>
        /// xóa bỏ vùng
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id vùng</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, int id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new BaseResponse { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/Area/Del?id={id}");
            AddAccessHistory(ret, 0, AccessHistoryMethod.Delete, $"Xóa vùng {id}");
            return ret;
        }

        /// <summary>
        /// Lấy thông tin vùng theo id
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id vùng</param>
        /// <returns></returns>
        [HttpGet]
        public AreaGetSingle GetById(long companyId, int id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaGetSingle { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            return api.Get<AreaGetSingle>($"{center.Ip}:{center.Port}/api/Area/GetById?id={id}");
        }

        /// <summary>
        ///     lấy thông tin vùng theo Nhóm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <returns></returns>
        [HttpGet]
        public AreaGetMulti GetByGroup(long companyId, long groupId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaGetMulti { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            return api.Get<AreaGetMulti>($"{center.Ip}:{center.Port}/api/Area/GetByGroup?companyId={companyId}&groupId={groupId}");
        }

        /// <summary>
        /// lấy tất cả thông tin các vùng
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public AreaGetMulti GetByCompany(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null) return new AreaGetMulti { Description = "Không xác định được máy chủ quản lý" };

            var api = new ForwardApi();
            return api.Get<AreaGetMulti>($"{center.Ip}:{center.Port}/api/Area/GetByCompany?companyId={companyId}");
        }
    }
}