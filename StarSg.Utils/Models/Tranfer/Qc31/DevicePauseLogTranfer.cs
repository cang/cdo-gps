using System;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer.Qc31
{
    public class DevicePauseLogTranfer
    {
        public long Id { get; set; }
        public long Serial { get; set; }
        public GpsPoint Point { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public long DriverId { get; set; }
       // public int Speed { get; set; }
        public string Bs { get; set; }
        public string DriverName { get; set; }
        public string Gplx { get; set; }
        public TimeSpan PauseTime { get; set; }
        public int ActivityType { get; set; }
    }
}