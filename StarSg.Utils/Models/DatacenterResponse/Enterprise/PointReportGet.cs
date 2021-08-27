using System;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class AreaReportGet:BaseResponse
    {
        public List<AreaReportTranfer> Datas { get; set; } 
    }

    public class PointReportGet : BaseResponse
    {
        public List<PointReportTranfer> Datas { get; set; }
    }

    public class AreaReportTranfer
    {
        public string Name { set; get; }
        public int MaxSpeed { get; set; }
        public int MaxDevice { get; set; }
        public string Points { get; set; }
        public DateTime CreateTime { get; set; }
        public string Type { get; set; }
        public int DeviceCount { get; set; }
    }

    public class PointReportTranfer
    {
        public string Name { set; get; }
        public DateTime CreateTime { get; set; }
        public string Type { get; set; }

        public int DeviceCount { get; set; }

        public int Radius { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Address { get; set; }
    }
}