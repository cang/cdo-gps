#region header
// /*********************************************************************************************/
// Project :DataTranport
// FileName : DeviceInfo.cs
// Time Create : 2:18 PM 17/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;

namespace Datacenter.DataTranport.Models
{
    public class DeviceInfo
    {
        public int Speed { get; set; }
        public string TaxiNumber { get; set; }
        public string SerialDriver { get; set; }
        public bool IsOpenDoor { get; set; }
        public bool IsOpenKey { get; set; }
        public bool IsRunAirconditioner { get; set; }
        public DateTime TimeUpdateInClient { get; set; }
        public double NewLat { get; set; }
        public double NewLng { get; set; }
        public double OldLat { get; set; }
        public double OldLong { get; set; }
    }
}