#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceTranfer.cs
// Time Create : 9:39 AM 24/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer
{
    /// <summary>
    ///     chứa các thông tin cần tạo của 1 thiết bị
    /// </summary>
    public class DeviceTranfer
    {
        public DeviceTranfer(long serial, string id, long companyId, long groupId, int type,
            string modelName)
        {
            Serial = serial;
            Id = id;
            CompanyId = companyId;
            GroupId = groupId;
            Type = type;
            ModelName = modelName;
        }

        public DeviceTranfer()
        {
        }

        /// <summary>
        ///     serial thiết bị
        /// </summary>
        public long Serial { set; get; }

        /// <summary>
        ///     id thiết bị
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     id công ty
        /// </summary>
        public long CompanyId { get; set; }

        /// <summary>
        ///     id đội xe
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        ///     loại hình kinh doanh
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///     loại xe
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        ///     Biển số
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        ///     Thời gian hết hạn
        /// </summary>
        public DateTime EndTime { get; set; }

        public string VinSerial { get; set; }
        
        /// <summary>
        ///     số điện thoại của thiết bị
        /// </summary>
        public string PhoneOfDevice { get; set; }
        
        

        /// <summary>
        ///     truyền tổng cục đường bộ
        /// </summary>
        public bool BgtTranport { get; set; }

        /// <summary>
        ///     sở giao thông vận tải
        /// </summary>
        public string Sgtvt { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Đảo ngược tình trạng cửa
        /// </summary>
        public bool InvertDoor { get; set; }

        /// <summary>
        /// Đảo ngược tình trạng máy lạnh
        /// </summary>
        public bool InvertAir { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Thời gian truyền lên bộ giao thông cuối cùng
        /// </summary>
        public DateTime BgtTime { get; set; }

        /// <summary>
        /// Số lần truyền lên bộ giao thông trong ngày
        /// </summary>
        public int BgtCount { get; set; }

        /// <summary>
        /// Ngày đóng phí
        /// </summary>
        public DateTime PaidFee { get; set; }

        /// <summary>
        /// Người lắp đặt
        /// </summary>
        public string Installer { get; set; }

        /// <summary>
        /// Người quản lý/bão trì
        /// </summary>
        public string Maintaincer { get; set; }


        /// <summary>
        ///     thiết lập nạp lệnh Cài Đặt mốc ngày lắp đặt và Cài đặt thông tin biển số xe xuống thiết bị 
        /// </summary>
        public bool SetSetup { get; set; } = true;

        /// <summary>
        /// bãng mẫu cây nhiên liệu
        /// </summary>
        public string FuelSheet { set; get; }

        /// <summary>
        /// Tham số bình nhiên liệu, cách nhau dấu '|', ',', ';', ' '
        /// </summary>
        public string FuelParams { get; set; }


        /// <summary>
        /// Định mức nhiên liệu chạy trong 100KM
        /// </summary>
        public float FuelQuotaKm { set; get; }

        /// <summary>
        /// Định mức nhiên liệu chạy trong 1 giờ
        /// </summary>
        public float FuelQuoteHour { set; get; }

        /// <summary>
        /// Sổ điện thoại của chủ xe
        /// </summary>
        public string OwnerPhone { get; set; }

        /// <summary>
        /// Gửi tin nhắn báo cho chủ xe
        /// </summary>
        public bool SmsAlarm { get; set; }

        /// <summary>
        /// Số lần giới hạn đồng bộ, nếu vượt quá số lần này thì gửi tin nhắn thông báo
        /// </summary>
        public int OnlineTimeout { get; set; }

        /// <summary>
        /// Gửi email báo cho chủ xe
        /// </summary>
        public bool EmailAlarm { get; set; }

        /// <summary>
        /// email của chủ xe
        /// </summary>
        public string EmailAddess { get; set; }


        /// <summary>
        /// DriverId (readonly)
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// CameraId
        /// </summary>
        public string CameraId { get; set; }

    }

}