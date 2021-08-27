using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(102)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P102OnAirCondition : BaseEvent
    {
        public P102OnAirCondition()
        {
        }

        public P102OnAirCondition(byte[] data) : base(data)
        {
        }
    }
}