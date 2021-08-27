using System;
using System.Runtime.Serialization;
using CorePacket;
using DevicePacketModels.ExternModel;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Events
{
    public class BaseEvent : DevicePacketModel
    {
        public BaseEvent()
        {
        }

        public BaseEvent(byte[] data) : base(data)
        {
        }

        //public long Serial { get; set; }
        [DataMember]
        public DateTime TimeUpdate { get; set; }
        [DataMember]
        public GpsInfo GpsInfo { get; set; } = new GpsInfo();
        public bool GpsStatus { get; set; }
        public override bool Deserializer()
        {
            Serial = ReadInt64();
            var unitTime = ReadInt32();
            TimeUpdate = DateTimeConvert.GetTimeByUnixTime(unitTime);
            GpsStatus = ReadBool();
            GpsInfo.Lat = ReadFloat();
            GpsInfo.Lng = ReadFloat();
            GpsInfo.Speed = ReadByte();
            return true;
        }
    }
}