using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(106)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P106BeginOverSpeed : BaseEvent
    {
        public P106BeginOverSpeed()
        {
        }

        public P106BeginOverSpeed(byte[] data) : base(data)
        {
        }
    }
}