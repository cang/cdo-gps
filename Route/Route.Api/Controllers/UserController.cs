using Route.Api.Auth;
using Route.Api.Auth.Core;
using Route.Api.Auth.Models.Req;
using Route.Api.Auth.Models.Response;
using StarSg.Core;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     xử lý các thông tin của người dùng
    /// </summary>
    [ValidRequestFilter]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserController : ValidController
    {
        /// <summary>
        ///     đăng xuất hệ thống
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/LogOut")]
        [ResponseType(typeof (BaseResponse))]
        public IHttpActionResult LogOut()
        {
            if (AccountManager.Logout(Token))
            {
                return Ok(new BaseResponse {Status = 1, Description = "OK"});
            }
            return Ok(new BaseResponse {Status = 0});
        }


        /// <summary>
        ///     đổi mật khẩu
        ///     thôi
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof (BaseResponse))]
        [Route("User/ChangePass")]
        public IHttpActionResult ChangePass(string password)
        {
            if (Account != null)
            {
                if (AccountManager.ChangePassword(Account.Username, password))
                {
                    Log.Warning("AdministrationController", $"đổi mật khẩu: {Account.Username} thành công");
                    AccountManager.ForceLogout(Account.Username);
                    return Ok(new BaseResponse {Status = 1, Description = "OK"});
                }
            }
            Log.Warning("AdministrationController", $"đổi mật khẩu thất bại");
            return Ok(new BaseResponse {Status = 0});
        }

        /// <summary>
        /// UpdateUser
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(BaseResponse))]
        [Route("User/Update")]
        public IHttpActionResult Update(AccountInfo account)
        {
            if (Account != null)
            {
                if (AccountManager.Update(Account.Username, account))
                {
                    Log.Warning("AdministrationController", $"cập nhật : {Account.Username} thành công");
                    AccountManager.ForceLogout(Account.Username);
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("AdministrationController", $"cập nhật thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }


        /// <summary>
        /// lấy thông tin chức năng mà tài khoản dc quản lý
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("User/GetRole")]
        [ResponseType(typeof(RoleResponse))]
        public IHttpActionResult GetRole()
        {
            return Ok(new RoleResponse
            {
                Status = 0,
                Description = "OK",
                Funcs = Account.Role?.Functions.Select(m => m.Fun).ToList(),
                Level = (int)Account.Level,
                Role=Account.GroupUserId,
                Username=Account.Username
            });
        }
    }


}