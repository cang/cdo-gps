using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(210)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P210DeviceUpdate:DevicePacketModel
    {
        public string Name { set; get; }
        public P210DeviceUpdate()
        {
        }

        public P210DeviceUpdate(byte[] data) : base(data)
        {
        }
        public override bool Deserializer()
        {
            var len = ReadByte();
            Name = ReadString(len);
            return true;
        }

        public override byte[] Serializer()
        {
            WriteByte((byte) Name.Length);
            WriteString(Name);
            return base.Serializer();
        }
    }
}