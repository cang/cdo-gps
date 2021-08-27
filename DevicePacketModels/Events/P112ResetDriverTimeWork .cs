using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(112)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P112ResetDriverTimeWork : BaseEvent
    {
        public P112ResetDriverTimeWork()
        {
        }

        public P112ResetDriverTimeWork(byte[] data) : base(data)
        {
        }

        public int DriverId { get; set; }

        public override bool Deserializer()
        {
            base.Deserializer();
            DriverId = ReadInt32();
            return true;
        }
    }
}