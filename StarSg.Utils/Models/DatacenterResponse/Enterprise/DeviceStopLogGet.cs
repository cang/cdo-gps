using System;
using System.Collections;
using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{

    public class DeviceStopLogGet:BaseResponse
    {
         public IList<DeviceStopTranfer> Datas { get; set; }
    }

    public class DeviceStopTranfer
    {
        public string Bs { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { set; get; }
        public GpsPoint Location { get; set; }
    }
}