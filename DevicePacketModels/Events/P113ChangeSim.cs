using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(113)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P113ChangeSim : DevicePacketModel
    {
        public P113ChangeSim()
        {
        }

        public P113ChangeSim(byte[] data) : base(data)
        {
        }

        //public long Serial { get; set; }
        public DateTime TimeUpdate { get; set; }

        public override bool Deserializer()
        {
            Serial = ReadInt64();
            var time = ReadInt32();
            TimeUpdate = DateTimeConvert.GetTimeByUnixTime(time);
            return true;
        }
    }
}