using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Events
{
    [DeviceOpCode(110)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P110DeviceReset : BaseEvent
    {
        //todo:  có thêm một số trường do người sử dụng thêm vào
        public P110DeviceReset()
        {
        }

        public P110DeviceReset(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            base.Deserializer();

            return true;
        }
    }
}