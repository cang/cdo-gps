namespace Datacenter.Model.Entity
{
    public enum DeviceActivityType
    {
        None=0,
        /// <summary>
        ///     100: Xe khách tuyến cố định
        /// </summary>
        Xekach = 100,

        /// <summary>
        ///     200: Vận tải hành khách bằng xe bus
        /// </summary>
        VanTaiBus = 200,

        /// <summary>
        ///     300: vận tải hành khách theo hợp đồng
        /// </summary>
        VanTaiKhach = 300,

        /// <summary>
        ///     400: Vận chuyển khách du lịch bằng ô tô
        /// </summary>
        VanChuyenKhach = 400,

        /// <summary>
        ///     500: vận tải hàng hóa bằng container
        /// </summary>
        VanChuyenHang = 500,

        /// <summary>
        ///     600 : Xe tải
        /// </summary>
        XeTai = 600,

        /// <summary>
        ///     700 : taxi
        /// </summary>
        Taxi = 700,

        /// <summary>
        ///     800 : taxi tải
        /// </summary>
        TaxiTai = 800,

        /// <summary>
        ///     900 : xe đầu Kéo
        /// </summary>
        XeDauKeo = 900
    }

    public static class DeviceActivityTypeUtil
    {
        public static bool Check(this int val)
        {
            switch (val)
            {
                case 100:
                case 200:
                case 300:
                case 400:
                case 500:
                case 600:
                case 700:
                case 800:
                case 900:
                    return true;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Biến chỉ định loại cấu hình setup
    /// </summary>
    public enum SetupCommandType : byte
    {
        None = 0,
        AutoChangeOverSpeed
    }

    /// <summary>
    /// Định nghĩa loại xe (chưa lưu db)
    /// </summary>
    public enum DeviceType : byte
    {
        None = 0,
        ConstructionVehicle,
        TaxiVehicle, //detect Guest Event 
        Dynamo, //may phat dien
        OilVehicle,//xe bon dau
        OtherVehicle = byte.MaxValue
    }

}