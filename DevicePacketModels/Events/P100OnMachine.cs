using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(100)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P100OnMachine : BaseEvent
    {
        public P100OnMachine(byte[] data) : base(data)
        {
        }

        public P100OnMachine()
        {
        }
    }
}