using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Setups
{
    [DeviceOpCode(205)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P205DeviceInfo : DevicePacketModel
    {
        public P205DeviceInfo()
        {
        }

        public P205DeviceInfo(byte[] data) : base(data)
        {
        }

        // public long Serial { get; set; }
        [DataMember]
        public DateTime TimeUpdate { get; set; }
        [DataMember]
        public int TimeSync { get; set; }
        [DataMember]
        public short OverTimeInSession { get; set; }
        [DataMember]
        public short OverTimeInDay { get; set; }
        [DataMember]
        public byte OverSpeed { get; set; }
        [DataMember]
        public IList<string> PhoneSystemControl { get; set; } = new List<string>();
        [DataMember]
        public string FirmWareVersion { get; set; }
        [DataMember]
        public string HardWareVersion { get; set; }

        public override bool Deserializer()
        {
            Serial = ReadInt64();
            var time = ReadInt32();
            TimeUpdate = DateTimeConvert.GetTimeByUnixTime(time);
            TimeSync = ReadInt16();
            OverTimeInSession = ReadInt16();
            OverTimeInDay = ReadInt16();
            OverSpeed = ReadByte();
            var phonelen = ReadByte();
            for (var i = 0; i < phonelen; i++)
            {
                PhoneSystemControl.Add(ReadString(16));
            }
            var flen = ReadByte();
            FirmWareVersion = ReadString(flen);
            var hlen = ReadByte();
            HardWareVersion = ReadString(hlen);
            return true;
        }
    }
}