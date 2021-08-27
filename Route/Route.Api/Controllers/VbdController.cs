#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 4:48 PM 30/10/2016
// FILENAME: VbdController.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using StarSg.Core;

namespace Route.Api.Controllers
{
    /// <summary>
    /// truy cập thông tin thông qua việt bản đồ
    /// </summary>
    public class VbdController:BaseController
    {
        /// <summary>
        /// lấy thông tin từ VBD
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Forward(VbdForwardInfo info)
        {

            var bodyData = new byte[0];
            //if (body != null)
            {
                var json = JsonConvert.SerializeObject(info.Data);
                bodyData = Encoding.UTF8.GetBytes(json);
            }

            var api = WebRequest.Create(info.Url);
            //reach (var h in _header)
            {
                api.Headers.Add("RegisterKey", "a3437a8f-e624-4d5e-a74b-64c227ca9227");
            }
            api.Method = "POST";
            api.ContentType = "application/json";
            var write = api.GetRequestStream();
            write.Write(bodyData, 0, bodyData.Length);
            write.Dispose();

            var response = api.GetResponse();

            var text = new StreamReader(response.GetResponseStream());
            var tmp = text.ReadToEnd();

            text.Dispose();
            response.Dispose();
            return Ok(JsonConvert.DeserializeObject(tmp));
        }
    }

    public class VbdForwardInfo
    {
        public string Url { get; set; }
        public object Data { get; set; }
    }
}