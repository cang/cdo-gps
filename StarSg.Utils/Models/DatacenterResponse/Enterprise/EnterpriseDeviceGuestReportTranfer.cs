using System;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class EnterpriseDeviceGuestReportTranfer
    {
        /// <summary>
        /// Serial
        /// </summary>
        public long Serial { get; set; }

        /// <summary>
        /// Ngày
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Bảng số xe
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        /// Mã Tài xế
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Tài xế
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// DT Tài xế
        /// </summary>
        public string DriverPhone { get; set; }

        /// <summary>
        /// Khoảng cách chạy trong ngày có khách
        /// </summary>
        public double HasGuestKm { get; set; }

        /// <summary>
        /// Khoảng cách chạy trong ngày không khách
        /// </summary>
        public double NoGuestKm { get; set; }

        /// <summary>
        /// Thời gian chạy trong ngày (phút) có khách
        /// </summary>
        public int HasGuestMinutes { get; set; }

        /// <summary>
        /// Thời gian chạy trong ngày (phút) không khách
        /// </summary>
        public int NoGuestMinutes { get; set; }

        //Ghi chú
        public string   Note { get; set; }

        //Có cuốc đặc biệt
        public bool     HasSpecial { get; set; }

        //Ghi chú cuốc đặc biệt
        public string   SpecialNote { get; set; }

    }

}