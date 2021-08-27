using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    /// <summary>
    ///     gói tin báo tắt máy
    /// </summary>
    [DeviceOpCode(101)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P101OffMachine : BaseEvent
    {
        public P101OffMachine()
        {
        }

        public P101OffMachine(byte[] data) : base(data)
        {
        }
    }
}