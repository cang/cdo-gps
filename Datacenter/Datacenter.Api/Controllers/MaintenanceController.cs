#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : MaintenanceController.cs
// Time Create : 8:24 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Maintenance;
using StarSg.Utils.Models.Tranfer.Maintenance;
using StarSg.Utils.Models.DatacenterResponse.Enterprise;
using StarSg.Utils.Models.Tranfer.DeviceManager;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin bảo trì của thiết bị
    /// </summary>
    [Auth, Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class MaintenanceController : BaseController
    {
        /// <summary>
        ///     cập nhật thông tin bảo trì thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyModelSpecification"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long id, [FromBody] ModelSpecificationTranfer companyModelSpecification)
        {
            if (companyModelSpecification != null)
            {
                try
                {
                    var oldData = DataContext.Get<CompanyModelSpecification>(id, MotherSqlId);
                    if (oldData == null)
                        return new BaseResponse { Description = "Không tìm thấy thông tin" };
                    oldData.KmDaoLop = companyModelSpecification.KmDaoLop;
                    oldData.KmThayLocDau = companyModelSpecification.KmThayLocDau;
                    oldData.KmThayLocGio = companyModelSpecification.KmThayLocGio;
                    oldData.KmThayLocNhot = companyModelSpecification.KmThayLocNhot;
                    oldData.KmThayNhot = companyModelSpecification.KmThayNhot;
                    oldData.KmThayVo = companyModelSpecification.KmThayVo;
                    DataContext.Update(oldData, MotherSqlId);
                    DataContext.Commit(MotherSqlId);
                    return new BaseResponse { Status = 1, Description = "Cập nhật thành công" };
                }
                catch (Exception exception)
                {
                    Log.Exception("Maintenance", exception, "Cập nhật thông tin bảo trì lỗi");
                    return new BaseResponse { Description = "Không thể cập nhật thông tin" };
                }
            }
            return new BaseResponse { Description = "Thông tin gưởi lên null" };
        }

        /// <summary>
        ///     lấy thông tin bảo trì theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public ModelSpecGetMulti GetByCompanyId([FromUri] long companyId)
        {
            try
            {
                //lấy cả device đã distint theo model xe
                var deviceList =
                    Cache.GetQueryContext<Device>()
                        .GetByCompany(companyId)
                        .ToList()
                        .Where(p => p.ModelName != null)
                        .GroupBy(p => p.ModelName)
                        .Select(g => g.First());
                //lấy tất cả model xe của công ty cấu hình
                var currentCompanyModelList =
                    DataContext.GetWhere<CompanyModelSpecification>(m => m.CompanyId == companyId,
                        MotherSqlId).ToList();
                //lấy tất cả model xe cấu hình mặc định
                var modelList =
                    DataContext.GetAll<string, DeviceModel>(m => m.Name, MotherSqlId).Values.ToList();
                foreach (var device in deviceList)
                {
                    //check model của xe đã có trong danh sách model của cty hay chưa
                    //nếu chưa có thì thêm mới vào database
                    if (currentCompanyModelList.Where(p => p.Name == device.ModelName).ToList().Count == 0)
                    {
                        var model = modelList.Where(p => p.Name == device.ModelName).ToList().FirstOrDefault();
                        if (model != null)
                        {
                            //add vào database
                            var md = new CompanyModelSpecification
                            {
                                KmThayLocNhot = model.KmThayLocNhot,
                                KmThayNhot = model.KmThayNhot,
                                KmThayVo = model.KmThayVo,
                                Name = model.Name,
                                CompanyId = companyId,
                                KmDaoLop = model.KmDaoLop,
                                KmThayLocDau = model.KmThayLocDau,
                                KmThayLocGio = model.KmThayLocGio
                            };
                            DataContext.Insert(md, MotherSqlId);
                            //add vào list data trả về cho front end
                            currentCompanyModelList.Add(md);
                        }
                    }
                }
                DataContext.Commit(MotherSqlId);
                return new ModelSpecGetMulti
                {
                    Status = 1,
                    Description = "OK",
                    Datas = currentCompanyModelList.Select(m => new ModelSpecificationTranfer
                    {
                        KmThayLocNhot = m.KmThayLocNhot,
                        KmThayNhot = m.KmThayNhot,
                        KmThayVo = m.KmThayVo,
                        Name = m.Name,
                        CompanyId = companyId,
                        KmDaoLop = m.KmDaoLop,
                        KmThayLocDau = m.KmThayLocDau,
                        KmThayLocGio = m.KmThayLocGio,
                        Id = m.Id
                    }).ToList()
                };
            }
            catch (Exception exception)
            {
                Log.Exception("Maintenance", exception, "");
            }
            return new ModelSpecGetMulti { Description = "Lấy thông tin lỗi" };
        }

        /// <summary>
        ///     xóa thông tin bảo trì thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Delete(long id, long companyId)
        {
            var deviceModel =
                DataContext.Get<CompanyModelSpecification>(id, MotherSqlId);
            if (deviceModel == null) return new BaseResponse { Description = "Không tìm thấy thông tin cần xóa" };
            if (deviceModel.CompanyId != companyId) return new BaseResponse { Description = "Không được phép xóa" };

            try
            {
                DataContext.Delete(deviceModel, MotherSqlId);
                DataContext.Commit(MotherSqlId);
            }
            catch (Exception e)
            {
                Log.Exception("Maintenance", e, "Xóa thông tin bảo trì lỗi");
                return new BaseResponse { Description = "Không thể xóa thông tin trong database" };
            }
            return new BaseResponse { Status = 1, Description = "Xóa thành công" };
        }


        /// <summary>
        ///     reset định mức bảo trì
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="optionName"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse ResetMaintenanceBySerial([FromUri] long serial, [FromUri] OptionNameType optionName)
        {
            try
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                if (device == null)
                    return new BaseResponse { Description = "Không tìm thấy thông tin thiết bị" };
                var company = Cache.GetCompanyById(device.CompanyId);
                long kmReset;
                //tạo giá trị mặc định khi null
                if (device.Specification == null)
                {
                    device.Specification = new Specification
                    {
                        Serial = device.Serial,
                        Device = device,
                        KmDaoLop = 0,
                        KmThayLocDau = 0,
                        KmThayLocGio = 0,
                        KmThayLocNhot = 0,
                        KmThayNhot = 0,
                        KmThayVo = 0
                    };

                    DataContext.Insert(device.Specification, MotherSqlId);
                    DataContext.Commit(MotherSqlId);
                }
                switch (optionName)
                {
                    case OptionNameType.DaoLop:
                        kmReset = device.Specification.KmDaoLop = device.Status.BasicStatus.TotalGpsDistance;
                        break;
                    case OptionNameType.ThayLocDau:
                        kmReset = device.Specification.KmThayLocDau = device.Status.BasicStatus.TotalGpsDistance;
                        break;
                    case OptionNameType.ThayLocGio:
                        kmReset = device.Specification.KmThayLocGio = device.Status.BasicStatus.TotalGpsDistance;
                        break;
                    case OptionNameType.ThayLocNhot:
                        kmReset = device.Specification.KmThayLocNhot = device.Status.BasicStatus.TotalGpsDistance;
                        break;
                    case OptionNameType.ThayNhot:
                        kmReset = device.Specification.KmThayNhot = device.Status.BasicStatus.TotalGpsDistance;
                        break;
                    case OptionNameType.ThayVo:
                        kmReset = device.Specification.KmThayVo = device.Status.BasicStatus.TotalGpsDistance;
                        break;
                    default:
                        return new BaseResponse { Description = "OptionName ko tồn tại" };
                }
                var changeSpecLog = new ChangeSpecificationLog
                {
                    Serial = serial,
                    TimeUpdate = DateTime.Now,
                    OptionName = optionName,
                    KmReset = kmReset,
                    DbId = company.DbId
                };
                //update database
                DataContext.Update(device.Specification, MotherSqlId);
                DataContext.Insert(changeSpecLog, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "Reset thông tin thành công" };
            }
            catch (Exception e)
            {
                Log.Exception("Maintenance", e, "");
            }
            return new BaseResponse { Description = "Không thể reset" };
        }

        /// <summary>
        ///     lấy báo cáo bảo trì theo serial
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpGet]
        public MaintenanceReportGet MaintenanceReportBySerial([FromUri] long serial)
        {
            try
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                if (device == null)
                {
                    return new MaintenanceReportGet { Description = "Thiết bị không tồn tại" };
                }
                var companyModelSpec =
                    DataContext.GetWhere<CompanyModelSpecification>(m => m.CompanyId == device.CompanyId
                                                                         && m.Name == device.ModelName,
                        MotherSqlId).ToList().FirstOrDefault();

                var result = new MaintenanceReportTranfer
                {
                    Serial = device.Serial,
                    ModelName = device.ModelName,
                    KmDaoLopCurrent =
                        (device.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                        (device.Specification?.KmDaoLop ?? 0),
                    KmDaoLopLimit = companyModelSpec?.KmDaoLop ?? 0,
                    KmThayLocDauCurrent =
                        (device.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                        (device.Specification?.KmThayLocDau ?? 0),
                    KmThayLocDauLimit = companyModelSpec?.KmThayLocDau ?? 0,
                    KmThayLocGioCurrent =
                        (device.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                        (device.Specification?.KmThayLocGio ?? 0),
                    KmThayLocGioLimit = companyModelSpec?.KmThayLocGio ?? 0,
                    KmThayLocNhotCurrent =
                        (device.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                        (device.Specification?.KmThayLocNhot ?? 0),
                    KmThayLocNhotLimit = companyModelSpec?.KmThayLocNhot ?? 0,
                    KmThayNhotCurrent =
                        (device.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                        (device.Specification?.KmThayNhot ?? 0),
                    KmThayNhotLimit = companyModelSpec?.KmThayNhot ?? 0,
                    KmThayVoCurrent =
                        (device.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                        (device.Specification?.KmThayVo ?? 0),
                    KmThayVoLimit = companyModelSpec?.KmThayVo ?? 0,
                    Bs = device.Bs
                };
                return new MaintenanceReportGet
                {
                    Status = 1,
                    Datas = new List<MaintenanceReportTranfer> { result },
                    Description = "OK"
                };
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "hàm MaintenanceReportBySerial");
            }
            return new MaintenanceReportGet { Description = "Lấy thông tin lỗi" };
        }

        private CompanyModelSpecification GetModelSpecByName(string name,
            List<CompanyModelSpecification> modelSpecList)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            if (modelSpecList == null)
                return null;
            foreach (var node in modelSpecList)
            {
                if (name.Equals(node.Name))
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        ///     lấy báo cáo bảo trì theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public MaintenanceReportGet MaintenanceReportByCompany(long companyId)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company == null) return new MaintenanceReportGet { Description = "Không tìm thấy công ty" };
                var modelSpecByCompany = DataContext.GetWhere<CompanyModelSpecification>(m => m.CompanyId == companyId,
                    MotherSqlId).ToList();
                var result = Cache.GetQueryContext<Device>().GetByCompany(companyId).Select(m =>
                {
                    var companyModelSpec = GetModelSpecByName(m.ModelName, modelSpecByCompany);
                    return new MaintenanceReportTranfer
                    {
                        Serial = m.Serial,
                        ModelName = m.ModelName,
                        KmDaoLopCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmDaoLop ?? 0),
                        KmDaoLopLimit = companyModelSpec?.KmDaoLop ?? 0,
                        KmThayLocDauCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayLocDau ?? 0),
                        KmThayLocDauLimit = companyModelSpec?.KmThayLocDau ?? 0,
                        KmThayLocGioCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayLocGio ?? 0),
                        KmThayLocGioLimit = companyModelSpec?.KmThayLocGio ?? 0,
                        KmThayLocNhotCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayLocNhot ?? 0),
                        KmThayLocNhotLimit = companyModelSpec?.KmThayLocNhot ?? 0,
                        KmThayNhotCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayNhot ?? 0),
                        KmThayNhotLimit = companyModelSpec?.KmThayNhot ?? 0,
                        KmThayVoCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayVo ?? 0),
                        KmThayVoLimit = companyModelSpec?.KmThayVo ?? 0,
                        Bs = m.Bs
                    };
                }).ToList();
                return new MaintenanceReportGet { Status = 1, Description = "OK", Datas = result };
            }
            catch (Exception e)
            {
                Log.Exception("Maintenance", e, "");
            }
            return new MaintenanceReportGet { Description = "Lỗi dữ liệu" };
        }

        /// <summary>
        ///     lấy báo cáo bảo trì thiết bị theo group
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        public MaintenanceReportGet MaintenanceReportByGroup(long companyId, long groupId)
        {
            try
            {
                var company = Cache.GetCompanyById(companyId);
                if (company == null) return new MaintenanceReportGet { Description = "Không tìm thấy công ty" };
                var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(groupId);
                if (group == null) return new MaintenanceReportGet { Description = "Không tìm thấy đội xe" };


                var modelSpecByCompany = DataContext.GetWhere<CompanyModelSpecification>(m => m.CompanyId == companyId,
                    MotherSqlId).ToList();
                var result = Cache.GetQueryContext<Device>().GetByGroup(companyId, groupId).Select(m =>
                {
                    var companyModelSpec = GetModelSpecByName(m.ModelName, modelSpecByCompany);
                    return new MaintenanceReportTranfer
                    {
                        Serial = m.Serial,
                        ModelName = m.ModelName,
                        KmDaoLopCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmDaoLop ?? 0),
                        KmDaoLopLimit = companyModelSpec?.KmDaoLop ?? 0,
                        KmThayLocDauCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayLocDau ?? 0),
                        KmThayLocDauLimit = companyModelSpec?.KmThayLocDau ?? 0,
                        KmThayLocGioCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayLocGio ?? 0),
                        KmThayLocGioLimit = companyModelSpec?.KmThayLocGio ?? 0,
                        KmThayLocNhotCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayLocNhot ?? 0),
                        KmThayLocNhotLimit = companyModelSpec?.KmThayLocNhot ?? 0,
                        KmThayNhotCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayNhot ?? 0),
                        KmThayNhotLimit = companyModelSpec?.KmThayNhot ?? 0,
                        KmThayVoCurrent =
                            (m.Status?.BasicStatus.TotalGpsDistance ?? 0) -
                            (m.Specification?.KmThayVo ?? 0),
                        KmThayVoLimit = companyModelSpec?.KmThayVo ?? 0,
                        Bs = m.Bs
                    };
                }).ToList();
                return new MaintenanceReportGet { Status = 1, Description = "OK", Datas = result };
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "hàm MaintenanceReportByGroup");
            }
            return new MaintenanceReportGet { Description = "Lỗi dữ liệu" };
        }

        /// <summary>
        ///     lấy danh sách log lịch sử bảo trì
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        public ChangeSpecModelGet GetMaintenanceHistory(long serial, DateTime beginDate,
            DateTime endDate)
        {
            try
            {
                var result =
                    DataContext.GetWhere<ChangeSpecificationLog>(
                        m => m.Serial == serial
                             && m.TimeUpdate >= beginDate
                             && m.TimeUpdate <= endDate,
                        MotherSqlId).Select(m => new MaintenanceHistoryTranfer
                        {
                            Id = m.Id,
                            OptionNameType = m.OptionName.ToString(),
                            DateUpdate = m.TimeUpdate,
                            KmReset = m.KmReset
                        }).ToList();

                return new ChangeSpecModelGet { Status = 1, Description = "OK", Datas = result };
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "");
            }
            return new ChangeSpecModelGet { Description = "Lỗi dữ liệu" };
        }

        [HttpPost]
        public BaseResponse TrackRaw(long serial)
        {
            if (Cache.ContainRawLogSerial(serial)) return new BaseResponse { Status = 1, Description = $"Đã track {serial} từ trước rồi" };
            Cache.TrackRawLogSerial(serial);
            return new BaseResponse { Status = 1, Description = $"Đã track {serial} thành công" };
        }

        [HttpGet]
        public DeviceRawGet GetRaw([FromUri] long serial, [FromUri]DateTime begin, [FromUri]DateTime end)
        {
            try
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(serial);
                if (device == null)
                    return new DeviceRawGet { Description = "Không tồn tại thiết bị này" };
                var company = Cache.GetCompanyById(device.CompanyId);
                if (company == null)
                    return new DeviceRawGet { Description = "Thiết bị không chứa trong 1 công ty nào cả" };
                var result = new DeviceRawGet();
                result.Datas = new List<DeviceRawTranfer>();

                var allData =
                    DataContext.CreateQuery<DeviceRawLog>(company.DbId)
                        .Where(m => m.Indentity == device.Indentity && m.ClientSend >= begin
                                    && m.ClientSend <= end)
                        .Execute();

                foreach (var log in allData.OrderBy(m => m.ClientSend))
                {
                    result.Datas.Add(
                        new DeviceRawTranfer
                        {
                            ClientSend = log.ClientSend,
                            ServerRecv = log.ServerRecv,
                            HexData = BitConverter.ToString(log.Data).Replace("-", string.Empty),
                            Note = log.Note
                            //Data = log.Data
                        }
                    );
                }
                result.Status = 1;
                result.Description = "OK";

                return result;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "GetRaw");
                return new DeviceRawGet { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }


        /// <summary>
        /// Ghi nhận số lần gói tin trùng trong ngày
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SyncDuplicateResponse GetSyncDuplicate()
        {
            try
            {
                IList<Device> alldevices = Cache.GetQueryContext<Device>().GetAll();

                SyncDuplicateResponse result = new SyncDuplicateResponse();
                result.Status = 1;
                result.Description = "OK";

                List<SerialCounter> ret = new List<SerialCounter>();
                foreach (var dev in alldevices)
                {
                    if (dev.Temp == null) continue;
                    if (dev.Temp.SyncDuplicate <= 0) continue;

                    ret.Add(new SerialCounter()
                    {
                        Serial = dev.Serial,
                        Counter = dev.Temp.SyncDuplicate
                    });
                }

                result.Data = ret.OrderByDescending(m => m.Counter).ToList();
                return result;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "GetSyncDuplicate");
                return new SyncDuplicateResponse { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }

        /// <summary>
        /// Ghi nhận số lần gói tin kết thúc sự kiện mà không có gói tin bắt đầu tương ứng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SyncDuplicateResponse GetEventWithoutStart()
        {
            try
            {
                IList<Device> alldevices = Cache.GetQueryContext<Device>().GetAll();

                SyncDuplicateResponse result = new SyncDuplicateResponse();
                result.Status = 1;
                result.Description = "OK";

                List<SerialCounter> ret = new List<SerialCounter>();
                foreach (var dev in alldevices)
                {
                    if (dev.Temp == null) continue;
                    if (dev.Temp.EventWithoutStart <= 0) continue;

                    ret.Add(new SerialCounter()
                    {
                        Serial = dev.Serial,
                        Counter = dev.Temp.SyncDuplicate
                    });
                }

                result.Data = ret.OrderByDescending(m => m.Counter).ToList();
                return result;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "GetEventWithoutStart");
                return new SyncDuplicateResponse { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }

        /// <summary>
        /// Ghi nhận số lần gói tin bắt đầu sự kiện mà không có gói tin kết thúc tương ứng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SyncDuplicateResponse GetEventWithoutEnd()
        {
            try
            {
                IList<Device> alldevices = Cache.GetQueryContext<Device>().GetAll();

                SyncDuplicateResponse result = new SyncDuplicateResponse();
                result.Status = 1;
                result.Description = "OK";

                List<SerialCounter> ret = new List<SerialCounter>();
                foreach (var dev in alldevices)
                {
                    if (dev.Temp == null) continue;
                    if (dev.Temp.EventWithoutEnd <= 0) continue;

                    ret.Add(new SerialCounter()
                    {
                        Serial = dev.Serial,
                        Counter = dev.Temp.SyncDuplicate
                    });
                }

                result.Data = ret.OrderByDescending(m => m.Counter).ToList();
                return result;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "GetEventWithoutEnd");
                return new SyncDuplicateResponse { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }


        /// <summary>
        /// Đánh dấu thiết bị thay thế
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse MarkReplaceSerial(long serial)
        {
            if (Cache.ContainReplaceSerial(serial)) return new BaseResponse { Status = 1, Description = $"Đã đánh dấu {serial} từ trước rồi" };
            Cache.TrackReplaceSerial(serial);
            return new BaseResponse { Status = 1, Description = $"Đã đánh dấu {serial} thành công" };
        }

        /// <summary>
        /// Hủy đánh dấu thiết bị thay thế
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse UnmarkReplaceSerial(long serial)
        {
            if (!Cache.ContainReplaceSerial(serial)) return new BaseResponse { Status = 1, Description = $"Không tồn tại {serial} đánh dấu" };
            Cache.UntrackReplaceSerial(serial);
            return new BaseResponse { Status = 1, Description = $"Hủy đánh dấu {serial} thành công" };
        }

        
        /// <summary>
        /// Đánh dấu thiết bị thay thế (chưa cần xài mà kiểm tra trực tiếp tb bh)
        /// </summary>
        /// <param name="newSerial"></param>
        /// <param name="originalSerial"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse FixDevice(long newSerial, long originalSerial)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(originalSerial);
            if (device == null)
                return new BaseResponse { Description = "Không tìm thấy thiết bị cu~" };

            var dv = new FixDevice()
            {
                Serial = newSerial,
                OriginalSerial = originalSerial,
                Bs = device.Bs,
                CreateTime = device.CreateTime,
                CompanyId = device.CompanyId,
                EmailAddess = device.EmailAddess,
                EndTime = device.EndTime,
                FixTime = DateTime.Now,
                GroupId = device.GroupId,
                Id = device.Id,
                Installer = device.Installer,
                Maintaincer = device.Maintaincer,
                ModelName = device.ModelName,
                Note = device.Note,
                OwnerPhone = device.OwnerPhone,
                PaidFee = device.PaidFee,
                Phone = device.Phone,
                Vin = device.Vin
            };

            try
            {
                DataContext.Insert(dv, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "Fix thiết bị thành công" };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, "Fix thiết bị vào database ko thành công");
                return new BaseResponse { Description = "Fix thiết bị vào database ko thành công" };
            }
        }


        /// <summary>
        /// Danh sách thiết bị đánh dấu thay thế
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ReplaceSerialGet ReplaceSerialList()
        {
            try
            {
                ReplaceSerialGet ret = new ReplaceSerialGet() { Status = 1 };
                ret.Datas = Cache.ReplaceSerialList();
                return ret;
            }
            catch (Exception e)
            {
                Log.Exception("MaintenanceDevice", e, "GetEventWithoutEnd");
                return new ReplaceSerialGet { Description = "Lỗi dữ liệu " + e.StackTrace };
            }
        }


    }
}