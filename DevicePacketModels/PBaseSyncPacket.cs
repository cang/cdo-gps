using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.ExternModel;
using DevicePacketModels.Utils;

namespace DevicePacketModels
{
    [DataContract]
    public class PBaseSyncPacket : DevicePacketModel
    {
        public PBaseSyncPacket()
        {
        }

        public PBaseSyncPacket(byte[] data) : base(data)
        {
        }

        [DataMember]
        // public long Serial { get; set; }
        public DateTime Time { get; set; }
        [DataMember]
        public bool GpsStatus { get; set; }
        [DataMember]
        public GpsInfo GpsInfo { get; set; } = new GpsInfo();
        [DataMember]
        public long TotalGpsDistance { get; set; }
        [DataMember]
        public long TotalCurrentGpsDistance { get; set; }
        [DataMember]
        public StatusIO StatusIo { get; set; }
        [DataMember]
        public int IOValue { get; set; }
        [DataMember]
        public int DriverId { get; set; }
        [DataMember]
        public int Fuel { get; set; }
        [DataMember]
        public short Temperature { get; set; }
        [DataMember]
        public byte GsmSignal { get; set; }
        [DataMember]
        public byte Power { get; set; }
        [DataMember]
        public IList<byte> SpeedLogs { get; set; } = new List<byte>();
        [DataMember]
        public short TimeWork { get; set; }
        [DataMember]
        public short TimeWorkInDay { get; set; }
        #region Overrides of PacketModel

        public override bool Deserializer()
        {
            //2E00 3939393939393939 FD6E7038 00 0001020304050607 0000000029AA5555 01020304 78563412 45231234 0000000000 42FA26C4
            //4100 3939393939393939 12550257 01 1E502C41 B648D542 03 B1CB740000000000 87D6120000000000 41AA5555 01020304 B80B0000 9600 14 5A0A02030301030200010301D96CC7CD
            //0100 3800 3939393939393939 C1CA0C57 00B1CB74000000000087D612000000000045AA55554E61BC00B80B00009600145A0A00000000000000000000D4BD69A0
            Serial = ReadInt64();
            var unixTime = ReadUInt32();

            Time = DateTimeConvert.GetTimeByUnixTime(unixTime);
            GpsStatus = ReadBool();
            if (GpsStatus)
            {
                GpsInfo.Lat = ReadFloat();
                GpsInfo.Lng = ReadFloat();
                GpsInfo.Speed = ReadByte();
            }
            TotalGpsDistance = ReadInt64();
            TotalCurrentGpsDistance = ReadInt64();
            IOValue = ReadInt32();
            StatusIo = StructBitConvert.ToStruct<StatusIO>(IOValue);
            DriverId = ReadInt32();
            Fuel = ReadInt32();
            Temperature = ReadInt16();
            GsmSignal = ReadByte();
            Power = ReadByte();
            var len = ReadByte();
            for (var i = 0; i < len; i++)
            {
                SpeedLogs.Add(ReadByte());
            }
            TimeWork = ReadInt16();
            TimeWorkInDay = ReadInt16();
            return true;
            //throw new System.NotImplementedException();
        }

        #region Overrides of PacketModel

        public override byte[] Serializer()
        {
            return base.Serializer();
        }

        #endregion

        #endregion
    }
}
