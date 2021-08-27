using System;
using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Setup;
using StarSg.Utils.Models.Tranfer.Setup;
using Route.Api.Auth.Models.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Route.Api.Controllers
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
        ///     yêu cầu thiết bị cập nhật phần mềm
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="rq">thông tin phần mềm cần cập nhật</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse UpdateFirmware(long companyId, UpdateFirmware rq)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền cập nhật" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/UpdateFirmware", rq);
            AddAccessHistory(ret, rq.Serial, AccessHistoryMethod.Setup, $"Cập nhật phần mềm cho {rq.Serial} tên {rq.Name}");
            AddSetupDeviceHistory(ret, rq.Serial,companyId,(short)210, "Firmware");
            return ret;
        }

        /// <summary>
        ///     thiết lập thông tin tài xế xuống thiết bị
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="rq">thông tin tài xế cần cập nhật</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupDriver(long companyId, SetupDriver rq)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền cài đặt tài xế" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/SetupDriver", rq);
            AddAccessHistory(ret, rq.Serial, AccessHistoryMethod.Setup, $"Cập nhật thông tin tài xế cho {rq.Serial} Mã tài xế {rq.DriverId}");
            AddSetupDeviceHistory(ret, rq.Serial, companyId, (short)201, "Thông tin tài xế");
            return ret;
        }

        /// <summary>
        ///     cài đặt giới hạn vận tốc
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="rq">thông tin quá vận tốc cần cài đặt</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupOverSpeed(long companyId, SetupOverSpeed rq)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền cài đặt giới hạn vận tốc" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/SetupOverSpeed", rq);
            AddAccessHistory(ret, rq.Serial, AccessHistoryMethod.Setup, $"Cài đặt giới hạn vận tốc cho {rq.Serial} tốc độ {rq.MaxSpeed}");
            AddSetupDeviceHistory(ret, rq.Serial, companyId, (short)203, "Giới hạn vận tốc");
            return ret;
        }

        /// <summary>
        ///     Cài đặt thông tin biển số xe 
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="rq">thông tin cài đặt</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupBS(long companyId, SetupBS rq)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền cài đặt thông tin biển số xe" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/SetupBS", rq);
            AddAccessHistory(ret, rq.Serial, AccessHistoryMethod.Setup, $"Cài đặt thông tin biển số xe {rq.Serial} Biển số {rq.Bs}");
            AddSetupDeviceHistory(ret, rq.Serial, companyId, (short)211, "Biển số xe");
            return ret;
        }

        /// <summary>
        ///     Cài Đặt mốc ngày lắp đặt xuống thiết bị 
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="rq">thông tin cài đặt</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupStartDate(long companyId, SetupStartDate rq)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền cài đặt mốc ngày lắp đặt" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/SetupStartDate", rq);
            AddAccessHistory(ret, rq.Serial, AccessHistoryMethod.Setup, $"Cài Đặt mốc ngày lắp đặt {rq.Serial} ngày {rq.StartDate}");
            AddSetupDeviceHistory(ret, rq.Serial, companyId, (short)212, "Ngày lắp đặt");
            return ret;
        }


        /// <summary>
        ///     cài đặt thông tin cơ bản cho thiết bị
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="rq">thông tin cơ bản</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse SetupNormal(long companyId, SetupNormal rq)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền cài đặt thông tin cơ bản" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Post<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/SetupNormal", rq);
            AddAccessHistory(ret, rq.Serial, AccessHistoryMethod.Setup, $"Cài Đặt thông tin cơ bản {rq.Serial}");
            AddSetupDeviceHistory(ret, rq.Serial, companyId, (short)202, "Thông tin cơ bản");
            return ret;
        }

        /// <summary>
        ///     lấy thông tin setup xuống thiết bị thiết bị
        /// </summary>
        /// <param name="serial">serial thiêt bị</param>
        /// <param name="begin">thời gian bắt đầu</param>
        /// <param name="end">thời gian kết thúc</param>
        /// <returns></returns>
        [HttpGet]
        public SetupGetMulti GetRequestSetup(long serial, DateTime begin, DateTime end)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new SetupGetMulti { Description = "Không có quyền lấy thông tin setup " };

            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null)
                return new SetupGetMulti {Description = "Không tìm thấy thông tin máy chủ xử lý"};
            var api = new ForwardApi();
            return
                api.Get<SetupGetMulti>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/GetRequestSetup?serial={serial}&begin={begin}&end={end}");
        }

        /// <summary>
        ///     xóa bỏ thông tin muốn gưởi xuống thiết bị
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="id">id của record</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long companyId, long id)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền xóa bỏ thông tin thiết bị " };

            Log.Debug("Controller", $"Yêu cầu xóa record setup công ty : {companyId} , idRecord: {id}");
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse {Description = "Không tìm thấy thông tin máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/DeviceSetup/Del?id={id}");
            AddAccessHistory(ret, id, AccessHistoryMethod.Delete, $"Xóa bỏ thông tin muốn gửi xuống thiết bị {id}");
            return ret;
        }

        /// <summary>
        /// yêu cầu lấy thông tin đã cài đặt xuống thiết bị
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="companyId">id công ty</param>
        /// <param name="username">tài khoản yêu cầu (thực chất ko cần thiết )</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse GetDeviceSetUpInfoRequest(long serial, long companyId,string username)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền yêu cầu lấy thông tin đã cài đặt" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/GetDeviceSetUpInfoRequest?serial={serial}&username={username}");
        }


        /// <summary>
        /// Cài đặt theo opcode không tham số 
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="companyId">id công ty</param>
        /// <param name="opcode">opcode</param>
        /// <param name="username">tài khoản yêu cầu (thực chất ko cần thiết )</param>
        /// <param name="retry">số lần nạp lệnh</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse SetupDeviceNoParam(long serial, long companyId, short opcode, string username, int retry = 1)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền yêu cầu lấy thông tin đã cài đặt" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            var api = new ForwardApi();

            BaseResponse ret = api.Get<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/SetupDeviceNoParam?serial={serial}&opcode={opcode}&username={username}&retry={retry}");

            AddAccessHistory(ret, serial, AccessHistoryMethod.Setup, $"Cài đặt theo opcode không tham số");
            AddSetupDeviceHistory(ret,serial, companyId, opcode, "Cài đặt theo opcode",retry);

            return ret;


        }

        /// <summary>
        /// Cài đặt theo opcode không tham số 
        /// </summary>
        /// <param name="serials">danh sach serial thiết bị cach nhau , | ; </param>
        /// <param name="opcode">opcode</param>
        /// <param name="username">tài khoản yêu cầu (thực chất ko cần thiết )</param>
        /// <param name="retry">số lần nạp lệnh</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse SetupDeviceNoParams(String serials,short opcode, string username, int retry = 1)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền yêu cầu lấy thông tin đã cài đặt" };

            var allId = new List<long>();
            try
            {
                allId = serials.Split('|', ',', ';', ' ').Where(m => !string.IsNullOrEmpty(m)).Select(m => long.Parse(m.Trim())).ToList();
            }
            catch (Exception)
            {
                return new BaseResponse { Description = $"{serials} sai dinh dang" };
            }
            if (allId.Count == 0) return new BaseResponse { Description = $"{serials} rong" };

            var api = new ForwardApi();
            BaseResponse ret = new BaseResponse();
            foreach (long serial in allId)
            {
                var center = DeviceRoute.GetDataCenter(serial);
                if (center == null) continue;

                ret = api.Get<BaseResponse>(
                        $"{center.Ip}:{center.Port}/api/DeviceSetup/SetupDeviceNoParam?serial={serial}&opcode={opcode}&username={username}&retry={retry}");
            }

            AddAccessHistory(ret, 0, AccessHistoryMethod.Setup, $"Cài đặt {serials} theo opcode không tham số");
            AddSetupDeviceHistory(ret, 0, 0, opcode, $"Cài đặt {serials} theo opcode", retry);

            return ret;
        }


        /// <summary>
        /// Cài đặt theo opcode không tham số 
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="opcode">opcode</param>
        /// <param name="username">tài khoản yêu cầu (thực chất ko cần thiết )</param>
        /// <param name="retry">số lần nạp lệnh</param>
        /// <returns></returns>
        [HttpGet]
        public BaseResponse SetupCompanyNoParam(long companyId, short opcode, string username, int retry = 1)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền yêu cầu lấy thông tin đã cài đặt" };

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new BaseResponse { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            var api = new ForwardApi();

            BaseResponse ret = api.Get<BaseResponse>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/SetupCompanyNoParam?companyId={companyId}&opcode={opcode}&username={username}&retry={retry}");

            AddAccessHistory(ret, 0, AccessHistoryMethod.Setup, $"Cài đặt theo opcode không tham số");
            AddSetupDeviceHistory(ret, 0, companyId, opcode, "Cài đặt theo opcode", retry);

            return ret;


        }


        /// <summary>
        ///     lấy thông tin cài đặt của 1 thiết bị
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <returns></returns>
        public DeviceSettingGet GetDeviceSettingBySerial(long serial)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center == null)
                return new DeviceSettingGet { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<DeviceSettingGet>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/GetDeviceSettingBySerial?serial={serial}");
        }

        /// <summary>
        ///     lấy thông tin cài đặt của tát cả các thiết  bị trong 1 công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        public DeviceSettingGetMulti GetDeviceSettingByCompany(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceSettingGetMulti { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<DeviceSettingGetMulti>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/GetDeviceSettingByCompany?companyId={companyId}");
        }

        /// <summary>
        ///     lấy thông tin cài đặt của các thiết bị trong 1 nhóm xe
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="group">id nhóm xe</param>
        /// <returns></returns>
        public DeviceSettingGetMulti GetDeviceSettingByGroup(long companyId, long group)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceSettingGetMulti { Description = "Không tìm thấy thông tin máy chủ xử lý" };
            var api = new ForwardApi();
            return
                api.Get<DeviceSettingGetMulti>(
                    $"{center.Ip}:{center.Port}/api/DeviceSetup/GetDeviceSettingByGroup?companyId={companyId}&group={group}");
        }


        protected void AddSetupDeviceHistory(BaseResponse res, long serial, long companyId, short opcode, string note,int retry = 1)
        {
            if (res == null) return;
            if (res.Status == 0) return;
            AddSetupDeviceHistory(serial, companyId, opcode, note,retry);
        }

        protected void AddSetupDeviceHistory(long serial, long companyId, short opcode, string note, int retry)
        {
            try
            {
                using (var db = _authloader.GetContext())
                {
                    long groupid = UserPermision.GetUserGroupId(companyId);
                    db.Insert<SetupDeviceHistory>(new SetupDeviceHistory()
                    {
                        Username = UserPermision.User.UserName,
                        AtTime = DateTime.Now,
                        CompanyId = companyId,
                        GroupId = groupid,
                        Serial = serial,
                        opcode = opcode,
                        Retry = retry,
                        Note = note
                    });
                }
            }
            catch (Exception e)
            {
                Log.Exception("SetupDeviceHistory", e, "Lỗi thêm vô lịch sử truy xuất");
            }
        }

    }
}