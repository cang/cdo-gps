using System;

namespace StarSg.Utils.Models.Tranfer.DeviceManager
{
    public class DeviceTraceLogTranfer
    {
        public long Id { get; set; }
        public long Serial { get; set; }
        public GpsPoint BeginLocation { get; set; }
        public GpsPoint EndLocation { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan OverTime { get; set; }
        public int DeviceStatusType { get; set; }
        public string Bs { get; set; }
        public int ActivityType { get; set; }
        public string Note { get; set; }
    }
}