using System;

namespace StarSg.Utils.Models.Tranfer.Maintenance
{
    public class MaintenanceHistoryTranfer
    {
        public long Id { get; set; }
        public DateTime DateUpdate { get; set; }
        public string OptionNameType { get; set; }
        public long KmReset { get; set; }
    }
}