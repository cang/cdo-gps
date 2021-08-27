#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DeviceSetupController.cs
// Time Create : 3:53 PM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using CorePacket.Utils;
using Datacenter.Api.Core;
using Datacenter.Model.Entity;
using Datacenter.Model.Setup;
using Datacenter.Model.Utils;
using DevicePacketModels.Setups;
using DevicePacketModels.Utils;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Setup;
using StarSg.Utils.Models.Tranfer.Setup;

#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     quản lý các thông tin lập trình cho thiết bị
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceSetupController : BaseController
    {
        /// <summary>
        ///     thêm thông tin vào database
        /// </summary>
        /// <param name="data"></param>
        /// <param name="username"></param>
        /// <param name="serial"></param>
        /// <param name="note"></param>
        /// <param name="pType"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        private BaseResponse BuildSetup(byte[] data, string username, long serial, string note, Type pType, bool commit = true)
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
                if(commit) DataContext.Commit(MotherSqlId);
                return new BaseResponse {Status = 1, Description = "Thiết lập thành công"};
            }
            catch (Exception e)
            {
                Log.Exception("DeviceSetupController", e, "thêm gói tin lập trình vào database ko thành công");
                return new BaseResponse {Description = "Lỗi hệ thống ."};
            }
        }

        /// <summary>
        ///     thêm thông tin vào database
        /// </summary>
        /// <param name="data"></param>
        /// <param name="username"></param>
        /// <param name="serial"></param>
        /// <param name="note"></param>
        /// <param name="pType"></param>
        /// <param name="sendreply">gửi thêm lệnh yêu cầu gừi thông tin</param>
        /// <param name="commit"></param>
        /// <returns></returns>
        private BaseResponse BuildSetup(byte[] data, string username, long serial, string note, Type pType,bool sendreply, bool commit=true)
        {
            try
            {
                BaseResponse ret = BuildSetup(data, username, serial, note, pType);
                if (sendreply && ret.Status > 0)
                    return BuildSetup(new P204ReadDeviceInfo().Serializer(), username, serial, "Yêu cầu đọc cài đặt tự động", typeof(P204ReadDeviceInfo), commit);
                return ret;
            }
            catch (Exception e)
            {
                Log.Exception("DeviceSetupController", e, "BuildSetup ko thành công");
                return new BaseResponse { Description = "Lỗi hệ thống ." };
            }
        }

        /// <summary>
        ///     yêu cầu thiết bị cập nhật phần mrrnf
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse UpdateFirmware(UpdateFirmware rq)
        {
            if (rq == null) return new BaseResponse {Description = "Thông tin cài đặt null"};
            if (string.IsNullOrEmpty(rq.Name))
                return new BaseResponse {Description = "Tên của verion không được phép null hoặc để trống"};
            var device = Cache.GetQueryContext<Device>().GetByKey(rq.Serial);

            if (device == null) return new BaseResponse {Description = "Không tìm thấy thiết bị"};

            var data = new P210DeviceUpdate {Name = rq.Name}.Serializer();
            return BuildSetup(data, rq.Username, device.Serial, $"Cập nhật firmware : {rq.Name}",
                typeof (P210DeviceUpdate));
        }

        /// <summary>
        ///     thiết lập thông tin tài xế xuống thiết bị
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupDriver(SetupDriver rq)
        {
            if (rq == null) return new BaseResponse {Description = "Thông tin cài đặt null"};

            var driver = Cache.GetQueryContext<Driver>().GetByKey(rq.DriverId);


            if (driver == null) return new BaseResponse {Description = "Không tìm thấy thông tin tài xế"};


            if (string.IsNullOrEmpty(driver.Gplx))
                return new BaseResponse {Description = "Lái xe chưa nhập bằng lái"};
            if (string.IsNullOrEmpty(driver.Name))
                return new BaseResponse {Description = "Lái xe chưa nhập tên"};

            var device = Cache.GetQueryContext<Device>().GetByKey(rq.Serial);

            if (device == null) return new BaseResponse {Description = "Không tìm thấy thiết bị"};

            var data =
                new P201SetupDriver
                {
                    DriverId = (int) driver.Id,
                    DriverSerial = driver.Gplx,
                    Name = driver.Name.RemoveVietnames()
                }.Serializer();

            return BuildSetup(data, rq.Username, device.Serial, "Thiết lập tài xế", typeof (P201SetupDriver));
        }

        /// <summary>
        ///     cài đặt giới hạn vận tốc
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupOverSpeed(SetupOverSpeed rq)
        {
            if (rq == null) return new BaseResponse {Description = "Thông tin cài đặt null"};
            if (rq.MaxSpeed == 0)
                return new BaseResponse {Description = "Không thể để giới hạn là 0"};

            var device = Cache.GetQueryContext<Device>().GetByKey(rq.Serial);
            if (device == null) return new BaseResponse {Description = "Không tìm thấy thiết bị"};

            #region  lưu cấu hình thiết bị vào db
            if (device.SetupInfo != null)
            {
                device.SetupInfo.OverSpeed= rq.MaxSpeed;
                device.SetupInfo.OverSpeedDefault = rq.MaxSpeed;
                device.SetupInfo.TimeUpdate = DateTime.Now;
                DataContext.Update(device.SetupInfo, MotherSqlId);
                DataContext.Commit(MotherSqlId);
            }
            #endregion  lưu cấu hình thiết bị vào db

            var data = new P203SetOverSpeed {OverSpeed = rq.MaxSpeed}.Serializer();
            return BuildSetup(data, rq.Username, device.Serial, $"Thiết lập quá vận tốc {rq.MaxSpeed} km/h",
                typeof (P203SetOverSpeed),true,true);
        }

        /// <summary>
        ///     Cài đặt thông tin biển số xe 
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupBS(SetupBS rq)
        {
            if (rq == null) return new BaseResponse { Description = "Thông tin cài đặt null" };
            if (string.IsNullOrEmpty(rq.Bs))
                return new BaseResponse { Description = "Biển số xe chưa có" };

            var device = Cache.GetQueryContext<Device>().GetByKey(rq.Serial);
            if (device == null) return new BaseResponse { Description = "Không tìm thấy thiết bị" };
            var data =
                new P211SetupBS
                {
                    Bs  = rq.Bs
                }.Serializer();
            return BuildSetup(data, rq.Username, device.Serial, "Cài đặt biển số xe", typeof(P211SetupBS));
        }

        /// <summary>
        ///     Cài Đặt mốc ngày lắp đặt xuống thiết bị 
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupStartDate(SetupStartDate rq)
        {
            if (rq == null) return new BaseResponse { Description = "Thông tin cài đặt null" };
            if (!rq.StartDate.IsValidDatetime())
                return new BaseResponse { Description = "Ngày lắp đặt chưa không chính xác" };

            var device = Cache.GetQueryContext<Device>().GetByKey(rq.Serial);
            if (device == null) return new BaseResponse { Description = "Không tìm thấy thiết bị" };
            var data =
                new P212SetupStartDate
                {
                    StartDate = rq.StartDate
                }.Serializer();
            return BuildSetup(data, rq.Username, device.Serial, "Cài Đặt ngày lắp", typeof(P212SetupStartDate));
        }

        /// <summary>
        ///     cài đặt thông tin cơ bản cho thiết bị
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupNormal(SetupNormal rq)
        {
            if (rq == null) return new BaseResponse {Description = "Thông tin cài đặt null"};

            try
            {
                var device = Cache.GetQueryContext<Device>().GetByKey(rq.Serial);
                if (device == null) return new BaseResponse { Description = "Không tìm thấy thiết bị" };

                #region  lưu cấu hình thiết bị vào db

                bool iscreate = device.SetupInfo == null;
                if (iscreate) device.SetupInfo = new DeviceSetupInfo() { Serial = device.Serial, Device = device };

                device.SetupInfo.OverSpeed = rq.OverSpeed;
                device.SetupInfo.OverSpeedDefault = rq.OverSpeed;
                device.SetupInfo.OverTimeInDay = rq.OverTimeInDay;
                device.SetupInfo.OverTimeInSession = rq.OverTimeInSession;
                device.SetupInfo.TimeSync = rq.TimeSync;
                device.SetupInfo.TimeUpdate = DateTime.Now;

                if (rq.PhoneSystemControl.Count > 0)
                    device.SetupInfo.AllPhoneSystem = rq.PhoneSystemControl.Aggregate((c, a) => c + '|' + a);
                else
                    device.SetupInfo.AllPhoneSystem = "";

                if(iscreate)
                    DataContext.Insert(device.SetupInfo, MotherSqlId);
                else
                    DataContext.Update(device.SetupInfo, MotherSqlId);

                DataContext.Commit(MotherSqlId);

                #endregion  lưu cấu hình thiết bị vào db

                var data = new P202SetupDevice
                {
                    OverSpeed = rq.OverSpeed,
                    OverTimeInDay = rq.OverTimeInDay,
                    OverTimeInSession = rq.OverTimeInSession,
                    PhoneSystemControl = rq.PhoneSystemControl,
                    TimeSync = rq.TimeSync,
                    Bs = rq.Bs
                }.Serializer();

                //Log.Info("SetupNormal", $"Serial {rq.Serial} Bs={rq.Bs} TimeSync={rq.TimeSync} OverSpeed={rq.OverSpeed} OverTimeInDay={rq.OverTimeInDay} OverTimeInSession={rq.OverTimeInSession}");

                return BuildSetup(data, rq.Username, device.Serial, "Cài đặt cơ bản", typeof(P202SetupDevice), true,true);
            }
            catch (Exception e)
            {
                Log.Exception("DeviceSetupController", e, "SetupNormal");
                return new BaseResponse { Description = "Lỗi hệ thống" };
            }
        }

        /// <summary>
        ///     lấy thông tin setup xuống thiết bị thiết bị
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public SetupGetMulti GetRequestSetup(long serial, DateTime begin, DateTime end)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new SetupGetMulti {Description = "Không tìm thấy thiết bị"};

            var result =
                DataContext.GetWhere<DeviceSetupRequest>(
                    m => m.Serial == serial && m.Request >= begin && m.Request <= end, MotherSqlId);
            return new SetupGetMulti
            {
                Status = 1,
                Description = "OK",
                SetupInfos = result.Select(m => new SetupInfoTranfer
                {
                    Complete = m.Complete,
                    Request = m.Request,
                    Data = m.Data,
                    Id = m.Id,
                    Note = m.Note,
                    Response = m.Request,
                    Serial = m.Serial,
                    UserName = m.UserName
                }).ToList()
            };
        }

        /// <summary>
        ///     Lấy thông tin cài đặt ở dưới thiết bị lên
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceSetUpInfoRequest(long serial, string username)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new BaseResponse {Description = "Không tìm thấy thiết bị"};

            var data = new P204ReadDeviceInfo().Serializer();

            return BuildSetup(data, username, device.Serial, "Yêu cầu đọc thông tin cài đăt",
                typeof (P204ReadDeviceInfo));
        }

        /// <summary>
        ///  Cài đặt theo opcode không tham số 
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="username"></param>
        /// <param name="opcode"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse SetupDeviceNoParam(long serial, short opcode, string username,int retry = 1)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new BaseResponse { Description = "Không tìm thấy thiết bị" };

            BaseResponse ret=null;

            if (retry < 1) retry = 1;


            for (int i = 0; i < retry; i++)
            {
                byte[] data;
                switch (opcode)
                {
                    case 210:
                        data = new P210DeviceUpdate() {  Name=""}.Serializer();
                        ret = BuildSetup(data, username, device.Serial, $"Yêu cầu update firmware, retry {i + 1}",
                            typeof(P210DeviceUpdate));
                        break;

                    case 220:
                        data = new P220TurnOffSound().Serializer();
                        ret = BuildSetup(data, username, device.Serial, $"Yêu cầu tắt âm thanh, retry {i+1}",
                            typeof(P220TurnOffSound));
                        break;

                    case 221:
                        data = new P221TurnOnSound().Serializer();
                        ret = BuildSetup(data, username, device.Serial, $"Yêu cầu bật âm thanh, retry {i+1}",
                            typeof(P221TurnOnSound));
                        break;

                    case 222:
                        data = new P222LogoutDriver().Serializer();
                        ret = BuildSetup(data, username, device.Serial, $"Yêu cầu logout tài xế, retry {i+1}",
                            typeof(P222LogoutDriver));
                        break;

                    case 223:
                        data = new P223TurnOffRF().Serializer();
                        ret = BuildSetup(data, username, device.Serial, $"Yêu cầu turn off RFID, retry {i+1}",
                            typeof(P223TurnOffRF));
                        break;

                    case 224:
                        data = new P224TurnOnRF().Serializer();
                        ret = BuildSetup(data, username, device.Serial, $"Yêu cầu turn on RFID, retry {i+1}",
                            typeof(P224TurnOnRF));
                        break;
                }
            }

            if (ret != null) return ret;
            return new BaseResponse { Description = $"chưa xử lý opcode {opcode} này"};
        }


        /// <summary>
        ///  Cài đặt theo opcode không tham số 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="username"></param>
        /// <param name="opcode"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse SetupCompanyNoParam(long companyId, short opcode, string username, int retry = 1)
        {
            BaseResponse ret = null;
            if (retry < 1) retry = 1;

            var devices = Cache.GetQueryContext<Device>().GetByCompany(companyId);
            foreach(var device in devices)
            {
                for (int i = 0; i < retry; i++)
                {
                    byte[] data;
                    switch (opcode)
                    {
                        case 210:
                            data = new P210DeviceUpdate() { Name = "" }.Serializer();
                            ret = BuildSetup(data, username, device.Serial, $"Yêu cầu update firmware, retry {i + 1}",
                                typeof(P210DeviceUpdate),false);
                            break;

                        case 220:
                            data = new P220TurnOffSound().Serializer();
                            ret = BuildSetup(data, username, device.Serial, $"Yêu cầu tắt âm thanh, retry {i + 1}",
                                typeof(P220TurnOffSound), false);
                            break;

                        case 221:
                            data = new P221TurnOnSound().Serializer();
                            ret = BuildSetup(data, username, device.Serial, $"Yêu cầu bật âm thanh, retry {i + 1}",
                                typeof(P221TurnOnSound), false);
                            break;

                        case 222:
                            data = new P222LogoutDriver().Serializer();
                            ret = BuildSetup(data, username, device.Serial, $"Yêu cầu logout tài xế, retry {i + 1}",
                                typeof(P222LogoutDriver), false);
                            break;

                        case 223:
                            data = new P223TurnOffRF().Serializer();
                            ret = BuildSetup(data, username, device.Serial, $"Yêu cầu turn off RFID, retry {i + 1}",
                                typeof(P223TurnOffRF), false);
                            break;

                        case 224:
                            data = new P224TurnOnRF().Serializer();
                            ret = BuildSetup(data, username, device.Serial, $"Yêu cầu turn on RFID, retry {i + 1}",
                                typeof(P224TurnOnRF), false);
                            break;
                    }
                }
            }


            if (ret != null)
            {
                try
                {
                    DataContext.Commit(MotherSqlId);
                }
                catch (Exception e)
                {
                    Log.Exception("DeviceSetupController", e, "thêm gói tin lập trình vào database ko thành công");
                    return new BaseResponse { Description = "Lỗi hệ thống ." };
                }
            }

            if (ret != null) return ret;

            return new BaseResponse { Description = $"chưa xử lý opcode {opcode} này" };
        }

        /// <summary>
        ///     xóa bỏ thông tin muốn gưởi xuống thiết bị
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long id)
        {
            var setup = DataContext.Get<DeviceSetupRequest>(id, MotherSqlId);
            if (setup == null) return new BaseResponse {Status = 0, Description = "Không tìm thấy thông tin yêu cầu"};

            try
            {
                DataContext.Delete(setup, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return new BaseResponse {Status = 1, Description = "Xóa thành công"};
            }
            catch (Exception e)
            {
                Log.Exception("DeviceSetupController", e, $"Xóa thông tin setup id{id} không thành công ");
                return new BaseResponse {Description = "Không thể xóa thông tin này trong database"};
            }
        }

        /// <summary>
        ///     lấy thông tin cài đặt của 1 thiết bị
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public DeviceSettingGet GetDeviceSettingBySerial(long serial)
        {
            var device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (device == null) return new DeviceSettingGet {Description = "Không tìm thấy thiết bị"};
            if (device.SetupInfo == null)
                return new DeviceSettingGet {Description = "Chưa đọc được thông tin cài đặt của thiết bị"};
            return new DeviceSettingGet
            {
                Status = 1,
                Description = "OK",
                Data = new DeviceSettingInfo
                {
                    Serial = serial,
                    FirmWareVersion = device.SetupInfo.FirmWareVersion,
                    HardWareVersion = device.SetupInfo.HardWareVersion,
                    OverSpeed = device.SetupInfo.OverSpeed,
                    OverTimeInDay = device.SetupInfo.OverTimeInDay,
                    OverTimeInSession = device.SetupInfo.OverTimeInSession,
                    PhoneSystemControl = device.SetupInfo.PhoneSystemControl,
                    TimeUpdate = device.SetupInfo.TimeUpdate,
                    TimeSync = device.SetupInfo.TimeSync
                }
            };
        }

        /// <summary>
        ///     lấy thông tin cài đặt của tát cả các thiết  bị trong 1 công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DeviceSettingGetMulti GetDeviceSettingByCompany(long companyId)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new DeviceSettingGetMulti {Description = "Không tìm thấy công ty"};

            var devices = Cache.GetQueryContext<Device>().GetByCompany(companyId);
            return new DeviceSettingGetMulti
            {
                Status = 1,
                Description = "OK",
                Data = devices.Where(m => m.SetupInfo != null).Select(device => new DeviceSettingInfo
                {
                    Serial = device.Serial,
                    FirmWareVersion = device.SetupInfo.FirmWareVersion,
                    HardWareVersion = device.SetupInfo.HardWareVersion,
                    OverSpeed = device.SetupInfo.OverSpeed,
                    OverTimeInDay = device.SetupInfo.OverTimeInDay,
                    OverTimeInSession = device.SetupInfo.OverTimeInSession,
                    PhoneSystemControl = device.SetupInfo.PhoneSystemControl,
                    TimeUpdate = device.SetupInfo.TimeUpdate,
                    TimeSync = device.SetupInfo.TimeSync
                }).ToList()
            };
        }

        /// <summary>
        ///     lấy thông tin cài đặt của các thiết bị trong 1 nhóm xe
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public DeviceSettingGetMulti GetDeviceSettingByGroup(long companyId, long group)
        {
            var company = Cache.GetCompanyById(companyId);
            if (company == null) return new DeviceSettingGetMulti {Description = "Không tìm thấy công ty"};
            var gr = Cache.GetQueryContext<DeviceGroup>().GetByKey(group);
            if (gr == null) return new DeviceSettingGetMulti {Description = "Không tìm thấy nhóm xe"};
            var devices = Cache.GetQueryContext<Device>().GetByGroup(companyId, group);
            return new DeviceSettingGetMulti
            {
                Status = 1,
                Description = "OK",
                Data = devices.Where(m => m.SetupInfo != null).Select(device => new DeviceSettingInfo
                {
                    Serial = device.Serial,
                    FirmWareVersion = device.SetupInfo.FirmWareVersion,
                    HardWareVersion = device.SetupInfo.HardWareVersion,
                    OverSpeed = device.SetupInfo.OverSpeed,
                    OverTimeInDay = device.SetupInfo.OverTimeInDay,
                    OverTimeInSession = device.SetupInfo.OverTimeInSession,
                    PhoneSystemControl = device.SetupInfo.PhoneSystemControl,
                    TimeUpdate = device.SetupInfo.TimeUpdate,
                    TimeSync = device.SetupInfo.TimeSync
                }).ToList()
            };
        }
    }
}