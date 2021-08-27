using System;
using System.Collections;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class DeviceFuelLogGet:BaseResponse
    {
         public IList<DeviceFuelLogTranfer> Datas { get; set; }
    }

    public class DeviceFuelLogTranfer
    {
        public long Distance { get; set; }
        public float Fuel { get; set; }
        public DateTime Time { get; set; }
    }
}