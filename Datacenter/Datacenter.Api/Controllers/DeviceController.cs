#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DeviceController.cs
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
using StarSg.Utils.Models.DatacenterResponse.Device;
using StarSg.Utils.Models.Tranfer;
using System.Collections.Generic;
using Datacenter.Model.Utils;
using NHibernate;
using DevicePacketModels.Setups;
using System.IO;
using DevicePacketModels.Utils;
using Datacenter.Model.Setup;
using System.Reflection;
using CorePacket.Utils;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin thiết bị
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceController : BaseController
    {
        /// <summary>
        ///     thêm mới thiết bị
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost]
        public DeviceAdd Add(DeviceTranfer device)
        {
            if (device == null)
                return new DeviceAdd {Description = "Thông tin gửi lên null"};
            if (device.Serial == 0) return new DeviceAdd {Description = "Số serial không hợp lệ"};

            //Kiểm tra tb cũ
            if (Cache.ContainOldSerial(device.Serial))
                return new DeviceAdd { Description = "Đây là thiết bị cũ đã được thay thế" };

            //Đây là thiết bị sử dụng để thay thế, không thể lắp mới
            if (Cache.ContainReplaceSerial(device.Serial))
                return new DeviceAdd { Description = "Đây là thiết bị sử dụng để thay thế, không thể lắp mới" };

            //Kiểm tra trùng serial
            if (Cache.GetQueryContext<Device>().GetByKey(device.Serial) != null)
                return new DeviceAdd { Description = "Đã tồn tại thiết bị này trên hệ thống" };

            //Tính hợp lệ BS xe
            if (string.IsNullOrWhiteSpace(device.Bs))
                return new DeviceAdd { Description = "Không được để trống Biển Số Xe" };
            device.Bs = device.Bs.ToUpper().Trim();

            //Kiểm tra trùng BS xe
            if (Cache.GetQueryContext<Device>().GetWhere(m => m.Bs == device.Bs).FirstOrDefault() != null)
                return new DeviceAdd { Description = "Đã tồn tại Biển Số Xe này trên hệ thống" };

            //Kiểm tra trùng số điện thoại
            if(!String.IsNullOrWhiteSpace(device.PhoneOfDevice))
            {
                if(device.PhoneOfDevice.Length>=9)
                {
                    if (Cache.GetQueryContext<Device>().GetWhere(m => m.Phone != null
                        && m.Phone.Length >= 9
                        && m.Phone.Substring(m.Phone.Length - 9) == device.PhoneOfDevice.Substring(device.PhoneOfDevice.Length - 9))
                        .FirstOrDefault() != null)
                        return new DeviceAdd { Description = "Đã tồn tại số điện thoại này trên hệ thống" };
                }
                else
                {
                    if (Cache.GetQueryContext<Device>().GetWhere(m => m.Phone == device.PhoneOfDevice).FirstOrDefault() != null)
                        return new DeviceAdd { Description = "Đã tồn tại số điện thoại này trên hệ thống" };
                }
            }

            if (Cache.GetQueryContext<Device>().GetWhere(m => m.Phone == device.PhoneOfDevice).FirstOrDefault() != null)
                return new DeviceAdd { Description = "Đã tồn tại số điện thoại này trên hệ thống" };

            //Các xử lý khác
            if (device.CompanyId == 0) return new DeviceAdd {Description = "Id công ty không hợp lệ"};
            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null) return new DeviceAdd {Description = "Thông tin công ty không chính xác"};

            DeviceGroup group = null;
            if (device.GroupId > 0)
            {
                group = Cache.GetQueryContext<DeviceGroup>().GetByKey(device.GroupId);
                if (group == null)
                    return new DeviceAdd {Description = "Không tìm thấy thông tin đội xe"};
            }

            DeviceModel model = null;
            if (!string.IsNullOrEmpty(device.ModelName))
            {
                model = Cache.GetQueryContext<DeviceModel>().GetByKey(device.ModelName);
                if (model == null)
                    return new DeviceAdd {Description = "Không tìm thấy model thiết bị"};
            }

            if (!device.Type.Check())
                return new DeviceAdd {Description = "Loại hình kinh doanh không chính xác"};

            if (string.IsNullOrEmpty(device.Id))
                return new DeviceAdd {Description = "Không được để trống biển Id thiết bị"};

            var dv = new Device();
            dv.Serial = device.Serial;
            dv.Indentity = Guid.NewGuid();
            dv.ActivityType = (DeviceActivityType) device.Type;
            dv.BgtTranportData = device.BgtTranport;
            dv.Bs = device.Bs;
            dv.Id = device.Id;
            dv.CompanyId = company.Id;

            dv.CreateTime = DateTime.Now;
            dv.PaidFee = dv.CreateTime;
            dv.EndTime = DateTime.Now.AddMonths(12);

            dv.GroupId = @group?.Id ?? 0;
            dv.ModelName = model?.Name ?? "";
            dv.NotView = false;
            dv.Sgtvt = device.Sgtvt;
            dv.Phone = device.PhoneOfDevice;
            dv.Vin = device.VinSerial;

            dv.InvertDoor = device.InvertDoor;
            dv.InvertAir= device.InvertAir;
            dv.Note = device.Note;

            dv.Installer = device.Installer;
            dv.Maintaincer = device.Maintaincer;

            dv.FuelSheet = device.FuelSheet;
            dv.FuelParams = device.FuelParams;

            dv.FuelQuotaKm = device.FuelQuotaKm;
            dv.FuelQuoteHour = device.FuelQuoteHour;

            dv.OwnerPhone = device.OwnerPhone;
            dv.SmsAlarm = device.SmsAlarm;
            dv.OnlineTimeout = device.OnlineTimeout;

            dv.EmailAlarm = device.EmailAlarm;
            dv.EmailAddess = device.EmailAddess;

            dv.CameraId = device.CameraId;

            try
            {
                Log.Debug("DEVICE", $"bắt đầu thêm mới thiết bị {dv.Serial}");
                DataContext.Insert(dv, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<Device>().Add(dv, company.Id))
                    return new DeviceAdd {Description = "Thêm thiết bị vào cache ko thành công"};

                //nạp setup
                if(device.SetSetup)
                {
                    //Nạp ngày lắp
                    var data =
                        new P212SetupStartDate
                        {
                            StartDate = device.CreateTime
                        }.Serializer();
                    BuildSetup(data, "auto", device.Serial, "Cài Đặt ngày lắp", typeof(P212SetupStartDate));

                    //nạp biển số
                    data =
                        new P211SetupBS
                        {
                            Bs = device.Bs
                        }.Serializer();
                    BuildSetup(data, "auto", device.Serial, "Cài đặt biển số xe", typeof(P211SetupBS));
                }

                return new DeviceAdd {Status = 1, Serial = dv.Serial, Description = "Thêm thiết bị thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, "Thêm thiết bị vào database ko thành công");
                return new DeviceAdd {Description = "Thêm thiết bị vào database ko thành công"};
            }
        }

        /// <summary>
        ///     Cập nhật thông tin thiết bị
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long serial, DeviceTranfer tran)
        {
            if (tran == null)
                return new BaseResponse {Description = "Thông tin gửi lên null"};
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new BaseResponse {Description = "Không tìm thấy thiết bị"};
            if (tran.CompanyId == 0) return new BaseResponse {Description = "Id công ty không hợp lệ"};
            var company = Cache.GetCompanyById(tran.CompanyId);
            if (company == null) return new BaseResponse {Description = "Thông tin công ty không chính xác"};

            DeviceGroup group = null;
            if (tran.GroupId > 0)
            {
                group = Cache.GetQueryContext<DeviceGroup>().GetByKey(tran.GroupId);
                if (group == null)
                    return new BaseResponse {Description = "Không tìm thấy thông tin đội xe"};
            }

            DeviceModel model = null;
            if (!string.IsNullOrEmpty(tran.ModelName))
            {
                model = Cache.GetQueryContext<DeviceModel>().GetByKey(tran.ModelName);
                if (model == null)
                    return new BaseResponse {Description = "Không tìm thấy model thiết bị"};
            }

            if (string.IsNullOrWhiteSpace(tran.Bs))
                return new BaseResponse {Description = "Không được để trống biển số xe"};
            tran.Bs = tran.Bs.ToUpper().Trim();

            //Kiểm tra trùng BS xe
            var deviceBs = Cache.GetQueryContext<Device>().GetWhere(m => m.Bs == tran.Bs).FirstOrDefault();
            if(deviceBs!=null && deviceBs.Serial!=serial)
                return new BaseResponse { Description = "Đã tồn tại Biển Số Xe này trên hệ thống" };

            //Kiểm tra trùng số điện thoại  
            Device devicePhone=null;
            if (!String.IsNullOrWhiteSpace(tran.PhoneOfDevice))
            {
                if (tran.PhoneOfDevice.Length >= 9)
                    devicePhone = Cache.GetQueryContext<Device>().GetWhere(m => m.Phone != null
                        && m.Phone.Length >= 9
                        && m.Phone.Substring(m.Phone.Length - 9) == tran.PhoneOfDevice.Substring(tran.PhoneOfDevice.Length - 9))
                        .FirstOrDefault();
                else
                    devicePhone = Cache.GetQueryContext<Device>().GetWhere(m => m.Phone == tran.PhoneOfDevice).FirstOrDefault();
            }
            if (devicePhone != null && devicePhone.Phone != tran.PhoneOfDevice)
                return new DeviceAdd { Description = "Đã tồn tại số điện thoại này trên hệ thống" };

            if (!tran.Type.Check())
                return new BaseResponse {Description = "Loại hình kinh doanh không chính xác"};

            if (string.IsNullOrEmpty(tran.Id))
                return new BaseResponse {Description = "Không được để trống biển số xe"};

            //device.Serial = tran.Serial;
            device.ActivityType = (DeviceActivityType) tran.Type;
            device.BgtTranportData = tran.BgtTranport;
            device.Bs = tran.Bs;
            device.Id = tran.Id;
            device.CompanyId = company.Id;
            device.GroupId = @group?.Id ?? 0;
            device.ModelName = model?.Name ?? "";
            device.NotView = false;
            device.Sgtvt = tran.Sgtvt;
            device.Phone = tran.PhoneOfDevice;
            device.Vin = tran.VinSerial;

            //if (tran.CreateTime > DateTime.MinValue)
            //    device.CreateTime = tran.CreateTime;

            //if (tran.EndTime > DateTime.MinValue)
            //    device.EndTime = tran.EndTime;

            //if (tran.PaidFee > DateTime.MinValue)
            //    device.PaidFee = tran.PaidFee;

            device.Note = tran.Note;
            device.InvertDoor = tran.InvertDoor;
            device.InvertAir = tran.InvertAir;
            device.Note = tran.Note;

            device.Installer = tran.Installer;
            device.Maintaincer = tran.Maintaincer;

            device.FuelSheet = tran.FuelSheet;
            device.FuelParams = tran.FuelParams;

            device.FuelQuotaKm = tran.FuelQuotaKm;
            device.FuelQuoteHour = tran.FuelQuoteHour;

            device.OwnerPhone = tran.OwnerPhone;
            device.SmsAlarm= tran.SmsAlarm;
            device.OnlineTimeout= tran.OnlineTimeout;

            device.EmailAlarm = tran.EmailAlarm;
            device.EmailAddess = tran.EmailAddess;

            device.CameraId = tran.CameraId;

            try
            {
               
                DataContext.Update(device, MotherSqlId, m => m.ActivityType, m => m.BgtTranportData, m => m.Bs,
                    m => m.Id, m => m.CompanyId, m => m.GroupId, m => m.ModelName, m => m.Sgtvt, mbox => mbox.Vin,
                    mbox => mbox.Phone

                    //, m => m.CreateTime
                    //, m => m.EndTime
                    //, m => m.PaidFee

                    , m => m.Note
                    , m => m.InvertDoor
                    , m => m.InvertAir

                    , m => m.Installer
                    , m => m.Maintaincer

                    , m => m.FuelSheet
                    , m => m.FuelParams

                    , m => m.FuelQuotaKm
                    , m => m.FuelQuoteHour

                    , m => m.OwnerPhone
                    , m => m.SmsAlarm
                    , m => m.OnlineTimeout

                    , m => m.EmailAlarm
                    , m => m.EmailAddess

                    , m => m.CameraId
                    );

                DataContext.Commit(MotherSqlId);
                return new BaseResponse {Description = "Cập nhật thiết bị thành công", Status = 1};
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, "Cập nhật thiết bị vào database ko thành công");
                return new BaseResponse {Description = "Cập nhật thiết bị vào database ko thành công"};
            }
        }


        /// <summary>
        ///     gia hạn phí
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="days">số ngày gia hạn</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetRenew(long serial, int days)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new BaseResponse { Description = "Không tìm thấy thiết bị" };

            device.EndTime = device.EndTime.AddDays(days);
            device.PaidFee = DateTime.Now;

            try
            {
                DataContext.Update(device, MotherSqlId, m => m.ActivityType
                    , m => m.EndTime
                    , m => m.PaidFee
                    );
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Description = "Gia hạn thiết bị thành công", Status = 1 };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, "Gia hạn thiết bị vào database ko thành công");
                return new BaseResponse { Description = "Gia hạn thiết bị vào database ko thành công" };
            }
        }

        /// <summary>
        ///     xóa thiết bị
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long serial)
        {
            //return new BaseResponse { Description = "Không được phép" };
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null)
                return new BaseResponse {Description = "Không tìm thấy thiết bị"};
            try
            {
                DataContext.Delete(device, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                if (!Cache.GetQueryContext<Device>().Del(device, device.CompanyId))
                    return new BaseResponse {Description = "Xóa dữ liệu thiết bị trên cache không thành công"};
                return new BaseResponse {Status = 1, Description = "Xóa thiết bị thành công"};
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"Xóa thiết bị trong database ko thành công {serial}");
                return new BaseResponse {Description = "Xóa thiết bị trong database ko thành công"};
            }
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo serial
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetSingle GetDeviceBySerial(long serial)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            return GetSingleResult(device);
        }

        /// <summary>
        ///     lấy thông tin ở rộng của thiết bị theo serial
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceBySerialEx(long serial)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            return GetMultiResultEx(new List<Device>() {device});
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo serials list
        /// </summary>
        /// <param name="serials"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetDeviceBySerials(String serials)
        {
            IList<Device> devices = new List<Device>();
            foreach (var item in serials.Split(','))
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(long.Parse(item));
                if (device == null) continue;
                devices.Add(device);
            }
            return GetMultiResult(devices);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo serials list
        /// </summary>
        /// <param name="serials"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceBySerialsEx(String serials)
        {
            IList<Device> devices = new List<Device>();
            foreach (var item in serials.Split(','))
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(long.Parse(item));
                if (device == null) continue;
                devices.Add(device);
            }
            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo bản số xe
        /// </summary>
        /// <param name="bs">bản số xe</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetSingle GetDeviceByNumber(String bs)
        {
            var device = Cache.GetQueryContext<Device>().GetWhere(m => m.Bs == bs.ToUpper()).FirstOrDefault();
            return GetSingleResult(device);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo bản số xe
        /// </summary>
        /// <param name="bs">bản số xe</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByNumberEx(String bs)
        {
            if (String.IsNullOrWhiteSpace(bs) || bs.Length < 3) return new DeviceGetMultiEx { Description = "Thông tin nhập quá ngắn" };
            //var devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Bs == bs.ToUpper());
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(bs.ToUpper());
            var devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Bs!=null && regEx.IsMatch(m.Bs.ToUpper()));
            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo số điện thoại
        /// </summary>
        /// <param name="phone">số điện thoại</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetSingle GetDeviceByPhone(String phone)
        {
            var device = Cache.GetQueryContext<Device>().GetWhere(m => m.Phone == phone.ToUpper()).FirstOrDefault();
            return GetSingleResult(device);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo số điện thoại
        /// </summary>
        /// <param name="phone">số điện thoại</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByPhoneEx(String phone)
        {
            if(String.IsNullOrWhiteSpace(phone)) return new DeviceGetMultiEx { Description = "Thông tin nhập rỗng" };

            IList<Device> devices;
            if (phone.Length>=9)
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Phone != null && m.Phone.Length>=9 && m.Phone.Substring(m.Phone.Length - 9) == phone.Substring(phone.Length - 9));
            else
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Phone == phone);

            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo số Vin
        /// </summary>
        /// <param name="vin">số Vin</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByVinEx(String vin)
        {
            var devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Vin == vin);
            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo ngày tạo
        /// </summary>
        /// <param name="from">từ ngày</param>
        /// <param name="to">đến ngày</param>
        /// <param name="companyId">theo công ty, 0 thì không lọc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetDeviceByCreateDate(DateTime from,DateTime to, long companyId = 0)
        {
            IList<Device> devices = null;

            if (companyId > 0)
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.CompanyId == companyId && m.CreateTime >= from && m.CreateTime <= to);
            else
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.CreateTime >= from && m.CreateTime <= to);

            return GetMultiResult(devices);
        }

        /// <summary>
        ///     lấy thông tin mở rộng thiết bị theo ngày tạo
        /// </summary>
        /// <param name="from">từ ngày</param>
        /// <param name="to">đến ngày</param>
        /// <param name="companyId">theo công ty, 0 thì không lọc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByCreateDateEx(DateTime from, DateTime to, long companyId=0)
        {
            IList<Device> devices = null;

            if(companyId>0)
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.CompanyId==companyId && m.CreateTime >= from && m.CreateTime <= to);
            else
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.CreateTime >= from && m.CreateTime <= to);

            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin thiết bị có chứa ghi chú
        /// </summary>
        /// <param name="note">Ghi chú có chứa giá trị này</param>
        /// <param name="companyId">theo công ty, 0 thì không lọc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetDeviceByNote(String note, long companyId = 0)
        {
            IList<Device> devices = null;

            if (companyId > 0)
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.CompanyId==companyId &&  m.Note != null && m.Note.ToLower().Contains(note.ToLower()));
            else
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Note != null && m.Note.ToLower().Contains(note.ToLower()));

            return GetMultiResult(devices);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị có chứa ghi chú
        /// </summary>
        /// <param name="note">Ghi chú có chứa giá trị này</param>
        /// <param name="companyId">theo công ty, 0 thì không lọc</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByNoteEx(String note, long companyId = 0)
        {
            IList<Device> devices = null;

            if (companyId > 0)
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.CompanyId == companyId && m.Note != null && m.Note.ToLower().Contains(note.ToLower()));
            else
                devices = Cache.GetQueryContext<Device>().GetWhere(m => m.Note != null && m.Note.ToLower().Contains(note.ToLower()));

            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin xe theo group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetByGroupId(long groupId)
        {
            var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(groupId);
            if (group == null)
                return new DeviceGetMulti {Description = "Không tìm thấy thông tin đội xe"};
            var devices = Cache.GetQueryContext<Device>().GetByGroup(group.CompnayId, group.Id);
            return GetMultiResult(devices);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetByGroupIdEx(long groupId)
        {
            var group = Cache.GetQueryContext<DeviceGroup>().GetByKey(groupId);
            if (group == null)
                return new DeviceGetMultiEx { Description = "Không tìm thấy thông tin đội xe" };
            var devices = Cache.GetQueryContext<Device>().GetByGroup(group.CompnayId, group.Id);
            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetByCompanyId(long companyId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new DeviceGetMulti {Description = "Thông tin công ty không chính xác"};
            var devices = Cache.GetQueryContext<Device>().GetByCompany(company.Id);
            return GetMultiResult(devices);
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetByCompanyIdEx(long companyId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new DeviceGetMultiEx { Description = "Thông tin công ty không chính xác" };
            var devices = Cache.GetQueryContext<Device>().GetByCompany(company.Id);
            return GetMultiResultEx(devices);
        }

        /// <summary>
        ///     Thay đổi thiết bị
        /// </summary>
        /// <param name="oldSerial">Serial Thiết bị Cũ</param>
        /// <param name="newSerial">Serial Thiết bị Mới</param>
        /// <param name="autoSetup">Tu dong nap lai vo thiet bi tu thong so thiet bi cu </param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse ChangeDevice(long oldSerial, long newSerial,bool autoSetup = false)
        {
            //thử tìm thiết bị mới
            var newdevice = Cache.GetQueryContext<Device>().GetByKey(newSerial);
            if (newdevice != null)
                return new BaseResponse { Description = $"Thiết bị mới {newSerial} đã tồn tại trong hệ thống" };

            //lấy thiết bị cũ
            var device = Cache.GetQueryContext<Device>().GetByKey(oldSerial);
            if (device == null)
                return new BaseResponse { Description = "Không tìm thấy thiết bị cũ" };

            var company = Cache.GetCompanyById(device.CompanyId);
            if (company == null)
                return new BaseResponse { Description = "Không tìm thấy Công Ty của thiết bị cũ" };

            //Đây là thiết bị bão hành, kiềm tra xe mới lắp thì không cho đổi
            if(newSerial.ToString().StartsWith("5"))
            {
                DeviceSimInfo simDev = DataContext.Get<DeviceSimInfo>(oldSerial, MotherSqlId);
                if (simDev == null)
                {
                    return new BaseResponse { Description = $"Không được đổi thiết bị mới {oldSerial} sang thiết bị bão hành {newSerial}"};
                }
            }
            

            ////kiem tra thiet bi sua 
            //FixDevice fixDev = DataContext.Get<FixDevice>(newSerial, MotherSqlId);
            //if (fixDev != null)
            //{
            //    device.EndTime = fixDev.EndTime;//doi lai gia han
            //    device.CreateTime = fixDev.CreateTime;
            //    device.PaidFee = fixDev.PaidFee;
            //}

            try
            {
                ////Thay thế trong database
                //if (fixDev != null)
                //{
                //    DataContext.Update(device, MotherSqlId, m => m.EndTime, m => m.CreateTime, m => m.PaidFee);
                //    DataContext.Commit(MotherSqlId);
                //}

                int ret = 0;
                DataContext.CustomHandle<ISession>(m => {
                    try
                    {
                        NHibernate.IQuery query = m.CreateSQLQuery("exec spChangeDevice @oldSerial=:oldSerial, @newSerial=:newSerial");
                        query.SetInt64("oldSerial", oldSerial);
                        query.SetInt64("newSerial", newSerial);
                        ret = query.UniqueResult<int>();
                    }
                    catch (Exception ex)
                    {
                        Log.Exception("DeviceController", ex, "Không thể thay đổi thiết bị trong database");
                    }
                  
                }, company.DbId);
                //DataContext.Commit(MotherSqlId);

                if (ret<=0)
                    return new BaseResponse { Description = $"Không thể thay đổi thiết bị, lỗi {ret}"};

                //bõ cái cũ ra khỏi memory
                Cache.GetQueryContext<Device>().Del(device, device.CompanyId);

                device.Temp = new DeviceTemp();
                //device.Temp.Serial = newSerial;

                //đổi lại serial
                device.Serial = newSerial;
                //device.SetupInfo.Serial = newSerial;
                if(device.Specification!=null) device.Specification.Serial = newSerial;
                if(device.SimInfo!=null) device.SimInfo.Serial = newSerial;

                //device.SimInfo = null;
                if (device.Status != null)
                {
                    device.Status.Serial = newSerial;
                    //reset LastTotalKmUsingOnDay
                    if (device.Status.BasicStatus != null)
                    {
                        device.Temp.LastTotalKmUsingOnDay 
                            = device.Status.LastTotalKmUsingOnDay 
                            = device.Status.BasicStatus.TotalGpsDistance;
                    }
                }
                //device.Status = null;

                //thêm vô lại 
                Cache.GetQueryContext<Device>().Add(device, device.CompanyId);

                //track lại tb cũ
                Cache.TrackOldSerial(oldSerial);

                //hủy đánh dấu thiết bị thay thế
                Cache.UntrackReplaceSerial(newSerial);

                //tu dong nap 
                if (autoSetup && device.SetupInfo!=null)
                {
                    ////nap thông tin cơ bản cho thiết bị
                    //var data = new P202SetupDevice
                    //{
                    //    OverSpeed = device.SetupInfo.OverSpeed,
                    //    OverTimeInDay = device.SetupInfo.OverTimeInDay,
                    //    OverTimeInSession = device.SetupInfo.OverTimeInSession,
                    //    PhoneSystemControl = device.SetupInfo.PhoneSystemControl,
                    //    TimeSync = (short)device.SetupInfo.TimeSync,
                    //    Bs = device.Bs
                    //}.Serializer();
                    //BuildSetup(data, "auto", device.Serial, "Cài đặt cơ bản tự động khi đổi thiết bị", typeof(P202SetupDevice));

                    //nạp ngày lắp
                    var data =
                        new P212SetupStartDate
                        {
                            StartDate = device.CreateTime
                        }.Serializer();
                    BuildSetup(data, "auto", device.Serial, "Cài Đặt ngày lắp tự động khi đổi thiết bị", typeof(P212SetupStartDate));

                    //nạp biển số
                    data =
                        new P211SetupBS
                        {
                            Bs = device.Bs
                        }.Serializer();
                    BuildSetup(data, "auto", device.Serial, $"Cài đặt biển số xe {device.Bs} tự động khi đổi thiết bị", typeof(P211SetupBS));


                    //quá vận tốc
                    data =
                        new P203SetOverSpeed
                        {
                            OverSpeed = device.SetupInfo.OverSpeed
                        }.Serializer();
                    BuildSetup(data, "auto", device.Serial, $"Cài đặt quá vận tốc {device.SetupInfo.OverSpeed} tự động khi đổi thiết bị", typeof(P203SetOverSpeed));


                    //Yêu cầu đọc cài đặt tự động
                    BuildSetup(new P204ReadDeviceInfo().Serializer(), "auto", device.Serial, "Yêu cầu đọc cài đặt tự động khi đổi thiết bị", typeof(P204ReadDeviceInfo));

                }
                device.SetupInfo = null;//de luu lai tren db khi nhan tu goi Xử lý thông tin cài đặt của thiết bị gưởi lên

                return new BaseResponse { Description = "Thay đổi thiết bị thành công", Status = 1 };
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, "Thay đổi thiết bị không thành công");
                return new BaseResponse { Description = "Thay đổi thiết bị không thành công" };
            }

        }

        /// <summary>
        /// Lấy kết quả trả về của danh sách thiết bị
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        private DeviceGetMulti GetMultiResult(IList<Device> devices)
        {
            if (devices == null)
                return new DeviceGetMulti { Description = "Không tìm thấy thiết bị" };

            return new DeviceGetMulti
            {
                Status = 1,
                Description = "OK",
                Devices = devices.Select(device => new DeviceTranfer
                {
                    Bs = device.Bs,
                    BgtTranport = device.BgtTranportData,
                    CompanyId = device.CompanyId,
                    EndTime = device.EndTime,
                    PaidFee = device.PaidFee,
                    GroupId = device.GroupId,
                    CreateTime = device.CreateTime,
                    Id = device.Id,
                    ModelName = device.ModelName,
                    PhoneOfDevice = device.Phone,
                    Serial = device.Serial,
                    Sgtvt = device.Sgtvt,
                    VinSerial = device.Vin,
                    Type = (int)device.ActivityType,
                    InvertDoor = device.InvertDoor,
                    InvertAir = device.InvertAir,
                    Note = device.Note,

                    BgtTime = device?.Temp?.BgtvLastTransfer ?? DateTimeFix.Min,
                    BgtCount = device?.Temp?.BgtvCountTransfer ?? 0,

                    Installer = device.Installer,
                    Maintaincer = device.Maintaincer,

                    FuelSheet = device.FuelSheet,
                    FuelParams = device.FuelParams,

                    FuelQuotaKm = device.FuelQuotaKm,
                    FuelQuoteHour = device.FuelQuoteHour,

                    OwnerPhone = device.OwnerPhone,
                    SmsAlarm = device.SmsAlarm,
                    OnlineTimeout = device.OnlineTimeout,

                    EmailAlarm = device.EmailAlarm,
                    EmailAddess = device.EmailAddess,

                    CameraId = device.CameraId,

                    DriverId = device?.Status?.DriverStatus?.DriverId ?? 0

                }).ToList()
            };
        }

        /// <summary>
        /// Lấy kết quả trả về của thiết bị
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private DeviceGetSingle GetSingleResult(Device device)
        {
            if (device == null)
                return new DeviceGetSingle { Description = "Không tìm thấy thiết bị" };
            return new DeviceGetSingle
            {
                Status = 1,
                Description = "OK",
                Device = new DeviceTranfer
                {
                    Bs = device.Bs,
                    BgtTranport = device.BgtTranportData,
                    CompanyId = device.CompanyId,
                    EndTime = device.EndTime,
                    PaidFee = device.PaidFee,
                    GroupId = device.GroupId,
                    Id = device.Id,
                    CreateTime = device.CreateTime,
                    ModelName = device.ModelName,
                    PhoneOfDevice = device.Phone,
                    Serial = device.Serial,
                    Sgtvt = device.Sgtvt,
                    VinSerial = device.Vin,
                    Type = (int)device.ActivityType,
                    InvertDoor = device.InvertDoor,
                    InvertAir = device.InvertAir,
                    Note = device.Note,

                    BgtTime = device?.Temp?.BgtvLastTransfer ?? DateTimeFix.Min,
                    BgtCount = device?.Temp?.BgtvCountTransfer ?? 0,

                    Installer = device.Installer,
                    Maintaincer = device.Maintaincer,

                    FuelSheet = device.FuelSheet,
                    FuelParams = device.FuelParams,

                    FuelQuotaKm = device.FuelQuotaKm,
                    FuelQuoteHour = device.FuelQuoteHour,

                    OwnerPhone = device.OwnerPhone,
                    SmsAlarm = device.SmsAlarm,
                    OnlineTimeout = device.OnlineTimeout,

                    EmailAlarm = device.EmailAlarm,
                    EmailAddess = device.EmailAddess,

                    CameraId = device.CameraId,

                    DriverId = device?.Status?.DriverStatus?.DriverId ?? 0
                }
            };
        }

        /// <summary>
        /// Lấy kết quả trả về mở rộng của danh sách thiết bị
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        private DeviceGetMultiEx GetMultiResultEx(IList<Device> devices)
        {
            if (devices == null)
                return new DeviceGetMultiEx { Description = "Không tìm thấy thiết bị" };

            return new DeviceGetMultiEx
            {
                Status = 1,
                Description = "OK",
                Devices = devices.Select(device => new DeviceTranferEx
                {
                    Bs = device.Bs,
                    BgtTranport = device.BgtTranportData,
                    CompanyId = device.CompanyId,
                    EndTime = device.EndTime,
                    GroupId = device.GroupId,
                    CreateTime = device.CreateTime,
                    PaidFee = device.PaidFee,
                    Id = device.Id,
                    ModelName = device.ModelName,
                    PhoneOfDevice = device.Phone,
                    Serial = device.Serial,
                    Sgtvt = device.Sgtvt,
                    VinSerial = device.Vin,
                    Type = (int)device.ActivityType,
                    InvertDoor = device.InvertDoor,
                    InvertAir = device.InvertAir,
                    Note = device.Note,

                    BgtTime = device.Temp.BgtvLastTransfer,
                    BgtCount = device.Temp.BgtvCountTransfer,

                    FirmWareVersion = device.SetupInfo?.FirmWareVersion??"",
                    HardWareVersion = device.SetupInfo?.HardWareVersion??"",

                    CompanyName = Cache.GetCompanyById(device.CompanyId)?.Name ?? "",
                    GroupName = Cache.GetQueryContext<DeviceGroup>().GetByKey(device.GroupId)?.Name ?? "",

                    Installer = device.Installer,
                    Maintaincer = device.Maintaincer,

                    FuelSheet = device.FuelSheet,
                    FuelParams = device.FuelParams,

                    FuelQuotaKm = device.FuelQuotaKm,
                    FuelQuoteHour= device.FuelQuoteHour,

                    OwnerPhone = device.OwnerPhone,
                    SmsAlarm = device.SmsAlarm,
                    OnlineTimeout = device.OnlineTimeout,

                    EmailAlarm = device.EmailAlarm,
                    EmailAddess = device.EmailAddess,

                    CameraId = device.CameraId

                }).ToList()
        };
        }


        /// <summary>
        ///     thêm thông tin vào database
        /// </summary>
        /// <param name="data"></param>
        /// <param name="username"></param>
        /// <param name="serial"></param>
        /// <param name="note"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        private BaseResponse BuildSetup(byte[] data, string username, long serial, string note, Type pType)
        {
            var opcode = pType.GetCustomAttribute<DeviceOpCodeAttribute>().Opcode;

            byte[] tmp;
            using (var memory = new MemoryStream())
            {
                using (var w = new BinaryWriter(memory))
                {
                    w.Write((short)opcode);
                    w.Write((short)data.Length);
                    w.Write(data);
                    //w.Write(new Crc32().ComputeChecksum(data));
                    w.Write(Crc32.ComputeChecksum(data));
                    tmp = memory.ToArray();
                }
            }

            var setup = new DeviceSetupRequest
            {
                Complete = false,
                Data = tmp,
                Id = 0,
                Request = DateTime.Now,
                Serial = serial,
                Response = new DateTime(2000, 1, 1),
                UserName = username,
                Note = note
            };
            try
            {
                DataContext.Insert(setup, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse { Status = 1, Description = "Thiết lập thành công" };
            }
            catch (Exception e)
            {
                Log.Exception("DeviceSetupController", e, "thêm gói tin lập trình vào database ko thành công");
                return new BaseResponse { Description = "Lỗi hệ thống ." };
            }
        }


        

    }
}