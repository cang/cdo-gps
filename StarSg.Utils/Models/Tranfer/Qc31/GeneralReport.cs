using Core.Models;

namespace StarSg.Utils.Models.Tranfer.Qc31
{
    public class GeneralReport
    {
        /// <summary>
        ///     biển số xe
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        ///     loại hình kinh doanh
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        ///     tổng số KM chạy được
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ 5-10
        /// </summary>
        public int Speed5To10 { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ 10-20
        /// </summary>
        public int Speed10To20 { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ 20-35
        /// </summary>
        public int Speed20To35 { get; set; }

        /// <summary>
        ///     số lần chạy quá tốc độ từ >35
        /// </summary>
        public int Speed35 { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ 5-10
        /// </summary>
        public double Speed5To10Percent { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ 10-20
        /// </summary>
        public double Speed10To20Percent { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ 20-35
        /// </summary>
        public double Speed20To35Percent { get; set; }

        /// <summary>
        ///     phần trăm số lần chạy quá tốc độ từ >35
        /// </summary>
        public double Speed35Percent { get; set; }

        /// <summary>
        ///     tổng số lần dừng đỗ
        /// </summary>
        public int PauseSum { get; set; }

        /// <summary>
        ///     ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}