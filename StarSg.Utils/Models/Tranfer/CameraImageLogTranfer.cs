using System;
using StarSg.Utils.Models.Tranfer;

namespace Core.Models.Tranfer
{
    public class CameraImageLogTranfer
    {
        public long Id { get; set; }
        public string Serial { get; set; }
        public DateTime TimeUpdate { get; set; }
        public byte[] ImageData { get; set; }
        public GpsPoint Location { get; set; }
        public int CameraLocation { get; set; }
    }
}