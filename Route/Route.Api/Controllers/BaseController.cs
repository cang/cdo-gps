using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Description;
using Log;
using Route.Api.Core;
using Route.Api.Models;
using Route.Core;
using StarSg.Core;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     quản lý cài đặt các thông số cần thiết cho hệ thống controller
    /// </summary>
    public class BaseController : ApiController, IControllerInstall
    {
        [Import]
        protected ILog Log { set; get; }
        [Import]
        protected ICompanyRouteTable CompanyRoute { get; set; }
        [Import]
        protected IDataCenterStore DataCenterStore { get; set; }
        [Import]
        protected IDeviceRouteTable DeviceRoute { get; set; }

        [Import]
        protected Auth.Core.IAccountManager _accountManager;

        protected ForwardApi ForwardApi { get; } = new ForwardApi();

        [Import]
        protected Route.Api.Auth.Core.Loader _authloader;

        /// <summary>
        /// token giao tiếp với hệ thống
        /// </summary>
        protected string Token { get; set; }

        /// <summary>
        ///     kiểm tra quyền truy cập
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool ValidAccess(IDependencyResolver dependency, HttpRequestHeaders header)
        {
            //return true;
            // Kiểm tra token có được add vào header
            if (!header.Contains("token"))
                return false;
            Token = header.GetValues("token").FirstOrDefault();
            // Check chứng thực sang server Auth
            var loader = (Loader)dependency.GetService(typeof(Loader));
            //ConfigAuthServer = loader.Config;

            Log = (ILog)dependency.GetService(typeof(ILog));

            //return true;
            //todo: chưa làm
            //Log.Debug("API", $"{loader.Config.Ip}:{loader.Config.Port}/Auth/CheckToken?token={Token}");
            UserPermision = new UserPermissionManager(Token, _accountManager);

            return UserPermision.Check();
        }

        //protected RequestAuthConfig ConfigAuthServer { get; set; }

        protected UserPermissionManager UserPermision { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool InstallOption(IDependencyResolver dependency, HttpRequestHeaders header)
        {
            return true;
        }

        /// <summary>
        /// Thêm vào lịch sử truy xuất
        /// </summary>
        /// <param name="Serial"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        protected void AddAccessHistory(BaseResponse res,long Serial, AccessHistoryMethod method, string content)
        {
            if (res == null) return;
            if (res.Status == 0) return;
            AddAccessHistory(Serial, method, content);
        }

        /// <summary>
        /// Thêm vào lịch sử truy xuất
        /// </summary>
        /// <param name="Serial"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        protected void AddAccessHistory(long Serial, AccessHistoryMethod method, string content)
        {
            try
            {
                using (var db = _authloader.GetContext())
                {
                    long companyid = UserPermision.GetFirstCompany();
                    long groupid = UserPermision.GetUserGroupId(companyid);
                    companyid = UserPermision.GetUserCompanyId(companyid);

                    db.Insert<AccessHistory>(new AccessHistory()
                    {
                        Username = UserPermision.User.UserName,
                        AtTime = DateTime.Now,
                        CompanyId = companyid,
                        GroupId = groupid<0?0: groupid,
                        Serial = Serial,
                        Method = method.ToString(),
                        Content = content,
                   });
                }
            }
            catch (Exception e)
            {
                Log.Exception("AddAccessHistory", e, "Lỗi thêm vô lịch sử tuy xuất");
            }
        }


    }
}