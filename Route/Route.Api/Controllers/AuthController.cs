using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Authentication.Models.Response;
using Route.Api.Auth.Core;
using Route.Api.Auth.Models.Req;
using Route.Api.Auth.Models.Response;
using StarSg.Utils.Models.Auth;
using StarSg.Core;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý các thông tin đăng nhập
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ValidRequestFilter]
    public class AuthController : Route.Api.Auth.BaseController
    {
        /// <summary>
        ///     đăng nhập
        /// </summary>
        /// <param name="loginReq"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(TokenResponse))]
        [Route("Auth/Login")]
        public IHttpActionResult Login(LoginTranfer loginReq)
        {
            if (loginReq == null) return Ok(new TokenResponse { Status = 0 });
            var token = AccountManager.Login(loginReq.Username, loginReq.Pwd);
            if (string.IsNullOrEmpty(token))
            {
                Log.Warning("AuthController", $"đang nhập tài khoản: {loginReq.Username} thất bại");
                return Ok(new TokenResponse { Status = 0 });
            }
            Log.Warning("AuthController", $"đang nhập tài khoản: {loginReq.Username} thành công");
            return Ok(new TokenResponse { Status = 1, Token = token, Description = "OK" });
        }


        /// <summary>
        ///     check token hợp lệ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(BaseResponse))]
        [Route("Auth/CheckToken")]
        public IHttpActionResult CheckToken(string token)
        {
            if (AccountManager.ValidToken(token))
            {
                Log.Warning("AuthController", $"check token: {token} hợp lệ");
                return Ok(new BaseResponse { Status = 1, Description = "OK" });
            }
            Log.Warning("AuthController", $"check token: {token} không hợp lệ");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        /// lấy thông tin tài khoản theo token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(bool))]
        [Route("Auth/GetUserInfo")]
        public IHttpActionResult GetUserInfo(string token)
        {
            if (!AccountManager.ValidToken(token))
            {
                return Ok(new UserInfoTranfer { IsValid = false });
            }
            var user = AccountManager.GetUser(token);
            var firstCompany = user.CompanyIds?.FirstOrDefault();

            return Ok(new UserInfoTranfer
            {
                UserName = user.Username,
                CompanyId = user.CompanyIds?.Select(m => m.CompanyId).ToList(),
                Level = (int)user.Level,
                DeviceSerial = user.DeviceIds?.Select(m => m.Serial).ToList(),
                IsValid = true,
                BranchCode = AccountManager.GetBranchOfCompany(firstCompany?.CompanyId ?? 0)?.BranchCode,
                DisplayName = user.DisplayName,
                Phone = user.Phone
            });

        }
    }
}