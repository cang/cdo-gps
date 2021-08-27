using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(204)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P204ReadDeviceInfo : DevicePacketModel
    {
        public P204ReadDeviceInfo()
        {
        }

        public P204ReadDeviceInfo(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }

    [DeviceOpCode(220)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P220TurnOffSound : DevicePacketModel
    {
        public P220TurnOffSound()
        {
        }

        public P220TurnOffSound(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }

    [DeviceOpCode(221)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P221TurnOnSound : DevicePacketModel
    {
        public P221TurnOnSound()
        {
        }

        public P221TurnOnSound(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }


    [DeviceOpCode(222)] //201 login driver
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P222LogoutDriver : DevicePacketModel
    {
        public P222LogoutDriver()
        {
        }

        public P222LogoutDriver(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }


    [DeviceOpCode(223)] //turn off RF
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P223TurnOffRF : DevicePacketModel
    {
        public P223TurnOffRF()
        {
        }

        public P223TurnOffRF(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }

    [DeviceOpCode(224)] //turn on  RF
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P224TurnOnRF : DevicePacketModel
    {
        public P224TurnOnRF()
        {
        }

        public P224TurnOnRF(byte[] data) : base(data)
        {
        }

        public override bool Deserializer()
        {
            return true;
        }
    }


}