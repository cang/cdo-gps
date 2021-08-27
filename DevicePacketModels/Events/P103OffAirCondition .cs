using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(103)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P103OffAirCondition : BaseEvent
    {
        public P103OffAirCondition()
        {
        }

        public P103OffAirCondition(byte[] data) : base(data)
        {
        }
    }
}