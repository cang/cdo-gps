using System;
using System.Collections.Generic;

namespace StarSg.Utils.Models.DatacenterResponse.Setup
{
    public class DeviceSettingInfo
    {
        public long Serial { get; set; }
        public DateTime TimeUpdate { get; set; }
        public int TimeSync { get; set; }
        public short OverTimeInSession { get; set; }
        public short OverTimeInDay { get; set; }
        public byte OverSpeed { get; set; }
        public IList<string> PhoneSystemControl { get; set; } = new List<string>();
        public string FirmWareVersion { get; set; }
        public string HardWareVersion { get; set; }
    }
}