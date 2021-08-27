using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Specials
{
    [DeviceOpCode(302)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P302RedirectConnection : DevicePacketModel
    {
        public P302RedirectConnection()
        {
        }

        public P302RedirectConnection(byte[] data) : base(data)
        {
        }

        public string Domain { get; set; }
        public short Port { get; set; }

        public override bool Deserializer()
        {
            var len = ReadByte();
            Domain = ReadString(len);
            Port = ReadInt16();
            return true;
        }

        public override byte[] Serializer()
        {
            WriteByte((byte) Domain.Length);
            WriteString(Domain);
            WriteInt16(Port);
            return base.Serializer();
        }
    }
}