using System;

namespace StarSg.Utils.Models.Tranfer.DeviceManager
{
    public class DeviceTripTranfer
    {
        public string Bs { get; set; }
        public DateTime TimeUpdate { get; set; }
        public GpsPoint Location { get; set; }
        public int Speed { get; set; }
        public bool MachineStatus { get; set; }
        public bool AirMachineStatus { get; set; }
        public bool DoorStatus { get; set; }
        public float Fuel { get; set; }
        public float Power { get; set; }
        public float Temperature { get; set; }
        public bool GpsStatus { get; set; }
        public long Distance { get; set; }
        public long TotalDistance { get; set; }
        public double Angle { get; set; }
        public string DriverName { get; set; }
        public string Gplx { get; set; }
        public DateTime GplxCreateTime { get; set; }
        public DateTime GplxEndTime { get; set; }
    }

    public class DeviceRawTranfer
    {
        public DateTime ClientSend { get; set; }
        public DateTime ServerRecv { get; set; }
        public String HexData { get; set; }
        //public byte[] Data { get; set; }
        public String Note { get; set; }
    }
}