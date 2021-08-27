using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(202)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]

    [DataContract]
    public class P202SetupDevice : DevicePacketModel
    {
        public P202SetupDevice()
        {
        }

        public P202SetupDevice(byte[] data) : base(data)
        {
        }

        [DataMember]
        public short TimeSync { get; set; }
        [DataMember]
        public short OverTimeInSession { get; set; }
        [DataMember]
        public short OverTimeInDay { get; set; }
        [DataMember]
        public byte OverSpeed { get; set; }
        [DataMember]
        public IList<string> PhoneSystemControl { get; set; } = new List<string>();
        [DataMember]
        public string Bs { get; set; }
        public override bool Deserializer()
        {
            TimeSync = ReadInt16();
            OverTimeInSession = ReadInt16();
            OverTimeInDay = ReadInt16();
            OverSpeed = ReadByte();
            var len = ReadByte();
            for (var i = 0; i < len; i++)
            {
                PhoneSystemControl.Add(ReadString(16));
            }
            len = ReadByte();
            Bs = ReadString(len);
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(TimeSync);
            WriteInt16(OverTimeInSession);
            WriteInt16(OverTimeInDay);
            WriteByte(OverSpeed);
            WriteByte((byte) PhoneSystemControl.Count);
            foreach (var s in PhoneSystemControl)
            {
                WriteString(s, 16);
            }
            WriteByte((byte) Bs.Length);
            WriteString(Bs);
            return base.Serializer();
        }
    }
}