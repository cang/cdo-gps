#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : IControllerInstall.cs
// Time Create : 8:28 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Net.Http.Headers;
using System.Web.Http.Dependencies;

namespace Route.Api.Auth
{
    /// <summary>
    /// quản lý inject các thông tin vào controller
    /// </summary>
    public interface IControllerInstall
    {
        /// <summary>
        /// cài đặt các service 
        /// </summary>
        /// <param name="depen"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        bool Install(IDependencyResolver depen, HttpRequestHeaders header);
    }
}