using System;

namespace StarSg.Utils.Models.Tranfer.DeviceManager
{
    public class DeviceSessionLogTranfer
    {
        public long Id { get; set; }
        public long Serial { get; set; }
        //public long GroupId { get; set; }
        public GpsPoint BeginLocation { get; set; }
        public GpsPoint EndLocation { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public long DistanceGps { get; set; }
        public TimeSpan OverTime { get; set; }
        //public float Fuel { set; get; }
        //public long DriverId { set; get; }
        public string Bs { get; set; }
    }
}