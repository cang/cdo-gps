using System;
using System.Collections;
using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class DeviceMachineGet:BaseResponse
    {
         public IList<DeviceMachineLogTranfer> Datas { get; set; }
    }

    public class DeviceMachineLogTranfer
    {
        public long Serial { get; set; }
        public string Bs { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public GpsPoint BeginLocation { get; set; }
        public GpsPoint EndLocation { get; set; }
        public TimeSpan Over { set; get; }
    }
}