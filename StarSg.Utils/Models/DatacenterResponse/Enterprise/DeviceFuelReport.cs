#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: StarSg.Utils
// TIME CREATE : 6:25 PM 30/10/2016
// FILENAME: DeviceFuelReport.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class DeviceFuelView
    {
        public    string Serial { get; set; }
        public    string Bs { get; set; }
         public int KmChayDuoc { get; set; }
         public TimeSpan ThoiGianNoMay { get; set; }
        public int NhienLieuBanDau { get; set; }
        public int NhienLieuTieuThu { get; set; }
        public int NhienLieuThatThoat { get; set; }
        public int NhienLieuThemVao { get; set; }
        public int NhienLieuConLai { get; set; }
        public float NhienLieuTrungBinh { get; set; }
        public string Note { get; set; }
    }

    public class DeviceFuelReportSingle : BaseResponse
    {
        public DeviceFuelView Data { get; set; }
    }
    public class DeviceFuelReportMulti : BaseResponse
    {
        public IList<DeviceFuelView> Data { get; set; }=new List<DeviceFuelView>();
    }
}