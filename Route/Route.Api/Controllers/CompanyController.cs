using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Compnay;
using StarSg.Utils.Models.Tranfer;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    
    /// <summary>
    ///     quản lý thông tin công ty
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class CompanyController : BaseController
    {

        /// <summary>
        ///     thêm mới công ty
        /// </summary>
        /// <param name="centerId">id của cụm máy chủ  ( lấy thông tin ở API System )</param>
        /// <param name="com">thông tin công ty</param>
        /// <returns></returns>
        [HttpPost]
        public CompanyAdd Add(Guid centerId, CompanyTranfer com)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster ) return new CompanyAdd { Description = "Không có quyền thêm công ty" };

            var center = DataCenterStore.Get(centerId);
            if (center == null)
                return new CompanyAdd {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            CompanyAdd ret = api.Post<CompanyAdd>($"{center.Ip}:{center.Port}/api/Company/Add", com);

            //Update Branch Code
            if(ret.Status>0 && ret.CompanyId>0 && com!=null && com.BranchCode!=null)
                _accountManager.EnsureBranch(ret.CompanyId, com?.BranchCode);

            AddAccessHistory(ret,0, AccessHistoryMethod.Add, $"Thêm công ty {com.Name}");

            return ret;
        }

        /// <summary>
        ///     cập nhật thông tin công ty
        /// </summary>
        /// <param name="id">id công ty</param>
        /// <param name="com"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, CompanyTranfer com)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền sửa công ty" };

            var center = CompanyRoute.GetDataCenter(id);
            if (center == null) return new BaseResponse {Description = "Không xác định được máy chủ quản lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/Company/Update?id={id}", com);

            //Update Branch Code
            if (ret.Status > 0 && id > 0 && com != null && com.BranchCode != null)
                _accountManager.EnsureBranch(id, com?.BranchCode);

            AddAccessHistory(ret,0, AccessHistoryMethod.Edit, $"Thay đổi công ty {id} {com.Name}");
            return ret;
        }

        /// <summary>
        ///     xóa thông tin công ty
        /// </summary>
        /// <param name="id">id công ty</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Delete(long id)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền xóa công ty" };

            var center = CompanyRoute.GetDataCenter(id);
            if (center == null) return new BaseResponse {Description = "Không xác định được máy chủ quản lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/Company/Delete?id={id}");
            AddAccessHistory(ret,0, AccessHistoryMethod.Delete, $"Xóa công ty {id}");
            return ret;
        }

        /// <summary>
        ///     lấy thông tin công ty theo id
        /// </summary>
        /// <param name="id">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public CompanyGetSignal GetById(long id)
        {
            if (id <= 0 && UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
                return new CompanyGetSignal() { Status = 1, Description = "OK" , Company= GetUnknownCompany() };

            var center = CompanyRoute.GetDataCenter(id);
            if (center == null) return new CompanyGetSignal {Description = "Không xác định được máy chủ quản lý"};

            var api = new ForwardApi();
            CompanyGetSignal ret = api.Get<CompanyGetSignal>($"{center.Ip}:{center.Port}/api/Company/GetById?id={id}");

            //Get Branch Code
            if (ret.Status > 0 && id > 0 && ret.Company!=null)
                ret.Company.BranchCode = _accountManager.GetBranchOfCompany(id)?.BranchCode;

            //AddAccessHistory(ret, 0, "GET", $"Xem công ty {id}");

            return ret;
        }

        /// <summary>
        ///     lấy toàn bộ thông tin công ty trên tất cả máy chủ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CompanyGetAll GetAll()
        {
            var allCenter = DataCenterStore.GetAll();

            var result = new CompanyGetAll();
            result.Status = 1;
            result.Description = "OK";
            result.Companies = new List<CompanyGet>();
            var api = new ForwardApi();
            foreach (var dataCenterInfo in allCenter)
            {
                var tmp = api.Get<CompanyGetAll>($"{dataCenterInfo.Ip}:{dataCenterInfo.Port}/api/Company/GetAll");
                if (tmp != null && tmp.Status == 1)
                    ((List<CompanyGet>) result.Companies).AddRange(tmp.Companies.Where(m=>UserPermision.ContainCompanyId(m.Id)));
            }

            //Get Branch Code
            foreach (var item in result.Companies)
            {
                item.BranchCode = _accountManager.GetBranchOfCompany(item.Id)?.BranchCode;
            }

            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
                result.Companies.Add(GetUnknownCompany());

            return result;
        }

        /// <summary>
        ///     lấy toàn bộ thông tin công ty của 1 cụm máy chủ
        /// </summary>
        /// <param name="idCenter">id của cụm máy chủ xử ký ( xem trong api Systen)</param>
        /// <returns></returns>
        [HttpGet]
        public CompanyGetAll GetAllByCenterId(Guid idCenter)
        {
            var center = DataCenterStore.Get(idCenter);
            if (center == null)
                return new CompanyGetAll {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();

            var result = api.Get<CompanyGetAll>($"{center.Ip}:{center.Port}/api/Company/GetAll");

            //Get Branch Code
            foreach (var item in result.Companies)
            {
                item.BranchCode = _accountManager.GetBranchOfCompany(item.Id)?.BranchCode;
            }

            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
                result.Companies.Add(GetUnknownCompany());

            return result;
        }

        private CompanyGet GetUnknownCompany()
        {
            return new CompanyGet()
                {
                CreateTime = new DateTime(2017, 4, 20, 12, 0, 0),
                    DbId = 0,
                    ShortName = "DATACENTER_01",
                    Name = "_XE CHƯA LẮP_",
                    Id = -1,
                    Description = "Chứa danh sách xe đã lắp đặt nhưng chưa cấu hình",
                    Location = new GpsPoint()
                    {
                        Lat = 10.7811702f,
                        Lng = 106.623249f,
                        Address = "157 Lê Lâm, P. Phú Thạnh, Q. Tân Phú, TP.HCM"
                    }
                };
        }

    }
}