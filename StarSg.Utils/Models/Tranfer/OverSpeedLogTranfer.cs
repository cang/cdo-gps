using System;

namespace StarSg.Utils.Models.Tranfer
{
    public class OverSpeedLogTranfer
    {
        public long Id { get; set; }
        public long Serial { get; set; }
        public long GroupId { get; set; }
        public long DriverId { get; set; }
        public long CompanyId { get; set; }
        public int LimitSpeed { get; set; }
        public int MaxSpeed { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public GpsPoint Point { get; set; }
        public string Bs { get; set; }
        public string DriverName { get; set; }
        public string Gplx { get; set; }
        public int Type { get; set; }
    }
}