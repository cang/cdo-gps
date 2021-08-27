using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(211)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P211SetupBS : DevicePacketModel
    {
        public string Bs { set; get; }
        public P211SetupBS()
        {
        }

        public P211SetupBS(byte[] data) : base(data)
        {
        }
        public override bool Deserializer()
        {
            var len = ReadByte();
            Bs = ReadString(len);
            return true;
        }

        public override byte[] Serializer()
        {
            WriteByte((byte)Bs.Length);
            WriteString(Bs);
            return base.Serializer();
        }
    }
}