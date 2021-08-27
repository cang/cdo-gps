#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : PointController.cs
// Time Create : 11:08 AM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Core.Models.Tranfer.GpsCheckPoint;
using Datacenter.Api.Core;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.PointGps;
using StarSg.Utils.Models.Tranfer;

#endregion

namespace Datacenter.Api.Controllers
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
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPost]
        public PointGpsAdd Add(GpsCheckPointTranfer tran)
        {
            if (tran == null) return new PointGpsAdd {Description = "Thông tin gưởi lên null"};
            var company = Cache.GetCompanyById(tran.CompanyId);
            if (company == null) return new PointGpsAdd {Description = "Không tìm thấy thông tin công ty"};
            if (tran.Location == null) return new PointGpsAdd {Description = "Tọa độ ko được để trống"};

            var point = new PointGps
            {
                Name = tran.Name,
                Radius = tran.Radius,
                CompanyId = tran.CompanyId,
                GroupId = tran.GroupId,
                CreateTime = DateTime.Now,
                Type = tran.Type,
                Description = tran.Description,
                Location = new GpsLocation
                {
                    Lat = tran.Location.Lat,
                    Lng = tran.Location.Lng,
                    Address = tran.Location.Address
                },
                Id = 0
            };

            try
            {
                DataContext.Insert(point, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<PointGps>().Add(point, company.Id))
                    return new PointGpsAdd {Description = "Thêm thông tin điểm vào cache không thành công"};
                return new PointGpsAdd {Status = 1, Description = "Thêm thông tin điểm thành công", Id = point.Id};
            }
            catch (Exception ex)
            {
                Log.Exception("PointController", ex, "Thêm thông tin điểm vào database không thành công");
                return new PointGpsAdd {Description = "Thêm thông tin vào điểm thành công"};
            }
        }

        /// <summary>
        ///     Cập nhật thông tin điểm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(int id, GpsCheckPointTranfer tran)
        {
            if (tran == null) return new BaseResponse {Description = "Thông tin gưởi lên null"};

            if (tran.Location == null) return new BaseResponse {Description = "Tọa độ ko được để trống"};
            var point = Cache.GetQueryContext<PointGps>().GetByKey(id);
            if (point == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin điểm"};

            point.Description = tran.Description;
            point.Name = tran.Name;
            point.Type = tran.Type;
            point.Radius = tran.Radius;
            point.Location.Lat = tran.Location.Lat;
            point.Location.Lng = tran.Location.Lng;
            point.Location.Address = tran.Location.Address;

            try
            {
                DataContext.Update(point, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse {Status = 1, Description = "Cập nhật thông tin điểm thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("PointController", ex, "Cập nhật điểm không thành công vào database");
                return new BaseResponse {Description = "Cập nhật thông tỉn điểm vào database không thành công"};
                throw;
            }
        }


        /// <summary>
        ///     xóa thông tin điểm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(int id)
        {
            var point = Cache.GetQueryContext<PointGps>().GetByKey(id);
            if (point == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin điểm"};

            try
            {
                DataContext.Delete(point, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<PointGps>().Del(point, point.CompanyId))
                    return new BaseResponse {Description = "Xóa điểm ra khỏi cache không thành công"};
                return new BaseResponse {Status = 1, Description = "Xóa thông tin điểm thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("PointController", ex,
                    $"Xóa điểm {point.Id} của công ty {point.CompanyId} không thành công");
                return new BaseResponse {Description = "Xóa thông tin điểm trong database không thành công"};
            }
        }

        /// <summary>
        ///     lấy thông tin điểm theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public PointGpsGetSingle GetById(int id)
        {
            var point = Cache.GetQueryContext<PointGps>().GetByKey(id);
            if (point == null)
                return new PointGpsGetSingle {Description = "Không tìm thấy thông tin điểm"};
            return new PointGpsGetSingle
            {
                Status = 1,
                Description = "OK",
                Point = new GpsCheckPointTranfer
                {
                    CompanyId = point.CompanyId,
                    Description = point.Description,
                    GroupId = point.GroupId,
                    Type = point.Type,
                    Id = point.Id,
                    Location = new GpsPoint
                    {
                        Address = point.Location.Address,
                        Lat = point.Location.Lat,
                        Lng = point.Location.Lng
                    },
                    Name = point.Name,
                    Radius = point.Radius
                }
            };
        }

        /// <summary>
        ///     lấy thông tin Điểm theo Nhóm
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId">Id Nhóm, không lọc nếu = 0</param>
        /// <returns></returns>
        [HttpGet]
        public PointGpsGetMulti GetByGroup(long companyId, long groupId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new PointGpsGetMulti { Description = "Không tìm thấy thông tin công ty" };
            return new PointGpsGetMulti
            {
                Status = 1,
                Description = "OK",
                Points =
                    Cache.GetQueryContext<PointGps>()
                        .GetByGroup(company.Id, groupId)
                       .Select(point => new GpsCheckPointTranfer
                       {
                           CompanyId = point.CompanyId,
                           Description = point.Description,
                           GroupId = point.GroupId,
                           Type = point.Type,
                           Id = point.Id,
                           Location = new GpsPoint
                           {
                               Address = point.Location.Address,
                               Lat = point.Location.Lat,
                               Lng = point.Location.Lng
                           },
                           Name = point.Name,
                           Radius = point.Radius
                       }).ToList()
            };
        }


        /// <summary>
        ///     Lấy thông tin điểm theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public PointGpsGetMulti GetByCompany(long companyId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new PointGpsGetMulti {Description = "Không tìm thấy thông tin công ty"};
            return new PointGpsGetMulti
            {
                Status = 1,
                Description = "OK",
                Points =
                    Cache.GetQueryContext<PointGps>()
                        .GetByCompany(company.Id)
                        .Select(point => new GpsCheckPointTranfer
                        {
                            CompanyId = point.CompanyId,
                            Description = point.Description,
                            GroupId = point.GroupId,
                            Type = point.Type,
                            Id = point.Id,
                            Location = new GpsPoint
                            {
                                Address = point.Location.Address,
                                Lat = point.Location.Lat,
                                Lng = point.Location.Lng
                            },
                            Name = point.Name,
                            Radius = point.Radius
                        }).ToList()
            };
        }


    }
}