#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceStatusTranfer.cs
// Time Create : 2:49 PM 16/05/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Runtime.Serialization;
using StarSg.Utils.Models.Tranfer;

namespace Core.Models.Tranfer
{
    /// <summary>
    ///     thông tin trạng thái của thiết bị
    /// </summary>
    [DataContract]
    public class DeviceStatusTranfer
    {
        [DataMember(Name = "Serial")]
        public string Serial { get; set; }

        [DataMember(Name = "Id")]
        public string Id { get; set; }

        [DataMember(Name = "Bs")]
        public string Bs { get; set; }

        [DataMember(Name = "lostgsm")]
        public bool LostGsm { get; set; }

        [DataMember(Name = "Angle")]
        public int Angle { set; get; }

        [DataMember(Name = "Location")]
        public GpsPoint Location { get; set; }

        [DataMember(Name = "mucSong")]
        public int Gsm { get; set; }

        [DataMember(Name = "mucXang")]
        public float Fuel { get; set; }

        [DataMember(Name = "speed")]
        public float Speed { set; get; }

        [DataMember(Name = "dienAp")]
        public float Power { get; set; }

        [DataMember(Name = "nhietDo")]
        public int Temper { get; set; }

        [DataMember(Name = "statusGps")]
        public bool Gps { get; set; }

        [DataMember(Name = "cuaXe")]
        public bool OpenDoor { get; set; }

        [DataMember(Name = "dungDo")]
        public bool Dungdo { get; set; }

        [DataMember(Name = "trangThaiMay")]
        public bool Key { get; set; }

        [DataMember(Name = "trangThaiMayLanh")]
        public bool AirMachine { get; set; }

        [DataMember(Name = "Gplx")]
        public string Gplx { get; set; }

        [DataMember(Name = "nameDriver")]
        public string Name { get; set; }

        [DataMember(Name = "tglxtn")]
        public int TimeDriverOnDay { set; get; }

        [DataMember(Name = "tglxlt")]
        public int TimeDriver { set; get; }

        [DataMember(Name = "qtgtn")]
        public int OverTimeOnday { set; get; }

        [DataMember(Name = "kmCuoc")]
        public int KmOnday { get; set; }

        [DataMember(Name = "timeUpdate")]
        public DateTime TimeUpdate { get; set; }

        [DataMember(Name = "isOverSpeed")]
        public bool OverSpeed { get; set; }

        [DataMember(Name = "soGtvt")]
        public string Sgtvt { get; set; }

        /// <summary>
        ///     trọng tải / số ghế
        /// </summary>
        [DataMember(Name = "sheat")]
        public string Sheats { get; set; }

        /// <summary>
        ///     trạng thái thẻ nhớ
        /// </summary>
        [DataMember(Name = "plusmemory")]
        public bool PlusMemory { get; set; }

        /// <summary>
        ///     loại xe
        /// </summary>
        [DataMember(Name = "modeltype")]
        public string ModelType { get; set; }

        /// <summary>
        ///     số điện thoại
        /// </summary>
        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        /// <summary>
        ///     số tiên trong sim
        /// </summary>
        [DataMember(Name = "money")]
        public string Money { get; set; }

        /// <summary>
        ///     mã vin
        /// </summary>
        [DataMember(Name = "vin")]
        public string Vin { get; set; }

        /// <summary>
        ///     số lần quá vận tốc
        /// </summary>
        [DataMember(Name = "overspeedcount")]
        public int OverSpeedCount { get; set; }

        /// <summary>
        ///     số lần mở cửa
        /// </summary>
        [DataMember(Name = "opendoorcount")]
        public int OpenDoorCount { get; set; }

        /// <summary>
        ///     số lần dừng đỗ
        /// </summary>
        [DataMember(Name = "pausecount")]
        public int PauseCount { get; set; }

        /// <summary>
        ///     kiểu xe (taxi,car)
        /// </summary>
        [DataMember(Name = "type")]
        public DeviceActivityType Type { get; set; }

        /// <summary>
        ///     tên đội xe
        /// </summary>
        [DataMember(Name = "group")]
        public string DeviceGroup { get; set; }

        /// <summary>
        ///     id công ty
        /// </summary>
        [DataMember(Name = "companyId")]
        public long CompanyId { get; set; }
    }
}