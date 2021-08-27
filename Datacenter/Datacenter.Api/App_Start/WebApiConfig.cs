#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : WebApiConfig.cs
// Time Create : 1:38 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#endregion

namespace Datacenter.Api
{
    /// <summary>
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Web API configuration and services
            var cros = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cros);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
                );

            //config.EnableSystemDiagnosticsTracing();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
                Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new StringEnumConverter());
        }
    }
}