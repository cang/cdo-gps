#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : CompanyController.cs
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
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Compnay;
using StarSg.Utils.Models.Tranfer;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin công ty
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Auth]
    public class CompanyController : BaseController
    {
        /// <summary>
        ///     thêm mới 1 công ty
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        [HttpPost]
        public CompanyAdd Add(CompanyTranfer com)
        {
            if (string.IsNullOrEmpty(com.Name))
                return new CompanyAdd {Status = 0, Description = "Không thể để trống tên công ty"};
            if (com.Location == null)
                return new CompanyAdd {Description = "Không để trống tạo độ và địa chỉ của công ty"};

            var company = new Company();
            company.Name = com.Name;
            company.TimeCreate = DateTime.Now;
            company.Description = com.Description;
            company.ShortName = com.ShortName;
            company.DbId = com.DbId;
            company.Setting = new CompanySetting {Company = company, Id = company.Id, TimeoutHidenDevice = 60 * 24 * 7, TimeoutLostDevice = 120 };
            //todo: cài đặt các thông tin mặc định cho công ty ở đây
            company.Location = new GpsLocation
            {
                Lat = com.Location.Lat,
                Lng = com.Location.Lng,
                Address = com.Location.Address
            };

            company.Type = com.Type;

            try
            {
                // thêm công ty vào database
                DataContext.Insert(company, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                // thêm công ty vào cache

                if (!Cache.AddCompany(company))
                {
                    return new CompanyAdd
                    {
                        Description = "Thêm công ty vào cache hệ thống lỗi . vui lòng liên hệ quảng trị"
                    };
                }

                return new CompanyAdd {Status = 1, CompanyId = company.Id, Description = "Thêm công ty thành công "};
            }
            catch (Exception ex)
            {
                Log.Exception("CompanyController", ex, "Thêm công ty lỗi");
                return new CompanyAdd
                {
                    Description = "Không thể thêm công ty vào database . vui lòng liên hệ quản trị"
                };
            }
        }

        /// <summary>
        ///     cập nhật công ty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="com"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, CompanyTranfer com)
        {
            if (id == 0) return new BaseResponse {Description = "id công ty không hợp lệ"};

            var company = Cache.GetCompanyById(id);
            if (company == null)
                return new BaseResponse {Description = "Không tìm thấy thống tin công ty trên cache"};
            if (string.IsNullOrEmpty(com.Name))
                return new CompanyAdd {Status = 0, Description = "Không thể để trống tên công ty"};
            if (com.Location == null)
                return new CompanyAdd {Description = "Không để trống tạo độ và địa chỉ của công ty"};

            company.Name = com.Name;
            company.TimeCreate = DateTime.Now;
            company.Description = com.Description;
            company.ShortName = com.ShortName;
            company.DbId = com.DbId;
            company.Location.Lat = com.Location.Lat;
            company.Location.Lng = com.Location.Lng;
            company.Location.Address = com.Location.Address;

            company.Type = com.Type;

            try
            {
                DataContext.Update(company, MotherSqlId);
                DataContext.Commit();
                return new BaseResponse {Status = 1, Description = "Cập nhật thông tin công ty thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("CompanyController", ex, "Cập nhật công ty lỗi");
                return new BaseResponse
                {
                    Description = "Cập nhật thông tin vào database lỗi . vui long liên hệ quản trị"
                };
            }
        }

        /// <summary>
        ///     xóa thông tin công ty
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Delete(long id)
        {
            if (id == 0) return new BaseResponse {Description = "Id Công ty không hợp lệ"};

            var company = Cache.GetCompanyById(id);
            if (company == null)
                return new BaseResponse {Description = "Không tìm thấy thống tin công ty trên cache"};

            // xóa dự liệu trong cache
            if (!Cache.RemoveCompany(id))
                return new BaseResponse {Description = "Xóa thông tin trong cache lỗi , vui long liên hệ quảng trị"};

            // xóa dự liệu trong databse
            try
            {
                DataContext.Delete(company, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse
                {
                    Status = 1,
                    Description = $"Xóa thành công công ty  {company.Id} - {company.Name}"
                };
            }
            catch (Exception e)
            {
                Log.Exception("CompanyController", e, "Xóa công ty lỗi");
                return new BaseResponse {Description = "Xóa thông tin trong database lỗi. vui long liên hệ quản trị"};
            }
        }

        /// <summary>
        ///     Lấy thông tin công ty theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public CompanyGetSignal GetById(long id)
        {
            var company = Cache.GetCompanyById(id);
            if (company == null) return new CompanyGetSignal {Description = "Không tìm thấy thông tin công ty"};
            return new CompanyGetSignal
            {
                Status = 1,
                Description = "OK",
                Company = new CompanyGet
                {
                    CreateTime = company.TimeCreate,
                    DbId = company.DbId,
                    Description = company.Description,
                    Id = company.Id,
                    Location = new GpsPoint
                    {
                        Lat = company.Location.Lat,
                        Lng = company.Location.Lng,
                        Address = company.Location.Address
                    },
                    Name = company.Name,
                    ShortName = company.ShortName,

                    Type = company.Type
                }
            };
        }

        /// <summary>
        ///     Lấy toàn bộ thông tin công ty trên cụm máy chủ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CompanyGetAll GetAll()
        {
            return new CompanyGetAll
            {
                Status = 1,
                Description = "OK",
                Companies = Cache.GetAllCompany().Select(company => new CompanyGet
                {
                    CreateTime = company.TimeCreate,
                    DbId = company.DbId,
                    Description = company.Description,
                    Id = company.Id,
                    Location = new GpsPoint
                    {
                        Lat = company.Location.Lat,
                        Lng = company.Location.Lng,
                        Address = company.Location.Address
                    },
                    Name = company.Name,
                    ShortName = company.ShortName,

                    Type = company.Type

                }).ToList()
            };
        }
    }
}