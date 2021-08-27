using System;

namespace Core.Models.Tranfer.SimManager
{
    public class SimTraceLogTranfer
    {
        public string Serial { get; set; }
        public DateTime TimeUpdate { get; set; }
        public string Phone { get; set; }
        public string Money { get; set; }
        public string Note { get; set; }
        public int TypeLog { get; set; }
    }
}