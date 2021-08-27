#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DeviceGroupController.cs
// Time Create : 1:38 PM 20/06/2016
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
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.DeviceGroup;
using StarSg.Utils.Models.Tranfer;

#endregion

namespace Datacenter.Api.Controllers
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
        /// <param name="gr"></param>
        /// <returns></returns>
        [HttpPost]
        public DeviceGroupAdd Add(DeviceGroupTranfer gr)
        {
            var company = Cache.GetCompanyById(gr.CompanyId);
            if (company == null) return new DeviceGroupAdd {Description = "Thông tin công ty không hợp lệ "};

            if (string.IsNullOrEmpty(gr.Name)) return new DeviceGroupAdd {Description = "KHông để trống tên đội"};

            var group = new DeviceGroup
            {
                Name = gr.Name,
                CompnayId = gr.CompanyId
            };

            try
            {
                // insert vào database
                DataContext.Insert(group, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                // thêm vào cache
                if (!Cache.GetQueryContext<DeviceGroup>().Add(group, company.Id))
                    return new DeviceGroupAdd {Description = "Không thêm group vào cache"};
                return new DeviceGroupAdd {Status = 1, Id = group.Id, Description = "Thêm đội xe thành công"};
            }
            catch (Exception e)
            {
                Log.Exception("DeviceGroupController", e, "Thêm group vào database lỗi");
                return new DeviceGroupAdd
                {
                    Description = "Không thể thêm đội xe vào databse, vung lòng liên hệ quảng trị"
                };
            }
        }

        /// <summary>
        ///     cập nhật lại đội xe
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gr"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, DeviceGroupTranfer gr)
        {
            if (string.IsNullOrEmpty(gr.Name)) return new BaseResponse {Description = "KHông để trống tên đội"};

            var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(id);
            if (group == null) return new BaseResponse {Description = "KHông tìm thấy đội xe"};

            group.Name = gr.Name;

            try
            {
                DataContext.Update(group, MotherSqlId, m => m.Name);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse {Status = 1, Description = "Cập nhật thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("GroupController", ex, "Cập nhật group vào database không thành công");
                return new BaseResponse {Description = "Cập nhật group vào database lỗi"};
            }
        }

        /// <summary>
        ///     xóa đội xe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long id)
        {
            var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(id);
            if (group == null) return new BaseResponse {Description = "KHông tìm thấy đội xe"};

            try
            {
                DataContext.Delete(group, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                if (!Cache.GetQueryContext<DeviceGroup>().Del(group, group.CompnayId))
                    return new BaseResponse {Description = "Xóa đội xe khỏi cache ko thành công"};
                return new BaseResponse {Status = 1, Description = "Xóa đội xe thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("GroupController", ex, "Xóa đội xe không thành công");
                return new BaseResponse {Description = "Xóa thông tin đội xe trong database ko thành công"};
            }
        }

        /// <summary>
        ///     lấy thông tin group qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGroupGetSingle GetById(long id)
        {
            var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(id);
            if (group == null) return new DeviceGroupGetSingle {Description = "KHông tìm thấy đội xe"};
            return new DeviceGroupGetSingle
            {
                Status = 1,
                Description = "OK",
                Group = new DeviceGroupGet {CompanyId = group.CompnayId, Id = group.Id, Name = group.Name}
            };
        }

        /// <summary>
        ///     lấy toàn bộ thông tin group theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGroupGetAll GetAllGroup(long companyId)
        {
            var group = Cache.GetQueryContext<DeviceGroup>().GetByCompany(companyId);

            return new DeviceGroupGetAll
            {
                Status = 1,
                Description = "OK",
                Groups =
                    group.Select(m => new DeviceGroupGet {CompanyId = m.CompnayId, Id = m.Id, Name = m.Name}).ToList()
            };
        }
    }
}