using System;

namespace Core.Models.Tranfer.LogManager
{
    public class DeviceRawLogTranfer
    {
        public long Id { get; set; }
        public string Serial { get; set; }
        public DateTime TimeRecive { get; set; }
        public DateTime TimeSend { get; set; }
        public string Data { get; set; }
        public string Cookie { get; set; }
    }
}