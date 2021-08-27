using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(209)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P209DeviceSimeMoneyInfo : DevicePacketModel
    {
        public P209DeviceSimeMoneyInfo()
        {
        }

        public P209DeviceSimeMoneyInfo(byte[] data) : base(data)
        {
        }

       // public long Serial { get; set; }
        public DateTime TimeUpdate { get; set; }
        public string Money { get; set; }

        public override bool Deserializer()
        {
            Serial = ReadInt64();
            var time = ReadInt32();
            TimeUpdate = DateTimeConvert.GetTimeByUnixTime(time);
            var len = ReadByte();
            Money = ReadString(len);
            return true;
        }
    }
}