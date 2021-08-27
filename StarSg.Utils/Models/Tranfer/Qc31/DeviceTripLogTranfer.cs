using System;
using System.Runtime.Serialization;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer.Qc31
{
    [DataContract]
    public class DeviceTripLogTranfer
    {
        [DataMember]
        public long Serial { get; set; }
        [DataMember]
        public string Bs { get; set; }
        [DataMember]
        public DateTime TimeUpdate { get; set; }

        [DataMember]
        public GpsPoint Location { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public String Note { get; set; }
    }
}