using System;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class EnterpriseDeviceFuelReportTranfer
    {
        /// <summary>
        /// Ngày
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Bảng số xe
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        /// Khoảng cách chạy trong ngày
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Thời gian chạy trong ngày (phút)
        /// </summary>
        public int TimeRun { get; set; }

        ///// <summary>
        ///// Thời gian dừng
        ///// </summary>
        //public int TimeStop { get; set; }

        /// <summary>
        /// Nhiên liệu đầu ngày
        /// </summary>
        public float BeginDateValue { get; set; }

        /// <summary>
        /// Nhiên liệu sử dụng
        /// </summary>
        public float ConsumeValue { get; set; }

        /// <summary>
        /// Nhiên liệu thất thoát
        /// </summary>
        public float LostValue { get; set; }

        /// <summary>
        /// Nhiên liệu thêm vào
        /// </summary>
        public float AddValue { get; set; }

        /// <summary>
        /// Nhiên liệu còn lại
        /// </summary>
        public float RemainValue { get; set; }

        /// <summary>
        /// Nhiên liệu trung bình 100KM
        /// </summary>
        public float Value100KM { get; set; }

        /// <summary>
        /// Nhiên liệu trung bình một giờ
        /// </summary>
        public float ValueHour { get; set; }
    }

}