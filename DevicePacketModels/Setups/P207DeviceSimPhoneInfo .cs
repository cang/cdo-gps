using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(207)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P207DeviceSimPhoneInfo : DevicePacketModel
    {
        public P207DeviceSimPhoneInfo()
        {
        }

        public P207DeviceSimPhoneInfo(byte[] data) : base(data)
        {
        }

        //public long Serial { get; set; }
        public DateTime TimeUpdate { get; set; }
        public string Phone { get; set; }

        public override bool Deserializer()
        {
            Serial = ReadInt64();
            var time = ReadInt32();
            TimeUpdate = DateTimeConvert.GetTimeByUnixTime(time);
            var len = ReadByte();
            Phone = ReadString(len);
            return true;
        }
    }
}