using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(109)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P109EndStop : BaseEvent
    {
        public P109EndStop()
        {
        }

        public P109EndStop(byte[] data) : base(data)
        {
        }
    }
}