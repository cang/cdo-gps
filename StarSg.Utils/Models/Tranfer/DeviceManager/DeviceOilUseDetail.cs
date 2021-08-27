using System;

namespace Core.Models.Tranfer.DeviceManager
{
    public class DeviceOilUseDetail
    {
        public DateTime TimeUpdate { get; set; }
        public long Distance { get; set; }
        public float FuelUsed { get; set; }
    }
}