#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : AreaController.cs
// Time Create : 2:29 PM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Core.Models.Tranfer.CheckZone;
using Datacenter.Api.Core;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using Datacenter.Model.Utils;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Area;
using StarSg.Utils.Models.Tranfer.CheckZone;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin vùng
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AreaController : BaseController
    {
        /// <summary>
        ///     thêm vùng
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPost]
        public AreaAdd Add(CheckZoneTranfer tran)
        {
            if (tran == null) return new AreaAdd {Description = "Thông tin gưởi lên null"};
            var company = Cache.GetCompanyById(tran.CompanyId);
            if (company == null) return new AreaAdd {Description = "Không tìm thấy thông tin công ty"};
            if (tran.Points == null || tran.Points.Count < 3)
                return new AreaAdd {Description = "Tọa độ ko được để trống hoặc không đủ 3 điểm"};

            var area = new Area();
            area.Description = tran.Description;
            area.CompanyId = company.Id;
            area.GroupId = tran.GroupId;
            area.CreateTime = DateTime.Now;
            area.MaxDevice = tran.MaxDevice;
            area.MaxSpeed = tran.MaxSpeed;
            area.Name = tran.Name;
            area.Type = tran.Type;
            area.Address = tran.Address;

            area.Points =
                tran.Points.Select(m => new GpsLocation {Lat = m.Lat, Lng = m.Lng}).ToList().PointListToString();
            try
            {
                DataContext.Insert(area, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<Area>().Add(area, company.Id))
                    return new AreaAdd {Description = "Thêm thông tin vùng vào cache ko thành công"};
                return new AreaAdd {Status = 1, Description = "Thêm thông tin vùng thành công", Id = area.Id};
            }
            catch (Exception ex)
            {
                Log.Exception("AreaController", ex, " Thêm thông tin vùng");
                return new AreaAdd {Description = "Thêm vùng vào databse không thành công"};
            }
        }

        /// <summary>
        ///     Cập nhật thông tin Vùng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(int id, CheckZoneTranfer tran)
        {
            if (tran == null) return new BaseResponse { Description = "Thông tin gưởi lên null" };
            if (tran.Points == null || tran.Points.Count < 3)
                return new AreaAdd { Description = "Tọa độ ko được để trống hoặc không đủ 3 điểm" };

            var area = Cache.GetQueryContext<Area>().GetByKey(id);
            if (area == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin Vùng" };

            area.Description = tran.Description;
            //area.CompanyId = company.Id;
            //area.GroupId = tran.GroupId;
            //area.CreateTime = DateTime.Now;
            area.MaxDevice = tran.MaxDevice;
            area.MaxSpeed = tran.MaxSpeed;
            area.Name = tran.Name;
            area.Type = tran.Type;
            area.Address = tran.Address;

            area.Points =
                tran.Points.Select(m => new GpsLocation { Lat = m.Lat, Lng = m.Lng }).ToList().PointListToString();

            try
            {
                DataContext.Update(area, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "Cập nhật thông tin Vùng thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("PointController", ex, "Cập nhật Vùng không thành công vào database");
                return new BaseResponse { Description = "Cập nhật thông tỉn Vùng vào database không thành công" };
                throw;
            }
        }

        /// <summary>
        ///     xóa vùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(int id)
        {
            var area = Cache.GetQueryContext<Area>().GetByKey(id);
            if (area == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin vùng"};

            try
            {
                DataContext.Delete(area, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<Area>().Del(area, area.CompanyId))
                    return new BaseResponse {Description = "Xóa vùng ra khỏi cache không thành công"};
                return new BaseResponse {Status = 1, Description = "Xóa thông tin vùng thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("PointController", ex,
                    $"Xóa vùng {area.Id} của công ty {area.CompanyId} không thành công");
                return new BaseResponse {Description = "Xóa thông tin vùng trong database không thành công"};
            }
        }

        /// <summary>
        ///     lấy thông tin vùng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public AreaGetSingle GetById(int id)
        {
            var area = Cache.GetQueryContext<Area>().GetByKey(id);
            if (area == null)
                return new AreaGetSingle {Description = "Không tìm thấy thông tin vùng"};
            return new AreaGetSingle
            {
                Status = 1,
                Description = "OK",
                Area = new CheckZoneTranfer
                {
                    CompanyId = area.CompanyId,
                    GroupId = area.GroupId,
                    Description = area.Description,
                    Id = area.Id,
                    Points = area.Points.StringToPointList().Select(m => new Point {Lat = m.Lat, Lng = m.Lng}).ToList(),
                    Name = area.Name,
                    MaxSpeed = area.MaxSpeed,
                    MaxDevice = area.MaxDevice,
                    Type = area.Type,
                    Address = area.Address
                }
            };
        }

        /// <summary>
        ///     lấy thông tin vùng theo Nhóm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <returns></returns>
        [HttpGet]
        public AreaGetMulti GetByGroup(long companyId,long groupId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new AreaGetMulti { Description = "Không tìm thấy thông tin công ty" };
            return new AreaGetMulti
            {
                Status = 1,
                Description = "OK",
                Areas =
                    Cache.GetQueryContext<Area>()
                        .GetByGroup(company.Id,groupId)
                        .Select(area => new CheckZoneTranfer
                        {
                            CompanyId = area.CompanyId,
                            GroupId = area.GroupId,
                            Description = area.Description,
                            Id = area.Id,
                            Points =
                                area.Points.StringToPointList()
                                    .Select(m => new Point { Lat = m.Lat, Lng = m.Lng })
                                    .ToList(),
                            Name = area.Name,
                            MaxSpeed = area.MaxSpeed,
                            MaxDevice = area.MaxDevice,
                            Type = area.Type,
                            Address = area.Address
                        }).ToList()
            };
        }

        /// <summary>
        ///     lấy thông tin vùng theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public AreaGetMulti GetByCompany(long companyId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new AreaGetMulti {Description = "Không tìm thấy thông tin công ty"};
            return new AreaGetMulti
            {
                Status = 1,
                Description = "OK",
                Areas =
                    Cache.GetQueryContext<Area>()
                        .GetByCompany(company.Id)
                        .Select(area => new CheckZoneTranfer
                        {
                            CompanyId = area.CompanyId,
                            GroupId = area.GroupId,
                            Description = area.Description,
                            Id = area.Id,
                            Points =
                                area.Points.StringToPointList()
                                    .Select(m => new Point {Lat = m.Lat, Lng = m.Lng})
                                    .ToList(),
                            Name = area.Name,
                            MaxSpeed = area.MaxSpeed,
                            MaxDevice = area.MaxDevice,
                            Type = area.Type,
                            Address = area.Address
                        }).ToList()
            };
        }

    }
}