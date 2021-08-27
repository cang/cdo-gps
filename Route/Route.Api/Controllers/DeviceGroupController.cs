using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.DeviceGroup;
using StarSg.Utils.Models.Tranfer;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     Quản lý thông tin nhóm xe
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceGroupController : BaseController
    {
        /// <summary>
        ///     thêm mới đội xe
        /// </summary>
        /// <param name="gr">thông tin đội xe</param>
        /// <returns></returns>
        [HttpPost]
        public DeviceGroupAdd Add(DeviceGroupTranfer gr)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer ) return new DeviceGroupAdd { Description = "Không có quyền thêm đội" };

            var center = CompanyRoute.GetDataCenter(gr.CompanyId);
            if (center == null)
                return new DeviceGroupAdd {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            DeviceGroupAdd ret = api.Post<DeviceGroupAdd>($"{center.Ip}:{center.Port}/api/DeviceGroup/Add", gr);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Add, $"Thêm đội {gr.Name}");
            return ret;
        }

        /// <summary>
        ///     cập nhật lại đội xe
        /// </summary>
        /// <param name="id">id đội xe</param>
        /// <param name="gr"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, DeviceGroupTranfer gr)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa đội" };

            var center = CompanyRoute.GetDataCenter(gr.CompanyId);
            if (center == null)
                return new DeviceGroupAdd {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            
            var api = new ForwardApi();
            DeviceGroupAdd ret = api.Put<DeviceGroupAdd>($"{center.Ip}:{center.Port}/api/DeviceGroup/Update?id={id}", gr);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Edit, $"Thay đổi đội {gr.Name}");
            return ret;
        }

        /// <summary>
        ///     xóa đội xe
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id đội xe</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, long id)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền xóa đội" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceGroup/Del?id={id}");
            AddAccessHistory(ret, 0, AccessHistoryMethod.Delete, $"Xóa đội {id} cty {companyId}");
            return ret;
        }

        /// <summary>
        ///     lấy thông tin group qua id
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id đội xe</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGroupGetSingle GetById(long companyId, long id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceGroupGetSingle {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return api.Get<DeviceGroupGetSingle>($"{center.Ip}:{center.Port}/api/DeviceGroup/GetById?id={id}");
        }

        /// <summary>
        ///     lấy toàn bộ thông tin group theo công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGroupGetAll GetAllGroup(long companyId)
        {
            if (companyId <= 0 && UserPermision.GetLevel() <= (int)AccountLevel.Administrator)
                return new DeviceGroupGetAll() { Status = 1, Description = "OK"};

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceGroupGetAll {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();

            DeviceGroupGetAll ret = api.Get<DeviceGroupGetAll>(
                    $"{center.Ip}:{center.Port}/api/DeviceGroup/GetAllGroup?companyId={companyId}");

            UserPermision.EnsureGroup(companyId, ret);

            return ret;
                
        }

    }
}