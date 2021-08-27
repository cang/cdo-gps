using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(105)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P105CloseDoor : BaseEvent
    {
        public P105CloseDoor()
        {
        }

        public P105CloseDoor(byte[] data) : base(data)
        {
        }
    }
}