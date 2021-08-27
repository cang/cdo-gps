using System;
using System.Collections;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class DeviceTemperLogGet:BaseResponse
    {
         public IList<DeviceTemperLogTranfer> Datas { get; set; }
    }

    public class DeviceTemperLogTranfer
    {
        public long Distance { get; set; }
        public int Temper { get; set; }
        public DateTime Time { get; set; }
    }
}