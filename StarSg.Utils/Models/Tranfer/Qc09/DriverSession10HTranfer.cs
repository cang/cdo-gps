using System;

namespace StarSg.Utils.Models.Tranfer.Qc09
{
    public class DriverSession10HTranfer
    {
        public long DriverId { get; set; }
        public string DriverName { get; set; }
        public string Gplx { get; set; }
        public long DeviceSerial { get; set; }

        public long CompanyId { get; set; }
        public GpsPoint BeginLocation { get; set; }

        public GpsPoint EndLocation { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// thời gian vi phạm ( phút )
        /// </summary>
        public int OverTime { get; set; }

        /// <summary>
        /// ngày vi phạm
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Bảng số xe
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        /// Loại hình KD
        /// </summary>
        public int ActivityType { get; set; }

    }
}
