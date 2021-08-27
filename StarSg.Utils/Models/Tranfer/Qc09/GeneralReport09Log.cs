namespace StarSg.Utils.Models.Tranfer.Qc09
{
    public class GeneralReport09Log
    {
        public long Serial { get; set; }
        public string Bs { get; set; }
        public int Speed5To10Count { get; set; }
        public int Speed10To20Count { get; set; }
        public int Speed20To35Count { get; set; }
        public int Speed35Count { get; set; }
        /// <summary>
        /// số lần vi phạm tốc độ xe
        /// </summary>
        public int OverspeedCount { get; set; }
        /// <summary>
        /// số lần vi pham/1000 km chạy
        /// </summary>
        public double Speed1000Count { get; set; }
        /// <summary>
        /// tỷ lệ km vi phạm/km xe chạy
        /// </summary>
        public double PercentKm { get; set; }
        /// <summary>
        /// tổng số km đã chạy
        /// </summary>
        public double KmTotal { get; set; }
        public int Report4HCount { get; set; }
        public int Report10HCount { get; set; }
        /// <summary>
        /// Tỷ lệ % số ngày vi phạm so với số ngày hoạt động
        /// </summary>
        public double OverTimePercent { get; set; }
        public string Note { get; set; }
    }
}
