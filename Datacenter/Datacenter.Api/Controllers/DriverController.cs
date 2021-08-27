#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DriverController.cs
// Time Create : 3:39 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Core.Models.Tranfer.Driver;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Driver;

#endregion

namespace Datacenter.Api.Controllers
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
        /// <param name="dr"></param>
        /// <returns></returns>
        [HttpPost]
        public DriverAdd Add(DriverTranfer dr)
        {
            if (string.IsNullOrEmpty(dr.Name))
                return new DriverAdd {Description = $"Tên tài xế không được để trống"};
            if (string.IsNullOrEmpty(dr.Gplx))
                return new DriverAdd {Description = $"Tên tài xế không được để giấy phép lái xe"};

            var company = Cache.GetCompanyById(dr.CompanyId);
            if (company == null)
                return new DriverAdd {Description = "Không tìm thấy thông tin công ty"};

            DeviceGroup gr = null;
            if (dr.GroupId > 0)
            {
                gr = Cache.GetQueryContext<DeviceGroup>().GetByKey(dr.GroupId);
                if (gr == null)
                    return new DriverAdd {Description = "Không tìm thấy đội xe"};
            }

            var driver = new Driver();
            driver.Name = dr.Name;
            driver.Address = dr.Address;
            driver.AddressOfGplx = dr.AddressOfGplx;
            driver.Born = dr.Born;
            driver.Cmnd = dr.Cmnd;
            driver.CompanyId = dr.CompanyId;
            driver.GroupId = dr.GroupId;
            driver.CreateDateOfGplx = dr.CreateDateOfGplx;
            driver.EndDateOfGplx = dr.EndDateOfGplx;
            driver.Gplx = dr.Gplx;
            driver.Mnv = dr.Mnv;
            driver.Phone = dr.Phone;
            driver.Sex = dr.Sex;

            try
            {
                DataContext.Insert(driver, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<Driver>().Add(driver, company.Id))
                    return new DriverAdd {Description = "Thêm thông tin lái xe vào cache ko thành công"};
                return new DriverAdd {Id = driver.Id, Status = 1, Description = "Thêm lái xe thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DriverController", ex,
                    $"Thêm thông tin tài xế {dr.Name} vảo công ty {company.Name} không thành công");

                return new DriverAdd {Description = "Thêm thông tin tài xế vào database không thành công"};
            }
        }

        /// <summary>
        ///     cập nhật thông tin lái xe
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, DriverTranfer dr)
        {
            if (string.IsNullOrEmpty(dr.Name))
                return new BaseResponse {Description = $"Tên tài xế không được để trống"};
            if (string.IsNullOrEmpty(dr.Gplx))
                return new BaseResponse {Description = $"Tên tài xế không được để giấy phép lái xe"};

            var company = Cache.GetCompanyById(dr.CompanyId);
            if (company == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin công ty"};

            DeviceGroup gr = null;
            if (dr.GroupId > 0)
            {
                gr = Cache.GetQueryContext<DeviceGroup>().GetByKey(dr.GroupId);
                if (gr == null)
                    return new BaseResponse {Description = "Không tìm thấy đội xe"};
            }

            var driver = Cache.GetQueryContext<Driver>().GetByKey(id);
            if (driver == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin tài xế"};
            driver.Name = dr.Name;
            driver.Address = dr.Address;
            driver.AddressOfGplx = dr.AddressOfGplx;
            driver.Born = dr.Born;
            driver.Cmnd = dr.Cmnd;
            driver.CompanyId = dr.CompanyId;
            driver.GroupId = dr.GroupId;
            driver.CreateDateOfGplx = dr.CreateDateOfGplx;
            driver.EndDateOfGplx = dr.EndDateOfGplx;
            driver.Gplx = dr.Gplx;
            driver.Mnv = dr.Mnv;
            driver.Phone = dr.Phone;
            driver.Sex = dr.Sex;

            try
            {
                DataContext.Update(driver, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse {Status = 1, Description = "Cập nhật thông tin lái xe thahf công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DriverController", ex, $"cập nhật thông tin lái xe {id} vào database ko thành công");
                return new BaseResponse {Description = "Cập nhật thông tin lái xe vào database không thành công"};
            }
        }

        /// <summary>
        ///     xóa thông tin lái xe
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long id)
        {
            var driver = Cache.GetQueryContext<Driver>().GetByKey(id);
            if (driver == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin lái xe "};
            try
            {
                DataContext.Delete(driver, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<Driver>().Del(driver, driver.CompanyId))
                    return new BaseResponse {Description = "Xóa thông tin driver trên cache không thành công"};
                return new BaseResponse {Status = 1, Description = "Xóa thông tin lái xe thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DriverController", ex, $"xóa thông tin lái xe {id} không thành công");
                return new BaseResponse {Description = "Xóa thông tin lái xe trong database không thành công"};
            }
        }


        /// <summary>
        ///     lấy thông tin lái xe theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverGetSingle GetDriverById(long id)
        {
            var driver = Cache.GetQueryContext<Driver>().GetByKey(id);
            if (driver == null)
                return new DriverGetSingle {Description = "Không tìm thấy thông tin lái xe "};
            return new DriverGetSingle
            {
                Status = 1,
                Description = "OK",
                Driver = new DriverTranfer
                {
                    Id = driver.Id,
                    Name = driver.Name,
                    Phone = driver.Phone,
                    AddressOfGplx = driver.AddressOfGplx,
                    Address = driver.Address,
                    Born = driver.Born,
                    Cmnd = driver.Cmnd,
                    CompanyId = driver.CompanyId,
                    CreateDateOfGplx = driver.CreateDateOfGplx,
                    EndDateOfGplx = driver.EndDateOfGplx,
                    Sex = driver.Sex,
                    GroupId = driver.GroupId,
                    Gplx = driver.Gplx,
                    Mnv = driver.Mnv
                }
            };
        }

        /// <summary>
        ///     lấy thông tin tài xế theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverGetMulti GetDriverByCompany(long companyId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null)
                return new DriverGetMulti {Description = "Không tìm thấy thông tin công ty"};
            return new DriverGetMulti
            {
                Status = 1,
                Description = "OK",
                Drivers = Cache.GetQueryContext<Driver>().GetByCompany(companyId).Select(
                    driver => new DriverTranfer
                    {
                        Id = driver.Id,
                        Name = driver.Name,
                        Phone = driver.Phone,
                        AddressOfGplx = driver.AddressOfGplx,
                        Address = driver.Address,
                        Born = driver.Born,
                        Cmnd = driver.Cmnd,
                        CompanyId = driver.CompanyId,
                        CreateDateOfGplx = driver.CreateDateOfGplx,
                        EndDateOfGplx = driver.EndDateOfGplx,
                        Sex = driver.Sex,
                        GroupId = driver.GroupId,
                        Gplx = driver.Gplx,
                        Mnv = driver.Mnv
                    }).ToList()
            };
        }

        /// <summary>
        ///     Lấy thông tin lái xe theo đội xe
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        public DriverGetMulti GetDriverByGroupId(long groupId)
        {
            var gr = Cache.GetQueryContext<DeviceGroup>().GetByKey(groupId);
            if (gr == null)
                return new DriverGetMulti {Description = "Không tìm thấy đội xe"};
            return new DriverGetMulti
            {
                Status = 1,
                Description = "OK",
                Drivers = Cache.GetQueryContext<Driver>().GetByGroup(gr.CompnayId, gr.Id).Select(
                    driver => new DriverTranfer
                    {
                        Id = driver.Id,
                        Name = driver.Name,
                        Phone = driver.Phone,
                        AddressOfGplx = driver.AddressOfGplx,
                        Address = driver.Address,
                        Born = driver.Born,
                        Cmnd = driver.Cmnd,
                        CompanyId = driver.CompanyId,
                        CreateDateOfGplx = driver.CreateDateOfGplx,
                        EndDateOfGplx = driver.EndDateOfGplx,
                        Sex = driver.Sex,
                        GroupId = driver.GroupId,
                        Gplx = driver.Gplx,
                        Mnv = driver.Mnv
                    }).ToList()
            };
        }
    }
}