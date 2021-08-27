using System;
using StarSg.Utils.Models.Tranfer;

namespace Core.Models.Tranfer
{
    public class DriverSessionLogTranfer
    {
        public long Id { get; set; }

        public long DriverId { get; set; }

        public long DeviceSerial { get; set; }

        public long GroupId { get; set; }

        public long CompanyId { get; set; }

        public GpsPoint BeginLocation { get; set; }

        public GpsPoint EndLocation { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Note { get; set; }
        public string Bs { get; set; }
        public string DriverName { get; set; }
        public string Gplx { get; set; }
        public int ActivityType { get; set; }
        public int OverTime { get; set; }
        /// <summary>
        /// ngày vi phạm
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}