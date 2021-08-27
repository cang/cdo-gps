using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(107)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P107EndOverSpeed : BaseEvent
    {
        public P107EndOverSpeed()
        {
        }

        public P107EndOverSpeed(byte[] data) : base(data)
        {
        }
    }
}