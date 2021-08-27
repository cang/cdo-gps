#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: DevicePacketModels
// TIME CREATE : 7:04 PM 01/11/2016
// FILENAME: P114EndOvertime.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.ExternModel;
using DevicePacketModels.Utils;

#endregion

namespace DevicePacketModels.Events
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(114)]
    [Export(typeof (DevicePacketModel))]
    [DataContract]
    public class P114EndOvertime : DevicePacketModel
    {
        public P114EndOvertime()
        {
        }

        public P114EndOvertime(byte[] data) : base(data)
        {
            //Debug.WriteLine(BitConverter.ToString(data));
        }

        [DataMember]
        public DateTime BeginTime { get; set; }

        [DataMember]
        public GpsInfo GpsBegin { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public GpsInfo GpsEnd { get; set; }

        [DataMember]
        public int DriverId { get; set; }
        [DataMember]
        public int Distance { get; set; }

        public override bool Deserializer()
        {
            Serial = ReadInt64();

            BeginTime = DateTimeConvert.GetTimeByUnixTime(ReadInt32());
            ReadByte();
            GpsBegin = new GpsInfo()
            {
                Lat = ReadFloat(),
                Lng = ReadFloat()
            };
            EndTime = DateTimeConvert.GetTimeByUnixTime(ReadInt32());
            ReadByte();
            GpsEnd = new GpsInfo()
            {
                Lat = ReadFloat(),
                Lng = ReadFloat()
            };
            DriverId = ReadInt32();
            Distance = ReadInt32();
            return true;
        }
    }
}