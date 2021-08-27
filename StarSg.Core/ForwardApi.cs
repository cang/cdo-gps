#region header

// /*********************************************************************************************/
// Project :Core
// FileName : ForwardApi.cs
// Time Create : 9:39 AM 29/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace StarSg.Core
{
    public class ForwardApi : IForwardApi
    {
        private readonly IDictionary<string, string> _header = new Dictionary<string, string>();

        public void AddHeader(string key, string value)
        {
            if (!_header.ContainsKey(key))
                _header.Add(key, value);
        }

        private T Forward<T>(string method, string url, object body)
        {
            var bodyData = new byte[0];
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                bodyData = Encoding.UTF8.GetBytes(json);
            }

            var api = WebRequest.Create(url);
            foreach (var h in _header)
            {
                api.Headers.Add(h.Key, h.Value);
            }
            api.Method = method;
            api.ContentType = "application/json";
            using (var write = api.GetRequestStream())
            {
                write.Write(bodyData, 0, bodyData.Length);
            }
            using (var response = api.GetResponse())
            {
                using (var text = new StreamReader(response.GetResponseStream()))
                {
                    // todo: coi lai 
                    //var serializer = new DataContractJsonSerializer(typeof (T));
                    //var read = response.GetResponseStream();
                    //var result = serializer.ReadObject(read);
                    //read.Dispose();
                    var tmp = text.ReadToEnd();
                    var result = JsonConvert.DeserializeObject<T>(tmp);
                    return (T)result;
                }
            }
        }

        private string ForwardWithoutResult<T>(string method, string url, object body)
        {
            var bodyData = new byte[0];
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                bodyData = Encoding.UTF8.GetBytes(json);
            }

            var api = WebRequest.Create(url);
            foreach (var h in _header)
            {
                api.Headers.Add(h.Key, h.Value);
            }
            api.Method = method;
            api.ContentType = "application/json";
            using (var write = api.GetRequestStream())
            {
                write.Write(bodyData, 0, bodyData.Length);
            }
            using (var response = api.GetResponse())
            {
                using (var text = new StreamReader(response.GetResponseStream()))
                {
                    return text.ReadToEnd();
                }
            }
        }

        #region Implementation of IForwardApi

        public T Post<T>(string url, object body = null)
        {
            return Forward<T>("POST", url, body);
        }

        public string PostWithoutResult<T>(string url, object body = null)
        {
            return ForwardWithoutResult<T>("POST", url, body);
        }

        public T Put<T>(string url, object body = null)
        {
            return Forward<T>("PUT", url, body);
        }

        public T Del<T>(string url, object body = null)
        {
            return Forward<T>("DELETE", url, body);
        }

        public T Get<T>(string url)
        {
            return Get<T>(url, -1);
        }

        public T Get<T>(string url, int msTimeout)
        {
            var api = WebRequest.Create(url);
            if(msTimeout>=0)
                api.Timeout = msTimeout;//default 100000
            foreach (var h in _header)
            {
                api.Headers.Add(h.Key, h.Value);
            }
            api.Method = "GET";


            using (var response = api.GetResponse())
            {
                using (var text = new StreamReader(response.GetResponseStream()))
                {
                    //var serializer = new DataContractJsonSerializer(typeof(T));
                    //var read = response.GetResponseStream();
                    //var result = serializer.ReadObject(read);
                    //read.Dispose();
                    //var serializer = new DataContractSerializer(typeof(T));
                    var tmp = text.ReadToEnd();
                    var result = JsonConvert.DeserializeObject<T>(tmp);
                    //var result = serializer.ReadObject(response.GetResponseStream());
                    return result;
                }
            }
        }

        public T Get<T>(string url, object body)
        {
            //chua dinh nghia
            return default(T);
        }

        #endregion
    }
}