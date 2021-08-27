#region header

// /*********************************************************************************************/
// Project :Authentication
// FileName : ValidController.cs
// Time Create : 8:27 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http.Dependencies;
using System.Web.Http.Description;
using Route.Api.Auth.Core;
using System.Net.Http;

namespace Route.Api.Auth
{
    /// <summary>
    /// </summary>
    public class ValidController : BaseController
    {
        /// <summary>
        ///     thông tin tài khoản
        /// </summary>
        protected Models.Entity.Account Account { get; set; }

        /// <summary>
        ///     quản trị tài khoản
        /// </summary>
        protected IAdministrationManager AdministrationManager { get; private set; }

        /// <summary>
        ///     quản lý phân quyền truy cập
        /// </summary>
        protected IPermissionManager PermissionManager { get; private set; }

        /// <summary>
        ///     token
        /// </summary>
        public string Token { get; set; }

        #region Overrides of BaseController

        /// <summary>
        ///     cài đặt các service
        /// </summary>
        /// <param name="depen"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public override bool Install(IDependencyResolver depen, HttpRequestHeaders header)
        {
            if (!base.Install(depen, header))
                return false;

            //Log.Info("ValidController",$"Path ={ Request.RequestUri.GetComponents(System.UriComponents.Path,System.UriFormat.Unescaped) }");
            //Log.Info("ValidController", $"Query ={ Request.RequestUri.Query }");

            //ignore for GET Branch and Branch/All
            if (Request.Method == HttpMethod.Get)
            {
                string path = Request.RequestUri.GetComponents(System.UriComponents.Path, System.UriFormat.Unescaped);
                if (
                    "Branch/All".Equals(path) 
                    || "Branch".Equals(path)
                    )
                    return true;
            }

            if (!header.Contains("token")) return false; // header bắt buộc phải có token 

            var token = header.GetValues("token").FirstOrDefault();
            Account = AccountManager.GetUser(token);
            Token = token;
            if (Account == null) return false;
            AdministrationManager = (IAdministrationManager) depen.GetService(typeof (IAdministrationManager));
            PermissionManager = (IPermissionManager) depen.GetService(typeof (IPermissionManager));
            return true;
        }

        #endregion
    }
}