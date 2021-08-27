using System;

namespace Core.Models.Tranfer.DeviceManager
{
    public class RefuelLogTranfer
    {
        public long Id { get; set; }
        public string Serial { get; set; }
        public string Bs { get; set; }
        public long Distance { get; set; }
        public float FuelUse { get; set; }
        public long DriverId { get; set; }
        public long GroupId { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}