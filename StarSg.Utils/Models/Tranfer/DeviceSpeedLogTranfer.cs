using System;

namespace StarSg.Utils.Models.Tranfer
{
    public class DeviceSpeedLogTranfer
    {
        public string Bs { get; set; }
        public string SpeedTrace { get; set; }

        public DateTime ClientSend { get; set; }
    }
}