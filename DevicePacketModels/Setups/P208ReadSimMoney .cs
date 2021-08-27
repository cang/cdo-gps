using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(208)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P208ReadSimMoney : DevicePacketModel
    {
        public P208ReadSimMoney()
        {
        }

        public P208ReadSimMoney(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }
}