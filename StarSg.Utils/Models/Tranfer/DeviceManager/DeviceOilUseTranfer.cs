using System.Collections.Generic;

namespace Core.Models.Tranfer.DeviceManager
{
    public class DeviceOilUseTranfer
    {
        public DeviceOilUseTranfer(string serial, List<DeviceOilUseDetail> dataOil)
        {
            Serial = serial;
            DataOil = dataOil;
        }

        public string Serial { get; set; }
        public List<DeviceOilUseDetail> DataOil { get; set; }
    }
}