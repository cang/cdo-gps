using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Core.Models.Auth;
using Route.Api.Auth.Core;
using StarSg.Core;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Response;
using Route.Api.Auth.Models.Req;
using Route.Api.Auth;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản trị tài khoản
    /// </summary>
    [ValidRequestFilter]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdministrationController : ValidController
    {
        /// <summary>
        ///     bắt buộc đăng xuất
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (BaseResponse))]
        [Route("admin/ForceLogout")]
        public IHttpActionResult ForceLogout(string username)
        {
            if (IsAdminAccount())
            {
                if (AccountManager.ForceLogout(username))
                {
                    Log.Warning("AdministrationController", $"bắt buột đăng xuất username: {username} thành công");
                    return Ok(new BaseResponse {Status = 1, Description = "OK"});
                }
            }
            Log.Warning("AdministrationController", $"bắt buột đăng xuất username: {username} thất bại");
            return Ok(new BaseResponse {Status = 0});
        }

        /// <summary>
        ///     kiểm tra tài khoản admin
        /// </summary>
        /// <returns></returns>
        private bool IsAdminAccount()
        {
            //return Account.Level <= AccountLevel.CustomerMaster;
            return Account.Level < AccountLevel.Customer;
        }

        /// <summary>
        ///     khóa tài khoản
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (BaseResponse))]
        [Route("admin/LockUser")]
        public IHttpActionResult LockUser(string username)
        {
            if (IsAdminAccount())
            {
                if (AdministrationManager.LockUser(username))
                {
                    Log.Warning("AdministrationController", $"khóa tài khoản username: {username} thành công");
                    return Ok(new BaseResponse {Status = 1, Description = "OK"});
                }
            }
            Log.Warning("AdministrationController", $"khóa tài khoản username: {username} thất bại");
            return Ok(new BaseResponse {Status = 0});
        }

        /// <summary>
        ///     lấy log của tài khoản
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (UserLogResponse))]
        [Route("admin/GetUserLog")]
        public IHttpActionResult GetUserLog(string username, DateTime begineDateTime, DateTime endDateTime)
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetUserLog(username, begineDateTime, endDateTime);
                if (result != null)
                {
                    Log.Warning("AdministrationController", $"lấy log của tài khoản username: {username} thành công");
                    return Ok(new UserLogResponse {Status = 1, Description = "OK", UserLogTranfers = result});
                }
            }
            Log.Warning("AdministrationController", $"lấy log của tài khoản username: {username} thất bại");
            return Ok(new UserLogResponse {Status = 0});
        }

        /// <summary>
        ///     lấy toàn bộ danh sách tài khoản theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (UserResponse))]
        [Route("admin/GetUserByCompany")]
        public IHttpActionResult GetUserByCompany(long companyId)
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetUserByCompany(companyId);
                if (result != null)
                {
                    Log.Warning("AdministrationController",
                        $"lấy toàn bộ danh sách tài khoản theo công ty: {companyId} thành công");
                    return Ok(new UserResponse {Status = 1, Description = "OK", AccountTranfers = result});
                }
            }
            Log.Warning("AdministrationController",
                $"lấy toàn bộ danh sách tài khoản theo công ty: {companyId} thất bại");
            return Ok(new UserResponse {Status = 0});
        }

        /// <summary>
        ///     lấy toàn bộ danh sách tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (UserResponse))]
        [Route("admin/GetAllUser")]
        public IHttpActionResult GetAllUser()
        {
            try
            {
                //if (IsAdminAccount())
                {
                    var result = new List<AccountTranfer>();

                    if(Account.Level < AccountLevel.CustomerMaster)
                    {
                        result =
                            AdministrationManager.GetAllUser().Where(m => m.Level > (int)Account.Level).ToList();
                    }
                    else if (Account.Level < AccountLevel.Customer)
                    {
                        foreach (var companyId in Account.CompanyIds)
                        {
                            result.AddRange(
                                AdministrationManager.GetUserByCompany(companyId.CompanyId)
                                    .Where(m => m.Level > (int)Account.Level)
                                    .ToList());
                        }
                    }
                    else // level 3 không quản lý danh sách user
                        result = AdministrationManager.GetAllUser(Account.GroupUserId);

                    if (result != null)
                    {
                        Log.Warning("AdministrationController", $"lấy toàn bộ danh sách tài khoản thành công");
                        return Ok(new UserResponse {Status = 1, Description = "OK", AccountTranfers = result});
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception("AdministrationController", e, "Lấy thông tin user lỗi");
                Console.WriteLine(e);
            }
            Log.Warning("AdministrationController", $"lấy toàn bộ danh sách tài khoản thất bại");
            return Ok(new UserResponse {Status = 0});
        }

        /// <summary>
        ///     tạo tài khoản
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof (BaseResponse))]
        [Route("admin/CreateUser")]
        public IHttpActionResult CreateUser(AccountTranfer account)
        {
            if (IsAdminAccount())
            {
                if (account == null)
                {
                    Log.Warning("AdministrationController", "account truyền vào null");
                    return Ok(new BaseResponse {Status = 0});
                }

                //if (account.Level < (int) Account.Level || account.Level == 0)
                //    return Ok(new BaseResponse {Status = 0, Description = "Không tạo được tài khoản quyền cao hơn "});
                if (account.Level <= (int)Account.Level || account.Level < (int)AccountLevel.Administrator )
                    return Ok(new BaseResponse { Status = 0, Description = "Chỉ tạo được tài khoản quyền thấp hơn" });

                if (AdministrationManager.CreateUser(account))
                {
                    Log.Warning("AdministrationController", $"thêm account {account.Username} thành công");
                    return Ok(new BaseResponse {Status = 1, Description = "OK"});
                }
            }
            Log.Warning("AdministrationController", $"thêm account {account.Username} thất bại");
            return Ok(new BaseResponse {Status = 0});
        }

        /// <summary>
        ///     cập nhật account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof (BaseResponse))]
        [Route("admin/UpdateUser")]
        public IHttpActionResult UpdateUser(AccountTranfer account)
        {
            if (IsAdminAccount())
            {
                ////không xài role
                //if (string.IsNullOrEmpty(account?.RoleId))
                //{
                //    Log.Warning("AdministrationController", "account truyền vào null");
                //    return Ok(new BaseResponse {Status = 0});
                //}

                //if (account.Level < (int) Account.Level)
                //    return Ok(new BaseResponse {Status = 0, Description = "Không tạo được tài khoản quyền cao hơn "});
                if (account.Level <= (int)Account.Level)
                    return Ok(new BaseResponse { Status = 0, Description = "Chỉ tạo được tài khoản quyền thấp hơn" });

                if (AdministrationManager.UpdateUser(account))
                {
                    Log.Warning("AdministrationController", $"update account {account.Username} thành công");
                    return Ok(new BaseResponse {Status = 1, Description = "OK"});
                }
            }
            Log.Warning("AdministrationController", $"update account {account.Username} thất bại");
            return Ok(new BaseResponse {Status = 0});
        }

        /// <summary>
        /// Lấy user theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(UsernameResponse))]
        [Route("admin/GetUser")]
        public IHttpActionResult GetUser(string username)
        {
            if (!IsAdminAccount()) return Ok(new BaseResponse { Status = 0, Description="Không có quyền" });

            var ret = AdministrationManager.GetUser(username);
            bool valid = Account.Level < AccountLevel.Customer;
            if(!valid)
                foreach(var com in Account.CompanyIds)
                {
                    if (ret.ContainsCompanyIds(com.CompanyId))
                    {
                        valid = true;
                        break;
                    }
                }

            if(valid)
                return Ok(new UsernameResponse()
                {
                    Status = 1,
                    Description = "OK",
                    User = ret
                });

            return Ok(new BaseResponse { Status = 0, Description = "khác công ty hoặc không tìm ra" });
        }


        /// <summary>
        ///     xóa tài khoản
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete]
        [ResponseType(typeof (BaseResponse))]
        [Route("admin/DeleteUser")]
        public IHttpActionResult DeleteUser(string username)
        {
            if (IsAdminAccount())
            {
                if (AdministrationManager.DeleteUser(username))
                {
                    Log.Warning("AdministrationController", $"xóa account {username} thành công");
                    return Ok(new BaseResponse {Status = 1, Description = "OK"});
                }
            }
            Log.Warning("AdministrationController", $"xóa account {username} thất bại");
            return Ok(new BaseResponse {Status = 0});
        }

        /// <summary>
        ///     lấy tất cả các user đang online
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (UserResponse))]
        [Route("admin/GetUserOnline")]
        public IHttpActionResult GetUserOnline()
        {
            if (IsAdminAccount())
            {
                Log.Warning("AdministrationController",
                    $"lấy thông tin {AccountManager.GetUserOnline().Count} user online ");
                return Ok(new UserResponse
                {
                    AccountTranfers = AccountManager.GetUserOnline(),
                    Status = 1,
                    Description = "OK"
                });
            }
            Log.Warning("AdministrationController", "lấy user online thất bại");
            return Ok(new UserResponse {Status = 0});
        }

        /// <summary>
        ///     thêm log tài khoản
        /// </summary>
        /// <param name="userLogTranfer"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof (BaseResponse))]
        [Route("admin/AddUserLog")]
        public IHttpActionResult AddUserLog(UserLogTranfer userLogTranfer)
        {
            if (AdministrationManager.AddUserLog(userLogTranfer))
            {
                return Ok(new BaseResponse
                {
                    Status = 1
                });
            }
            Log.Warning("AdministrationController", "thêm log user");
            return Ok(new BaseResponse {Status = 0});
        }


        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="method">Phương thức đổi Add,Edit,Delete,Setup,Get,List...(tùy chọn)</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(AccessHistoryResponse))]
        [Route("admin/GetAccessHistory")]
        public IHttpActionResult GetAccessHistory(string username, DateTime begin, DateTime end,string method ="")
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetAccessHistory(username, begin, end,method);
                if (result != null)
                {
                    Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo username: {username} thành công");
                    return Ok(new AccessHistoryResponse { Status = 1, Description = "OK", Data = result });
                }
            }
            Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo username: {username} thất bại");
            return Ok(new AccessHistoryResponse { Status = 0 });
        }


        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo cty (cty= 0 là lấy theo serial), nhóm (nhóm = 0 là lấy tất cả) hoặc theo serial nếu có nhập
        /// </summary>
        /// <param name="companyId">0 lấy theo serial nếu có nhập serial</param>
        /// <param name="groupId">0 lấy tất cả theo cty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="method">Phương thức đổi Add,Edit,Delete,Setup,Get,List...(tùy chọn)</param>
        /// <param name="serial">serial (tùy chọn)</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(AccessHistoryResponse))]
        [Route("admin/GetAccessHistoryByGroup")]
        public IHttpActionResult GetAccessHistoryByGroup(long companyId, long groupId, DateTime begin, DateTime end, string method = "", long serial = 0)
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetAccessHistory(companyId,groupId,begin, end, method,serial);
                if (result != null)
                {
                    Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo companyId : {companyId}  groupId : {groupId} thành công");
                    return Ok(new AccessHistoryResponse { Status = 1, Description = "OK", Data = result });
                }
            }
            Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo companyId : {companyId}  groupId : {groupId} thất bại");
            return Ok(new AccessHistoryResponse { Status = 0 });
        }

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo serial
        /// </summary>
        /// <param name="serial">serial</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(AccessHistoryResponse))]
        [Route("admin/GetAccessHistoryBySerial")]
        public IHttpActionResult GetAccessHistoryBySerial(long serial)
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetAccessHistoryBySerial(serial);
                if (result != null)
                {
                    Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo serial : {serial}");
                    return Ok(new AccessHistoryResponse { Status = 1, Description = "OK", Data = result });
                }
            }
            Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo serial : {serial}");
            return Ok(new AccessHistoryResponse { Status = 0 });
        }

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo giá trị bên trong nội dung
        /// </summary>
        /// <param name="content">Giá trị nội dung</param>
        [HttpGet]
        [ResponseType(typeof(AccessHistoryResponse))]
        [Route("admin/GetAccessHistoryByContent")]
        public IHttpActionResult GetAccessHistoryByContent(String content)
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetAccessHistoryByContent(content);
                if (result != null)
                {
                    Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo nội dung : {content}");
                    return Ok(new AccessHistoryResponse { Status = 1, Description = "OK", Data = result });
                }
            }
            Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo nội dung : {content}");
            return Ok(new AccessHistoryResponse { Status = 0 });
        }


        /// <summary>
        /// lấy danh sách lịch sử lap trinh thiet bi (cty= 0 là lấy theo serial), nhóm (nhóm = 0 là lấy tất cả) hoặc theo serial nếu có nhập
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="opcode">0 lat tat ca</param>
        /// <param name="companyId">0 lấy theo serial nếu có nhập serial</param>
        /// <param name="groupId">0 lấy tất cả theo cty</param>
        /// <param name="serial">serial (tùy chọn)</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(SetupDeviceHistoryResponse))]
        [Route("admin/GetSetupDeviceHistory")]
        public IHttpActionResult GetSetupDeviceHistory(DateTime begin, DateTime end, short opcode=0,long companyId=0, long groupId=0,long serial = 0)
        {
            if (IsAdminAccount())
            {
                var result = AdministrationManager.GetSetupDeviceHistory(begin, end,opcode, companyId, groupId,serial);
                if (result != null)
                {
                    Log.Warning("AdministrationController", $"lấy danh sách lịch sử cau hinh thiet bi thành công");
                    return Ok(new SetupDeviceHistoryResponse { Status = 1, Description = "OK", Data = result });
                }
            }
            Log.Warning("AdministrationController", $"lấy danh sách lịch sử truy xuất theo companyId : {companyId}  groupId : {groupId} thất bại");
            return Ok(new SetupDeviceHistoryResponse { Status = 0 });
        }


    }
}