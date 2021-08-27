using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using System;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(212)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P212SetupStartDate : DevicePacketModel
    {
        public DateTime StartDate { get; set; }
        public P212SetupStartDate()
        {
        }

        public P212SetupStartDate(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            var time = ReadInt32();
            StartDate = DateTimeConvert.GetTimeByUnixTime(time);
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt32((int)DateTimeConvert.GetTimeByUnixTime(StartDate));
            return base.Serializer();
        }
    }
}