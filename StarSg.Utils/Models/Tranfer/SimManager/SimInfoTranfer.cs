using System;
using System.Runtime.Serialization;

namespace Core.Models.Tranfer.SimManager
{
    [DataContract]
    public class SimInfoTranfer
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Serial { get; set; }

        [DataMember]
        public string Bs { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public DateTime PhoneUpdate { get; set; }

        [DataMember]
        public string Money { get; set; }

        [DataMember]
        public DateTime MoneyUpdate { get; set; }
    }
}