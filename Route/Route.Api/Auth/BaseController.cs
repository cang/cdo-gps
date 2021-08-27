#region header

// /*********************************************************************************************/
// Project :Authentication
// FileName : BaseController.cs
// Time Create : 3:46 PM 14/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Description;
using Log;
using Route.Api.Auth.Core;

namespace Route.Api.Auth
{
    /// <summary>
    /// </summary>
    public class BaseController : ApiController, IControllerInstall
    {
        /// <summary>
        ///     log
        /// </summary>
        protected ILog Log { set; get; }

        /// <summary>
        ///     quản lý thông tin tài khoản đăng nhập
        /// </summary>
        protected IAccountManager AccountManager { get; private set; }

        #region Implementation of IControllerInstall

        /// <summary>
        ///     cài đặt các service
        /// </summary>
        /// <param name="depen"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual bool Install(IDependencyResolver depen, HttpRequestHeaders header)
        {
            AccountManager = (IAccountManager) depen.GetService(typeof (IAccountManager));
            Log = (ILog) depen.GetService(typeof (ILog));
            return true;
        }

        #endregion
    }
}