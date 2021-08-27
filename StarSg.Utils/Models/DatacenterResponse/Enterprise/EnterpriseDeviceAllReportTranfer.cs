using System;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class EnterpriseDeviceAllReportTranfer
    {
        public DateTime Date { get; set; }
        public string Bs { get; set; }
        public int TimeRun { get; set; }
        public int TimeStop { get; set; }
        public int PauseCount { get; set; }
        public double Distance { get; set; }
        public int OnAirMachineCount { get; set; }

        /// <summary>
        /// Số lần mở cửa
        /// </summary>
        public int DoorCount { get; set; }

        /// <summary>
        ///     loại hình kinh doanh
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Định mức nhiên liệu chạy trong 100KM
        /// </summary>
        public float FuelQuotaKm { set; get; }

        /// <summary>
        /// Định mức nhiên liệu chạy trong 1 giờ
        /// </summary>
        public float FuelQuoteHour { set; get; }

        /// <summary>
        /// Nhiên liệu tiêu thụ trong 100KM
        /// </summary>
        public float ValueQuotaKm { get; set; }

        /// <summary>
        /// Nhiên liệu tiêu thụ trong 1 giờ
        /// </summary>
        public float ValueQuoteHour { get; set; }

    }
}