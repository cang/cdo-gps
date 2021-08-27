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
using StarSg.Utils.Models.DatacenterResponse.SpecialTour;
using StarSg.Utils.Models.Tranfer;
using StarSg.Utils.Models.DatacenterResponse.Area;
using System.Collections.Generic;
using System.Linq.Expressions;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin cuốc đặc biệt
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SpecialTourController : BaseController
    {
        /// <summary>
        ///     Thêm cuốc đặc biệt
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPost]
        public AreaAdd Add(SpecialTourTranfer tran)
        {
            if (tran == null) return new AreaAdd {Description = "Thông tin gưởi lên null"};

            var device = Cache.GetQueryContext<Device>().GetByKey(tran.Serial);
            if (device == null) return new AreaAdd { Description = "Không tìm thấy thông tin thiết bị" };

            //var company = Cache.GetCompanyById(device.CompanyId);
            //if (company == null) return new AreaAdd {Description = "Không tìm thấy thông tin công ty"};

            SpecialTour obj = new SpecialTour()
            {
                Serial = tran.Serial,
                Date = tran.Date,
                HowTimes = tran.HowTimes,
                Address = tran.Address,
                KmOnPlan = tran.KmOnPlan,
                Note = tran.Note,
                CompanyId = device.CompanyId,
                GroupId = device.GroupId,
                UpdateTime = DateTime.Now
            };

            try
            {
                DataContext.Insert(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);

                //if (!Cache.GetQueryContext<SpecialTour>().Add(obj, company.Id))
                //    return new AreaAdd {Description = "Thêm thông tin cuốc đặc biệt ko thành công" };

                return new AreaAdd {Status = 1, Description = "Thêm thông tin cuốc đặc biệt thành công", Id = obj.Id};
            }
            catch (Exception ex)
            {
                Log.Exception("SpecialTourController", ex, "Thêm thông tin cuốc đặc biệt");
                return new AreaAdd {Description = "Thêm cuốc đặc biệt vào databse không thành công" };
            }
        }

        /// <summary>
        ///     Cập nhật thông tin cuốc đặc biệt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, SpecialTourTranfer tran)
        {
            if (tran == null) return new BaseResponse { Description = "Thông tin gưởi lên null" };

            var device = Cache.GetQueryContext<Device>().GetByKey(tran.Serial);
            if (device == null) return new BaseResponse { Description = "Không tìm thấy thông tin thiết bị" };

            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null) return new BaseResponse { Description = "Không tìm thấy thông tin công ty" };

            SpecialTour obj = DataContext.Get<SpecialTour>(id, company.DbId);
            if (obj == null) return new BaseResponse { Description = "Không tìm thấy cuốc đặc biệt" };

            if( (DateTime.Now -  obj.UpdateTime).TotalMinutes > 120)
                return new BaseResponse { Description = "Quá thời gian 120 phút sau khi thêm, không cho sửa" };

            obj.HowTimes = tran.HowTimes;
            obj.Date = tran.Date;
            obj.Address = tran.Address;
            obj.KmOnPlan = tran.KmOnPlan;
            obj.Note = tran.Note;
            obj.UpdateTime = DateTime.Now;
            obj.GroupId = device.GroupId;
            obj.CompanyId = device.CompanyId;

            try
            {
                DataContext.Update(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "Cập nhật thông tin cuốc đặc biệt thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("SpecialTourController", ex, "Cập nhật cuốc đặc biệt không thành công vào database");
                return new BaseResponse { Description = "Cập nhật thông tin cuốc đặc biệt vào database không thành công" };
                throw;
            }
        }

        /// <summary>
        ///     Xóa cuốc đặc biệt
        /// </summary>
        /// <param name="idcompanyId></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, long id)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new BaseResponse { Description = "Không tìm thấy thông tin công ty" };

            SpecialTour obj = DataContext.Get<SpecialTour>(id, company.DbId);
            if (obj == null) return new BaseResponse { Description = "Không tìm thấy cuốc đặc biệt" };

            try
            {
                DataContext.Delete(obj, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                //if (!Cache.GetQueryContext<Area>().Del(obj, company.CompanyId))
                //    return new BaseResponse {Description = "Xóa cuốc đặc biệt ra khỏi cache không thành công" };

                return new BaseResponse {Status = 1, Description = "Xóa cuốc đặc biệt thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("PointController", ex,
                    $"Xóa cuốc đặc biệt {obj.Id} của công ty {company.Id} không thành công");
                return new BaseResponse {Description = "Xóa cuốc đặc biệt trong database không thành công" };
            }
        }

        /// <summary>
        /// Lấy cuốc đặc biệt theo id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGet GetById(long companyId, long id)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new SpecialTourGet { Description = "Không tìm thấy thông tin công ty" };

            SpecialTour ret = DataContext.Get<SpecialTour>(id, company.DbId);
            if(ret==null) return new SpecialTourGet { Description = "Không tìm thấy cuốc đặc biệt" };

            var device = Cache.GetQueryContext<Device>().GetByKey(ret.Serial);

            return new SpecialTourGet
            {
                Status = 1,
                Description = "OK",
                Data = new SpecialTourTranfer()
                {
                    Id = ret.Id,
                    Date = ret.Date,
                    HowTimes = ret.HowTimes,
                    KmOnPlan = ret.KmOnPlan,
                    Serial = ret.Serial,
                    UpdateTime = ret.UpdateTime,
                    Address = ret.Address,
                    Note = ret.Note,
                    Bs = device?.Bs ?? "",
                    GroupId = ret.GroupId,
                    CompanyId = ret.CompanyId
                }
            };
        }

        /// <summary>
        /// Lấy cuốc đặc biệt theo serial và ngày
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGetMulti GetBySerial(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new SpecialTourGetMulti { Description = "Không tồn tại thiết bị này" };
            return GetReports(device.CompanyId, 0, serial.ToString(), begin, end);
        }

        /// <summary>
        ///     Lấy cuốc đặc biệt tổng quát
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">Danh sách serial cách nhau | hoặc , hoặc ; hoặc kí tự trống, nếu = "" thì lấy hết theo groupId </param>
        /// <param name="serial">serial, nếu = 0 thì lấy hết theo ids</param>
        /// <returns></returns>
        [HttpGet]
        public SpecialTourGetMulti GetReports(long companyId, DateTime begin, DateTime end, long groupId, string seriallist, long serial)
        {
            if (serial > 0)
                return GetReports(companyId, groupId, serial.ToString(), begin, end);
            else if (!String.IsNullOrWhiteSpace(seriallist))
                return GetReports(companyId, groupId, seriallist, begin, end);
            else
                return GetReports(companyId, groupId, "", begin, end);
        }

        /// <summary>
        /// Lấy cuốc đặc biệt theo serial list , nhóm hoặc cty
        /// </summary>
        /// <param name="companyId">Mã công ty</param>
        /// <param name="groupId">Mã nhóm : nếu = 0 thì lấy hết</param>
        /// <param name="seriallist">seriallist, nếu = "" thì lấy hết</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private SpecialTourGetMulti GetReports(long companyId, long groupId, string seriallist, DateTime begin, DateTime end)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new SpecialTourGetMulti { Description = "Công ty chưa thiết bị không tồn tại" };

            IList<Device> allDevice;
            List<SpecialTour> alltrace = new List<SpecialTour>();

            //Tìm theo danh sách serial list
            if (!String.IsNullOrWhiteSpace(seriallist))
            {
                var allSeial = new List<long>();
                try
                {
                    allSeial = seriallist.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
                }
                catch (Exception)
                {
                    return new SpecialTourGetMulti { Description = "Thông tin seriallist truyền lên không hợp lệ" };
                }
                if (allSeial.Count == 0) return new SpecialTourGetMulti { Description = "Danh sách serial rỗng" };
                allDevice = allSeial.Select(m => Cache.GetQueryContext<Device>().GetByKey(m)).Where(m => m != null).ToList();

                alltrace.AddRange(
                DataContext.CreateQuery<SpecialTour>(company.DbId)
                    .Where(m => m.Date >= begin &&
                                m.Date <= end)
                    .WhereOr(
                        allDevice.Select(m => (Expression<Func<SpecialTour, bool>>)(x => x.Serial == m.Serial))
                            .ToArray()
                    ).Execute());
            }
            //Tìm theo nhóm
            else if (groupId > 0)
            {
                allDevice = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId);
                alltrace.AddRange(
                   DataContext.CreateQuery<SpecialTour>(company.DbId)
                       .Where(m => m.Date >= begin &&
                                m.Date <= end &&
                                m.CompanyId == companyId &&
                                m.GroupId == groupId
                                ).Execute());
            }
            //Tìm theo cty
            else
            {
                allDevice = Cache.GetQueryContext<Device>().GetByCompany(companyId);
                alltrace.AddRange(
                   DataContext.CreateQuery<SpecialTour>(company.DbId)
                       .Where(m => m.Date >= begin &&
                                m.Date <= end &&
                                m.CompanyId == companyId
                                ).Execute());
            }

            if (allDevice == null || allDevice.Count <= 0) return new SpecialTourGetMulti()
            {
                Status = 1,
                Description = "Không có dữ liệu",
                Datas = new List<SpecialTourTranfer>(0)
            };

            List<SpecialTourTranfer> rets = new List<SpecialTourTranfer>();
            foreach (var device in allDevice)
            {
                List<SpecialTour> logs = alltrace.FindAll(m => m.Serial == device.Serial).OrderBy(m => m.Date).ToList();
                rets.AddRange(
                    logs.Select(m => new SpecialTourTranfer()
                    {
                        Id = m.Id,
                        Date = m.Date,
                        HowTimes = m.HowTimes,
                        KmOnPlan = m.KmOnPlan,
                        Serial = m.Serial,
                        UpdateTime = m.UpdateTime,
                        Address = m.Address,
                        Note = m.Note,
                        Bs = device.Bs,
                        GroupId = m.GroupId,
                        CompanyId = m.CompanyId
                    })
                );

            }

            return new SpecialTourGetMulti()
            {
                Status = 1,
                Description = "OK",
                Datas = rets
            };

        }

    }
}