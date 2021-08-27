using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(203)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P203SetOverSpeed : DevicePacketModel
    {
        public P203SetOverSpeed()
        {
        }

        public P203SetOverSpeed(byte[] data) : base(data)
        {
        }

        [DataMember]
        public byte OverSpeed { get; set; }

        public override bool Deserializer()
        {
            OverSpeed = ReadByte();
            return true;
        }

        public override byte[] Serializer()
        {
            WriteByte(OverSpeed);
            return base.Serializer();
        }
    }
}