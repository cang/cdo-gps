namespace Core.Utils
{
    public class DeviceStatus
    {
        /// <summary>
        ///     Chìa khóa
        /// </summary>
        [BitField(Index = 0)]
        public bool IsOpenKey { get; set; }

        /// <summary>
        ///     Máy lạnh
        /// </summary>
        [BitField(Index = 1)]
        public bool IsRunAirconditioner { get; set; }

        /// <summary>
        ///     Chích Xung
        /// </summary>
        [BitField(Index = 2)]
        public bool IsValidClockSignal { get; set; }

        /// <summary>
        ///     Mở cửa
        /// </summary>
        [BitField(Index = 3)]
        public bool IsOpenDoor { get; set; }

        /// <summary>
        ///     Có khách
        /// </summary>
        [BitField(Index = 4)]
        public bool NotWorking { get; set; }

        /// <summary>
        ///     Mất nguồn
        /// </summary>
        [BitField(Index = 5)]
        public bool IsLockTaxiMeter { get; set; }

        /// <summary>
        ///     Lập trình
        /// </summary>
        [BitField(Index = 6)]
        public bool IsSettup { get; set; }

        /// <summary>
        ///     SOS
        /// </summary>
        [BitField(Index = 7)]
        public bool IsSOS { get; set; }

        /// <summary>
        ///     Lấy vị trí
        /// </summary>
        [BitField(Index = 8)]
        public bool IsGetLocaltion { get; set; }

        /// <summary>
        ///     Bận
        /// </summary>
        [BitField(Index = 9)]
        public bool IsBusy { get; set; }

        public int Val { set; get; }
    }
}