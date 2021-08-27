using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(206)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P206ReadSimPhone : DevicePacketModel
    {
        public P206ReadSimPhone()
        {
        }

        public P206ReadSimPhone(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }
}