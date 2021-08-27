#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : Global.asax.cs
// Time Create : 1:38 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Web;
using System.Web.Http;

#endregion

namespace Datacenter.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            MefConfig.Register();
        }
    }
}