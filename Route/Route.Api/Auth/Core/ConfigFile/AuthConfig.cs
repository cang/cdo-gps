#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : AuthConfig.cs
// Time Create : 9:30 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Runtime.Serialization;
using ConfigFile;

namespace Route.Api.Auth.Core.ConfigFile
{
    /// <summary>
    /// thông tin cấu hình của hệ thống
    /// </summary>
    [DataContract]
    public class AuthConfig:IConfigObject
    {
        /// <summary>
        /// ip của máy chủ sql
        /// </summary>
        [DataMember]
        public string DbIp { get; set; }
        /// <summary>
        /// tên cơ sở dữ liệu
        /// </summary>
        [DataMember]
        public string DbName { get; set; }
        /// <summary>
        /// username
        /// </summary>
        [DataMember]
        public string DbUser { get; set; }
        /// <summary>
        /// mật khẩu
        /// </summary>
        [DataMember]
        public string DbPass { get; set; }

        [DataMember]
        public string GeoServer { get; set; }

        [DataMember]
        public string RouteDomain { get; set; }

        #region Implementation of IConfigObject

        /// <summary>
        ///     cấu hình các thông tin mặc định
        /// </summary>
        public void Fix()
        {
            if (string.IsNullOrEmpty(DbIp))
                DbIp = "127.0.0.1";
            if (string.IsNullOrEmpty(DbName))
                DbName = "SgsiAccount";
            if (string.IsNullOrEmpty(DbUser))
                DbUser = "sa";
            if (string.IsNullOrEmpty(DbPass))
                DbPass = "@123456a";
        }

        #endregion
        
        public static string RouteDomainUrl = "http://route.sgsi.vn";
        public static string GeoServerUrl = "http://127.0.0.1:3000";

    }
}