using System.ComponentModel.Composition;
using System.Web.Http;
using Core.Models.Tranfer.Driver;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Driver;
using System.Collections.Generic;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin tài xế
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DriverController : BaseController
    {
        /// <summary>
        ///     thêm mới thông tin tài xế
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        [HttpPost]
        public DriverAdd Add(long companyId, DriverTranfer dr)
        {
            //Tạm thời cho khách lẻ và đội thêm/sửa tài xế
            //if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new DriverAdd { Description = "Không có quyền thêm tài xế" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DriverAdd {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            DriverAdd ret = api.Post<DriverAdd>($"{center.Ip}:{center.Port}/api/Driver/Add", dr);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Add, $"Thêm mới thông tin tài xế {dr.Name}");
            return ret;
        }

        /// <summary>
        ///     cập nhật thông tin lái xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long companyId, long id, DriverTranfer dr)
        {
            //Tạm thời cho khách lẻ và đội thêm/sửa tài xế
            //if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa tài xế" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DriverAdd {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            DriverAdd ret = api.Put<DriverAdd>($"{center.Ip}:{center.Port}/api/Driver/Update?id={id}", dr);
            AddAccessHistory(ret, 0, AccessHistoryMethod.Edit, $"Thay đổi thông tin tài xế {id} tên {dr.Name}");
            return ret;
        }

        /// <summary>
        ///     xóa thông tin lái xe
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id tài xế</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, long id)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền xóa tài xế" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/Driver/Del?id={id}");
            AddAccessHistory(ret, 0, AccessHistoryMethod.Delete, $"Xóa thông tin tài xế {id}");
            return ret;
        }


        /// <summary>
        ///     lấy thông tin lái xe theo id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverGetSingle GetDriverById(long companyId, long id)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DriverGetSingle {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DriverGetSingle>(
                    $"{center.Ip}:{center.Port}/api/Driver/GetDriverById?id={id}");
        }

        /// <summary>
        ///     lấy thông tin tài xế theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverGetMulti GetDriverByCompany(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DriverGetMulti {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();

            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
                return api.Get<DriverGetMulti>(
                    $"{center.Ip}:{center.Port}/api/Driver/GetDriverByCompany?companyId={companyId}");

            long groupId = UserPermision.GetUserGroupId(companyId);
            if (groupId == -1)
                return api.Get<DriverGetMulti>(
                    $"{center.Ip}:{center.Port}/api/Driver/GetDriverByCompany?companyId={companyId}");

            if (groupId > 0)
                return api.Get<DriverGetMulti>(
                    $"{center.Ip}:{center.Port}/api/Driver/GetDriverByGroupId?groupId={groupId}");
            else
                return new DriverGetMulti
                {
                    Status = 1,
                    Description = "OK",
                    Drivers = new List<DriverTranfer>()
                };
        }

        /// <summary>
        ///     Lấy thông tin lái xe theo đội xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">nếu = 0 thì lấy theo companyId</param>
        /// <returns></returns>
        [HttpGet]
        public DriverGetMulti GetDriverByGroupId(long companyId, long groupId)
        {
            if (groupId == 0)
                return GetDriverByCompany(companyId);

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DriverGetMulti {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<DriverGetMulti>(
                    $"{center.Ip}:{center.Port}/api/Driver/GetDriverByGroupId?groupId={groupId}");
        }

    }
}