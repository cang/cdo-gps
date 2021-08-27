#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : ResponseDataConfig.cs
// Time Create : 11:26 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ConfigFile;
using DataCenter.Core;

#endregion

namespace Datacenter.Api.Models
{
    /// <summary>
    ///     chứa thông tin server sql
    /// </summary>
    [DataContract]
    public class SqlConfig
    {
        /// <summary>
        ///     địa chỉ ip or domain
        /// </summary>
        [DataMember]
        public string Ip { get; set; }

        /// <summary>
        ///     User đăng nhập
        /// </summary>
        [DataMember]
        public string User { get; set; }

        /// <summary>
        ///     Mật khẩu
        /// </summary>
        [DataMember]
        public string Pass { get; set; }

        /// <summary>
        ///     Tên database
        /// </summary>
        [DataMember]
        public string DataName { get; set; }

        /// <summary>
        ///     Id của máy chủ report
        /// </summary>
        [DataMember]
        public int Id { get; set; }
    }

    /// <summary>
    ///     thông tin cáu hình việt bản đồ
    /// </summary>
    [DataContract]
    public class GeoCodeConfig
    {
        ///// <summary>
        /////     địa chỉ VBD (Không còn sử dụng)
        ///// </summary>
        //[DataMember]
        //public string Host { get; set; }


        ///// <summary>
        /////     key VBD (Không còn sử dụng)
        ///// </summary>
        //[DataMember]
        //public string RegisterKey { get; set; }

        /// <summary>
        ///     địa chỉ của GEOSERVER
        /// </summary>
        [DataMember]
        public string GeoServerUrl { get; set; } = "http://localhost:3000";

        /// <summary>
        ///     Nếu 2 điểm nằm trong khoảng cách này thì trùng địa chỉ, đơn vị mét
        /// </summary>
        [DataMember]
        public float AddressDistance { get; set; } = 5;

    }

    /// <summary>
    ///     cấu hình của web api
    /// </summary>
    [DataContract]
    public class ResponseDataConfig : IConfigObject
    {
        /// <summary>
        ///     cấu hình của dữ liệu chính
        /// </summary>
        [DataMember]
        public SqlConfig MotherSql { get; set; }

        /// <summary>
        ///     cấu hình của dữ liệu báo cáo
        /// </summary>
        [DataMember]
        public IDictionary<int, SqlConfig> ReportSqls { get; set; }


        /// <summary>
        ///     Tên miền route để chạy SignalR
        /// </summary>
        [DataMember]
        public string RouteDomain { get; set; }

        /// <summary>
        ///     Id định danh của cụm datacenter
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Thông tin GEO
        /// </summary>
        [DataMember]
        public GeoCodeConfig GeoCode { get; set; }

        /// <summary>
        ///     Su dung foward cho xe dien
        /// </summary>
        [DataMember]
        public bool UseTramForward { get; set; }

        /// <summary>
        ///     dia chi forward goi sync
        /// </summary>
        [DataMember]
        public string TramForwardSync { get; set; }

        /// <summary>
        ///     dia chi forward goi su kien
        /// </summary>
        [DataMember]
        public string TramForwardEvent { get; set; }


        #region Implementation of IConfigObject

        /// <summary>
        ///     cấu hình các thông tin mặc định
        /// </summary>
        public void Fix()
        {
            if (ReportSqls == null)
                ReportSqls = new Dictionary<int, SqlConfig>();
            if (MotherSql == null)
            {
                MotherSql = new SqlConfig
                {
                    DataName = "SgsiData",
                    Id = 0,
                    Ip = "127.0.0.1",
                    Pass = "@123456a",
                    User = "sa"
                };
            }
            if (Id == Guid.Empty)
            {
                Id = MachineIdFactory.GetMachineId();
            }
            if (GeoCode == null)
            {
                GeoCode = new GeoCodeConfig();
                GeoCode.GeoServerUrl = "http://localhost:3000";
                GeoCode.AddressDistance = 5;
    }
        }

        #endregion

        /// <summary>
        /// gan tu LocationQuery.GetAddress
        /// </summary>
        public static string GeoServerUrl;

        /// <summary>
        /// Tên miền route để chạy SignalR
        /// </summary>
        public static string RouteDomainUrl = "http://route.sgsi.vn";


        /// <summary>
        /// Su dung foward cho xe dien
        /// </summary>
        public static bool UseTramForwardUrl = false;

        /// <summary>
        /// dia chi forward goi sync
        /// </summary>
        //public static string TramForwardSyncUrl = "http://45.119.83.35:1880/forward-sync";
        public static string TramForwardSyncUrl = "http://127.0.0.1:8001/forward-sync";

        /// <summary>
        /// dia chi forward goi su kien
        /// </summary>
        //public static string TramForwardEventUrl = "http://45.119.83.35:1880/forward-event";
        public static string TramForwardEventUrl = "http://127.0.0.1:8001/forward-event";


    }
}