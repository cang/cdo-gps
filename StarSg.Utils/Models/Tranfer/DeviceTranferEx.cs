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
using System.Collections.Generic;

namespace StarSg.Utils.Models.Tranfer
{
    /// <summary>
    ///     chứa các thông tin mở rộng của 1 thiết bị ( dùng để xem thông tin)
    /// </summary>
    public class DeviceTranferEx : DeviceTranfer
    {
        public DeviceTranferEx()
        {
        }

        /// <summary>
        /// Version phần mềm
        /// </summary>
        public string FirmWareVersion { get; set; }

        /// <summary>
        /// Version phần cứng
        /// </summary>
        public string HardWareVersion { get; set; }

        /// <summary>
        /// Tên công ty
        /// </summary>
        public string CompanyName { get; set; }


        /// <summary>
        /// Tên đội xe
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Danh sách User sử dụng
        /// </summary>
        public IList<String> Users { get; set; } = new List<String>(0);

        /// <summary>
        /// Danh sách User trực tiếp sử dụng (tên hiển thị)
        /// </summary>
        public IList<String> DirectlyUsers { get; set; } = new List<String>(0);

        /// <summary>
        /// Danh sách User trực tiếp sử dụng (username)
        /// </summary>
        public IList<String> DirectlyUsernames { get; set; }

    }


    /// <summary>
    /// Thông tin giả lặp
    /// </summary>
    public class DeviceTranferFake
    {
        /// <summary>
        /// Serial (bắt buột)
        /// </summary>
        public long Serial { get; set; }
        //public byte[] OriginalData { get; set; }

        /// <summary>
        /// Thời gian phát (bắt buột)
        /// </summary>
        public DateTime Time { get; set; }
        public bool GpsStatus { get; set; }


        /// <summary>
        /// Vị trí lan (bắt buột)
        /// </summary>
        public float GpsInfoLat { get; set; }

        /// <summary>
        /// Vị trí lat (bắt buột)
        /// </summary>
        public float GpsInfoLng { get; set; }

        public byte  GpsInfoSpeed { get; set; }

        //public long TotalGpsDistance { get; set; }
        //public long TotalCurrentGpsDistance { get; set; }

        /// <summary>
        /// Tình trạng khóa xe (bắt buột)
        /// </summary>
        public bool StatusIoKey { get; set; }

        public bool StatusIoAirMachine { get; set; }
        public bool StatusIoSos { get; set; }
        public bool StatusIoUseTemperature { get; set; }
        public bool StatusIoUseFuel { get; set; }
        public bool StatusIoUseRfid { get; set; }
        public bool StatusIoDoor { get; set; }

        public int IOValue { get; set; }
        public int DriverId { get; set; }
        public int Fuel { get; set; }
        public short Temperature { get; set; }
        public byte GsmSignal { get; set; }
        public byte Power { get; set; }
        public IList<byte> SpeedLogs { get; set; } = new List<byte>();
        public short TimeWork { get; set; }
        public short TimeWorkInDay { get; set; }
    }

}