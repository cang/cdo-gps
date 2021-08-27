using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Route.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cros = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cros);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }
                );
            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
               Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new StringEnumConverter());
        }
    }
}
