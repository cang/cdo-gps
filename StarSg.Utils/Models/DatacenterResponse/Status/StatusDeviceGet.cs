using System;
using System.Collections;
using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Status
{
    public class StatusDeviceGet:BaseResponse
    {
        public IList<StatusDeviceTranfer> Datas { get; set; }
    }
    public class StatusDeviceGetBare : BaseResponse
    {
        public IList<StatusDeviceTranferBare> Datas { get; set; }
    }

    public class StatusDeviceTranfer : StatusDeviceTranferBare
    {
        //public string Bs { get; set; }
        //public long Serial { get; set; }
        public bool Machine { get; set; }
        public bool AirMachine { get; set; }
        public bool Door { get; set; }
        //public int Speed { get; set; }
        public int Fuel { get; set; }
        public int OverSpeedCount { get; set; }
        public int OverTime { get; set; }
        public int OverTimeInDay { get; set; }
        public bool Gps { get; set; }
        public int Gsm { get; set; }
        public float Power { get; set; }
        public string Name { get; set; }
        public string Gplx { get; set; }
        //public DateTime Time { get; set; }
        //public GpsPoint Point { get; set; }
        public bool LostSingnal { get; set; }
        public long TotalGpsDistance { get; set; }
        public long TotalCurrentGpsDistance { get; set; }
        public short PauseCount { get; set; }
        public string Model { get; set; }
        public int KmOnDay { get; set; }
        public long GroupId { get; set; }
        //public DateTime EndTime { get; set; }

        public bool useGuest { get; set; }
        public bool Guest { get; set; }

        /// <summary>
        /// Thời điểm dừng lần cuối, nếu giá trị này là <= DateTime.MinValue thì có nghĩa là xe đang chạy
        /// </summary>
        public DateTime PauseTime { get; set; }

        public string CameraId { get; set; }
    }

    public class StatusDeviceTranferBare
    {
        public long Serial { get; set; }
        public string Bs { get; set; }
        public DateTime Time { get; set; }
        public DateTime EndTime { get; set; }
        public int Speed { get; set; }
        public GpsPoint Point { get; set; }
        public string status { get; set; }
    }

}