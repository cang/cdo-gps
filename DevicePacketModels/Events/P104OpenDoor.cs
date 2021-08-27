using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(104)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P104OpenDoor : BaseEvent
    {
        public P104OpenDoor()
        {
        }

        public P104OpenDoor(byte[] data) : base(data)
        {
        }
    }
}