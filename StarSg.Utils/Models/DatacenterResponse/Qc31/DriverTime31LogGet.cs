using System;
using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Qc31
{
    public class DriverTime31LogGet:BaseResponse
    {
         public IList<DriverTime31Tranfer> Datas { get; set; } 
    }

    public class DriverTime31Tranfer
    {
        public string Bs { get; set; }
        public string Name { get; set; }
        public string Gplx { get; set; }
        public int ActivityType { get; set; }
        public DateTime BeginTime { get; set; }
        public GpsPoint BeginLocation { get; set; }
        public DateTime EndTime { get; set; }
        public GpsPoint EndLocation { get; set; }
        public string Over { get; set; }
    }
}