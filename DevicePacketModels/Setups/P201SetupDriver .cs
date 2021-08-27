using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.Text;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(201)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P201SetupDriver : DevicePacketModel
    {
        [DataMember]
        public int DriverId { get; set; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public string DriverSerial { get; set; }

        public override bool Deserializer()
        {
            DriverId = ReadInt32();
            var nameLen = ReadByte();
            Name = Encoding.ASCII.GetString(ReadBytes(nameLen));
            var dLen = ReadByte();
            DriverSerial = Encoding.ASCII.GetString(ReadBytes(dLen));
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt32(DriverId);
            WriteByte((byte) Name.Length);
            WriteString(Name);
            WriteByte((byte) DriverSerial.Length);
            WriteString(DriverSerial);
            return base.Serializer();
        }
    }
}