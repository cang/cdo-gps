using System;

namespace Core.Models.Tranfer.Qc31
{
    public class GeneralCompanyReport
    {
        public string Bs { get; set; }
        public DeviceActivityType ActivityType { get; set; }
        public DateTime DateTime { get; set; }

        /// <summary>
        ///     tổng số KM chạy được
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        ///     tổng số lần dừng đỗ
        /// </summary>
        public int PauseSum { get; set; }

        /// <summary>
        ///     tổng số lần quá tốc độ
        /// </summary>
        public int OverSpeedSum { get; set; }

        /// <summary>
        ///     tổng số lần mở cửa
        /// </summary>
        public int OpenDoorSum { get; set; }

        /// <summary>
        ///     tổng số lần chạy liên tục 4h
        /// </summary>
        public int Run4hSum { get; set; }

        /// <summary>
        ///     số làn chạy quá 10h
        /// </summary>
        public int Run10hSum { get; set; }

        public int MaxSpeed { get; set; }
        public int AverageSpeed { get; set; }
        public TimeSpan PauseTimeSpan { get; set; }
        public TimeSpan RunTimeSpan { get; set; }
    }
}