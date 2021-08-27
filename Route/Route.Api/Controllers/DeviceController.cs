using System;
using System.ComponentModel.Composition;
using System.Web.Http;
using Route.Api.Core;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Device;
using StarSg.Utils.Models.Tranfer;
using System.Collections.Generic;
using Route.DeviceServer;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý thông tin thiết bị
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeviceController : BaseController
    {
        [Import]
        private ClientCachePacket _cachePacket;


        /// <summary>
        ///     thêm mới thiết bị
        /// </summary>
        /// <param name="device">thông tin thiết bị</param>
        /// <returns></returns>
        [HttpPost]
        public DeviceAdd Add(DeviceTranfer device)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer ) return new DeviceAdd { Description = "Không có quyền thêm thiết bị" };

            var center = CompanyRoute.GetDataCenter(device.CompanyId);
            if (center == null)
                return new DeviceAdd {Description = "không tìm thấy máy chủ xử lý"};

            var api = new ForwardApi();
            DeviceAdd ret = api.Post<DeviceAdd>($"{center.Ip}:{center.Port}/api/Device/Add", device);

            if(ret.Status>0 && _cachePacket != null)
            {
                _cachePacket.RemoveUnknownDevice(device.Serial);
            }

            AddAccessHistory(ret,device.Serial, AccessHistoryMethod.Add, $"Thêm thiết bị {device.Bs}");

            return ret;
        }

        /// <summary>
        ///     Cập nhật thông tin thiết bị
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <param name="tran"></param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(long serial, DeviceTranfer tran)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền sửa thiết bị" };

            //var center = DeviceRoute.GetDataCenter(serial);
            var center = GetDataCenter(serial);
            if (center == null)
                return new BaseResponse {Description = "không tìm thấy máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/Device/Update?serial={serial}", tran);
            AddAccessHistory(ret,serial, AccessHistoryMethod.Edit, $"Thay đổi thiết bị {tran.Bs}");
            return ret;
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
            //if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster ) return new DeviceAdd { Description = "Không có quyền gia hạn thiết bị" };

            if (UserPermision.GetLevel() == (int)AccountLevel.Root
                || (UserPermision.GetLevel() == (int)AccountLevel.Administrator && "root".Equals(UserPermision.User.GroupUserId)) 
                )
            {
                //var center = DeviceRoute.GetDataCenter(serial);
                var center = GetDataCenter(serial);
                if (center == null)
                    return new BaseResponse { Description = "không tìm thấy máy chủ xử lý" };

                var api = new ForwardApi();
                BaseResponse ret = api.Get<BaseResponse>($"{center.Ip}:{center.Port}/api/Device/GetRenew?serial={serial}&days={days}");
                AddAccessHistory(ret, serial, AccessHistoryMethod.Edit, $"Gia hạn thiết bị serial {serial} {days} ngày");
                return ret;
            }

            return new DeviceAdd { Description = "Không có quyền gia hạn thiết bị" };
        }


        /// <summary>
        ///     xóa thiết bị
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Del(long serial)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster ) return new BaseResponse { Description = "Không có quyền xóa thiết bị" };

            //var center = DeviceRoute.GetDataCenter(serial);
            var center = GetDataCenter(serial);
            if (center == null)
                return new DeviceAdd {Description = "không tìm thấy máy chủ xử lý"};

            var api = new ForwardApi();
            BaseResponse ret = api.Del<BaseResponse>($"{center.Ip}:{center.Port}/api/Device/Del?serial={serial}");
            AddAccessHistory(ret,serial, AccessHistoryMethod.Delete, $"Xóa thiết bị serial {serial}");
            return ret;
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo serial
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetSingle GetDeviceBySerial(long serial)
        {
            //var center = DeviceRoute.GetDataCenter(serial);
            var center = GetDataCenter(serial);
            if (center == null)
                return new DeviceGetSingle {Description = "không tìm thấy máy chủ xử lý"};
            var api = new ForwardApi();
            return api.Get<DeviceGetSingle>($"{center.Ip}:{center.Port}/api/Device/GetDeviceBySerial?serial={serial}");
        }


        /// <summary>
        ///     lấy thông tin ở rộng của thiết bị theo serial
        /// </summary>
        /// <param name="serial">serial thiết bị</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceBySerialEx(long serial)
        {
            //var center = DeviceRoute.GetDataCenter(serial);
            var center = GetDataCenter(serial);
            if (center == null)
                return new DeviceGetMultiEx { Description = "không tìm thấy máy chủ xử lý" };
            var api = new ForwardApi();

            DeviceGetMultiEx ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceBySerialEx?serial={serial}");
            UpdateDeviceGetMultiEx(ret);
            return ret;
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo bản số xe
        /// </summary>
        /// <param name="bs">bản số xe</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetSingle GetDeviceByNumber(String bs)
        {
            var centers = DataCenterStore.GetAll();
            foreach(var center in  centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                DeviceGetSingle ret = api.Get<DeviceGetSingle>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByNumber?bs={bs}");
                if (ret.Status > 0) return ret;
            }
            return new DeviceGetSingle { Description = "Không tìm thấy thiết bị" };
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo bản số xe
        /// </summary>
        /// <param name="bs">bản số xe</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByNumberEx(String bs)
        {
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                DeviceGetMultiEx ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByNumberEx?bs={bs}");
                if (ret.Status > 0)
                {
                    UpdateDeviceGetMultiEx(ret);
                    return ret;
                }
            }
            return new DeviceGetMultiEx { Description = "Không tìm thấy thiết bị" };
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo số điện thoại
        /// </summary>
        /// <param name="phone">số điện thoại</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByPhoneEx(String phone)
        {
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                DeviceGetMultiEx ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByPhoneEx?phone={phone}");
                if (ret.Status > 0)
                {
                    UpdateDeviceGetMultiEx(ret);
                    return ret;
                }
            }
            return new DeviceGetMultiEx { Description = "Không tìm thấy thiết bị" };
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo số Vin
        /// </summary>
        /// <param name="vin">số vin</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByVinEx(String vin)
        {
            var centers = DataCenterStore.GetAll();
            foreach (var center in centers)
            {
                if (center == null) continue;
                var api = new ForwardApi();
                DeviceGetMultiEx ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByVinEx?vin={vin}");
                if (ret.Status > 0)
                {
                    UpdateDeviceGetMultiEx(ret);
                    return ret;
                }
            }
            return new DeviceGetMultiEx { Description = "Không tìm thấy thiết bị" };
        }

        /// <summary>
        ///     lấy thông tin mở rộng thiết bị theo ngày tạo
        /// </summary>
        /// <param name="from">từ ngày</param>
        /// <param name="to">đến ngày</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByCreateDateEx(DateTime from, DateTime to)
        {
            var devicelist = new List<DeviceTranferEx>();

            //level 0 1 thì xem toàn bộ
            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
            {
                var centers = DataCenterStore.GetAll();
                foreach (var center in centers)
                {
                    if (center == null) continue;
                    var api = new ForwardApi();
                    DeviceGetMultiEx subret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByCreateDateEx?from={from}&to={to}");
                    if (subret.Status > 0 && subret.Devices != null)
                        devicelist.AddRange(subret.Devices);
                }
            }
            //level còn lại thì phải lấy theo công ty của user quản lý
            else
            {
                IList<long> companyids = UserPermision.GetCompany();
                foreach (var cid in companyids)
                {
                    var companyId = cid > Auth.Core.AccountManager.COMPANYLIMIT ? cid / Auth.Core.AccountManager.COMPANYLIMIT : cid;
                    var center = CompanyRoute.GetDataCenter(companyId);
                    if(center!=null)
                    {
                        var api = new ForwardApi();
                        DeviceGetMultiEx subret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByCreateDateEx?from={from}&to={to}&companyId={companyId}");
                        if (subret.Status > 0 && subret.Devices != null)
                            devicelist.AddRange(subret.Devices);
                    }
                }
            }

            if (devicelist.Count > 0)
            {
                var ret = new DeviceGetMultiEx() { Status = 1, Description = "OK", Devices = devicelist };
                UpdateDeviceGetMultiEx(ret);
                return ret;
            }

            return new DeviceGetMultiEx { Description = "Không tìm thấy thiết bị" };
        }


        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị có chứa ghi chú
        /// </summary>
        /// <param name="note">Ghi chú có chứa giá trị này</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetDeviceByNoteEx(String note)
        {
            var devicelist = new List<DeviceTranferEx>();

            //level 0 1 thì xem toàn bộ
            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
            {
                var centers = DataCenterStore.GetAll();
                foreach (var center in centers)
                {
                    if (center == null) continue;
                    var api = new ForwardApi();
                    DeviceGetMultiEx subret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByNoteEx?note={note}");
                    if (subret.Status > 0 && subret.Devices != null)
                        devicelist.AddRange(subret.Devices);
                }
            }
            //level còn lại thì phải lấy theo công ty của user quản lý
            else
            {
                IList<long> companyids = UserPermision.GetCompany();
                foreach (var cid in companyids)
                {
                    var companyId = cid > Auth.Core.AccountManager.COMPANYLIMIT ? cid / Auth.Core.AccountManager.COMPANYLIMIT : cid;
                    var center = CompanyRoute.GetDataCenter(companyId);
                    if (center != null)
                    {
                        var api = new ForwardApi();
                        DeviceGetMultiEx subret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceByNoteEx?note={note}&companyId={companyId}");
                        if (subret.Status > 0 && subret.Devices != null)
                            devicelist.AddRange(subret.Devices);
                    }
                }
            }

            if (devicelist.Count > 0)
            {
                var ret = new DeviceGetMultiEx() { Status = 1, Description = "OK", Devices = devicelist };
                UpdateDeviceGetMultiEx(ret);
                return ret;
            }

            return new DeviceGetMultiEx { Description = "Không tìm thấy thiết bị" };

        }


        /// <summary>
        ///     lấy thông tin xe theo group id
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="groupId">id đội xe, nếu bằng 0 thì lấy theo id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetByGroupId(long companyId, long groupId)
        {
            if (groupId == 0)
                return GetByCompanyId(companyId);

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceGetMulti {Description = "không tìm thấy máy chủ xử lý"};

            var api = new ForwardApi();

            //dành cho khách lẻ, bõ qua groupId
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer)
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if (serials != null && serials.Count > 0)
                    return api.Get<DeviceGetMulti>($"{center.Ip}:{center.Port}/api/Device/GetDeviceBySerials?serials={String.Join(",", serials)}");
            }

            return api.Get<DeviceGetMulti>($"{center.Ip}:{center.Port}/api/Device/GetByGroupId?groupId={groupId}");
        }

        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo group id
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <param name="groupId">id đội xe, nếu bằng 0 thì lấy theo id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetByGroupIdEx(long companyId, long groupId)
        {
            if (groupId == 0)
                return GetByCompanyIdEx(companyId);

            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceGetMultiEx { Description = "không tìm thấy máy chủ xử lý" };

            var api = new ForwardApi();

            DeviceGetMultiEx ret = null;

            //dành cho khách lẻ, bõ qua groupId
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer)
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if (serials != null && serials.Count > 0)
                    ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceBySerialsEx?serials={String.Join(",", serials)}");
            }

            if(null==ret) ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetByGroupIdEx?groupId={groupId}");

            UpdateDeviceGetMultiEx(ret);

            return ret;
        }

        /// <summary>
        ///     lấy thông tin thiết bị theo công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMulti GetByCompanyId(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceGetMulti {Description = "không tìm thấy máy chủ xử lý"};

            var api = new ForwardApi();

            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster  ) return api.Get<DeviceGetMulti>($"{center.Ip}:{center.Port}/api/Device/GetByCompanyId?companyId={companyId}");

            //dành cho khách lẻ
            if(UserPermision.GetLevel()>= (int)AccountLevel.Customer)
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if(serials!=null && serials.Count>0)
                    return api.Get<DeviceGetMulti>($"{center.Ip}:{center.Port}/api/Device/GetDeviceBySerials?serials={String.Join(",", serials)}");
            }

            long groupId = UserPermision.GetUserGroupId(companyId);
            if(groupId==-1) return api.Get<DeviceGetMulti>($"{center.Ip}:{center.Port}/api/Device/GetByCompanyId?companyId={companyId}");

            if (groupId > 0)
                return api.Get<DeviceGetMulti>($"{center.Ip}:{center.Port}/api/Device/GetByGroupId?groupId={groupId}");
            else
                //return api.Get<DeviceGetMulti>( $"{center.Ip}:{center.Port}/api/Device/GetByCompanyId?companyId={companyId}");
                return new DeviceGetMulti
                {
                    Status = 1,
                    Description = "OK",
                    Devices = new List<DeviceTranfer>()
                };
        }


        /// <summary>
        ///     lấy thông tin mở rộng của thiết bị theo công ty
        /// </summary>
        /// <param name="companyId">id công ty</param>
        /// <returns></returns>
        [HttpGet]
        public DeviceGetMultiEx GetByCompanyIdEx(long companyId)
        {
            var center = CompanyRoute.GetDataCenter(companyId);
            if (center == null)
                return new DeviceGetMultiEx { Description = "không tìm thấy máy chủ xử lý" };

            var api = new ForwardApi();

            DeviceGetMultiEx ret = null;

            if (UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster) ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetByCompanyIdEx?companyId={companyId}");

            //dành cho khách lẻ
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer)
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if (serials != null && serials.Count > 0)
                    ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetDeviceBySerialsEx?serials={String.Join(",", serials)}");
            }

            if(null==ret)
            {
                long groupId = UserPermision.GetUserGroupId(companyId);
                if (groupId == -1) ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetByCompanyIdEx?companyId={companyId}");
                else if (groupId > 0)
                    ret = api.Get<DeviceGetMultiEx>($"{center.Ip}:{center.Port}/api/Device/GetByGroupIdEx?groupId={groupId}");
            }

            if (ret != null)
            {
                UpdateDeviceGetMultiEx(ret);
                return ret;
            }
            else
                //return api.Get<DeviceGetMulti>( $"{center.Ip}:{center.Port}/api/Device/GetByCompanyId?companyId={companyId}");
                return new DeviceGetMultiEx
                {
                    Status = 1,
                    Description = "OK",
                    Devices = new List<DeviceTranferEx>()
                };
        }

        private void UpdateDeviceGetMultiEx(DeviceGetMultiEx devices)
        {
            if (devices == null) return;
            if (devices.Devices == null) return;
            if (_accountManager == null) return;

            try
            {
                //sử dụng users để cache lại những user theo công ty và nhóm khỏi query nhiều lần.
                Dictionary<long, IList<string>> users = new Dictionary<long, IList<string>>();

                foreach (var device in devices.Devices)
                {
                    if (device == null) continue;

                    //device.Users = _accountManager.GetUsersOfGroup(device.CompanyId,device.GroupId);

                    //Lấy danh sách user liên quan đến nhóm và công ty
                    var groupid = device.CompanyId * Auth.Core.AccountManager.COMPANYLIMIT + device.GroupId;//một cách để hợp nhất 2 id thành 1 duy nhất
                    if (!users.ContainsKey(groupid))
                        users.Add(groupid, _accountManager.GetUsersOfGroup(device.CompanyId, device.GroupId));
                    device.Users = users[groupid];

                    //Danh sách user trực tiếp
                    device.DirectlyUsernames = _accountManager.GetUsersOfSerial(device.Serial);
                    if (device.DirectlyUsernames.Count > 0 )
                    {
                        String phonelist = "";
                        foreach (var username in device.DirectlyUsernames)
                        {
                            Auth.Models.Req.AccountInfo userInfo = _accountManager.GetUserInformation(username);
                            if (userInfo != null)
                            {
                                device.DirectlyUsers.Add(userInfo.DisplayName);
                                phonelist += "," + userInfo.Phone;
                            }
                        }

                        //Lấy số điện thoại nếu chưa khai báo
                        if (String.IsNullOrWhiteSpace(device.OwnerPhone) && phonelist.Length > 0) device.OwnerPhone = phonelist.Substring(1);
                    }

                }

                users.Clear();
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        ///     Thay đổi thiết bị, Cần phải thay đổi users khách lẽ cho thiết bị mới.
        /// </summary>
        /// <param name="oldSerial">Serial Thiết bị Cũ</param>
        /// <param name="newSerial">Serial Thiết bị Mới</param>
        /// <param name="autoSetup">Tu dong nap lai vo thiet bi tu thong so thiet bi cu (mac dinh)</param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse ChangeDevice(long oldSerial, long newSerial, bool autoSetup = true)
        {
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer ) return new BaseResponse { Description = "Không có quyền đổi thiết bị" };

            //var center = DeviceRoute.GetDataCenter(oldSerial);
            var center = GetDataCenter(oldSerial);
            if (center == null)
                return new BaseResponse { Description = "không tìm thấy máy chủ xử lý" };

            var api = new ForwardApi();
            BaseResponse ret = api.Put<BaseResponse>($"{center.Ip}:{center.Port}/api/Device/ChangeDevice?oldSerial={oldSerial}&newSerial={newSerial}&autoSetup={autoSetup}");
            AddAccessHistory(ret,oldSerial, AccessHistoryMethod.Edit, $"Thay đổi thiết bị serial {oldSerial} thành serial {newSerial} auto {autoSetup}");
            return ret;
        }


        /// <summary>
        ///     Giả lặp dữ liệu hiện hành
        /// </summary>
        /// <param name="device">thông tin thiết bị giả lặp</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse Fake(DeviceTranferFake device)
        {
            //if (UserPermision.GetLevel() >= (int)AccountLevel.Customer) return new BaseResponse { Description = "Không có quyền giả lặp dữ liệu thiết bị" };
            if (UserPermision.GetLevel() >= (int)AccountLevel.CustomerMaster) return new BaseResponse { Description = "Không có quyền giả lặp dữ liệu thiết bị" };

            var center = DeviceRoute.GetDataCenter(device.Serial);
            if (center == null)
                return new BaseResponse { Description = "không tìm thấy máy chủ xử lý" };

            DevicePacketModels.P01SyncPacket packet = new DevicePacketModels.P01SyncPacket()
            {
                Serial = device.Serial
                 ,
                Time = device.Time
                 ,
                GpsInfo = new DevicePacketModels.ExternModel.GpsInfo() { Lat = device.GpsInfoLat, Lng = device.GpsInfoLng, Speed = device.GpsInfoSpeed }
                 ,
                StatusIo = new DevicePacketModels.ExternModel.StatusIO()
                {
                    Key = device.StatusIoKey
                      ,
                    AirMachine = device.StatusIoAirMachine
                      ,
                    Door = device.StatusIoDoor
                      ,
                    Sos = device.StatusIoSos
                      ,
                    UseFuel = device.StatusIoUseFuel
                      ,
                    UseRfid = device.StatusIoUseRfid
                      ,
                    UseTemperature = device.StatusIoUseTemperature
                },
                Fuel = device.Fuel
                 ,
                GpsStatus = device.GpsStatus
                 ,
                IOValue = device.IOValue
                 ,
                Power = device.Power
                 ,
                SpeedLogs = device.SpeedLogs
                 ,
                Temperature = device.Temperature
                 ,
                GsmSignal = device.GsmSignal
            };

            BaseResponse ret = new BaseResponse() { Status = 1, Description = "FAKE OK" };

            try
            {
                var fw = new ForwardApi();
                fw.AddHeader("serial", device.Serial.ToString());
                var url = $"{center.Ip}:{center.Port}/api/DevicePacketHandle/Sync";
                Log.Debug("DeviceController", $"URL request sync : {url}");
                fw.Post<byte[]>(url, packet);
            }
            catch (Exception ex)
            {
                Log.Exception("DeviceController", ex, $"fake packet fail {device.Serial}");
                ret.Status = 0; ret.Description = "Giả lặp dữ liệu không thành công";
            }

            AddAccessHistory(ret, device.Serial, AccessHistoryMethod.Add, $"Giả lặp dữ liệu Serial {device.Serial}");

            return ret;
        }


        //sua loi khong biet vi sao serial nay khong ton tai trong bo key
        private Route.Core.DataCenterInfo GetDataCenter(long serial)
        {
            var center = DeviceRoute.GetDataCenter(serial);
            if (center != null) return center;

            //try to add if only one server
            Log.Error("DeviceController",$"Không thấy serial {serial} trong DeviceRoute");
            var centers = DataCenterStore.GetAll();
            if (centers.Count==1)
            {
                foreach (var item in centers)
                {
                    return item;
                }
            }

            return null;
        }

    }
}