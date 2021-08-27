namespace Core.Models.Tranfer.DeviceManager
{
    public class DeviceTraceSheetTranfer
    {
        /// <summary>
        ///     số tài
        /// </summary>
        public string Id { get; set; }

        public string Serial { get; set; }
        public string Bs { get; set; }

        /// <summary>
        ///     số lần mất tín hiệu gps
        /// </summary>
        public int LostGpsCount { get; set; }

        /// <summary>
        ///     số lần mất tín hiệu gsm
        /// </summary>
        public int LostGsmCount { get; set; }

        /// <summary>
        ///     số lần quá tốc độ
        /// </summary>
        public int OverSpeed { get; set; }

        public int LostMemoryPlugCount { get; set; }
        public int PauseCount { get; set; }
    }
}