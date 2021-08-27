using System;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer.Qc09
{
    public class OverSpeedReport09Tranfer
    {
        public long Serial { get; set; }
        public long CompanyId { get; set; }
        public string Bs { get; set; }
        public int ActivityType { get; set; }
        public int Speed5To10Count { get; set; }
        public int Speed10To20Count { get; set; }
        public int Speed20To35Count { get; set; }
        public int Speed35Count { get; set; }
        /// <summary>
        /// số lần vi phạm
        /// </summary>
        public int OverspeedCount { get; set; }
        public double Speed1000Count { get; set; }
        /// <summary>
        /// số km chạy quá tốc độ
        /// </summary>
        public double KmOverspeed { get; set; }
        /// <summary>
        /// tổng số km đã chạy
        /// </summary>
        public double KmTotal { get; set; }
        public double PercentKm { get; set; }
        public TimeSpan TimeOverspeed { get; set; }
        public TimeSpan TimeTotal { get; set; }
        public double PercentTime { get; set; }
    }
}
