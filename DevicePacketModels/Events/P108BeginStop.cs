using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(108)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P108BeginStop : BaseEvent
    {
        public P108BeginStop()
        {
        }

        public P108BeginStop(byte[] data) : base(data)
        {
        }
    }
}