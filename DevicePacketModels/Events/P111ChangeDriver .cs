using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(111)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P111ChangeDriver : BaseEvent
    {
        public P111ChangeDriver()
        {
        }

        public P111ChangeDriver(byte[] data) : base(data)
        {
        }

        /// <summary>
        ///     Id tài xế mới
        /// </summary>
        [DataMember]
        public int NewDriverId { get; set; }

        /// <summary>
        ///     Id tài xế cũ
        /// </summary>
        [DataMember]
        public int OldDriverId { get; set; }

        public override bool Deserializer()
        {
            base.Deserializer();
            NewDriverId = ReadInt32();
            OldDriverId = ReadInt32();
            return true;
        }
    }
}