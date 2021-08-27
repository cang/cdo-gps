using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Models;
using CorePacket.Utils;
using DevicePacketModels.Utils;

namespace DevicePacketModels.Specials
{
    [DeviceOpCode(301)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P301CompressPacket : DevicePacketModel
    {
        public P301CompressPacket()
        {
        }

        public P301CompressPacket(byte[] data) : base(data)
        {
            Debug.WriteLine(BitConverter.ToString(data));
        }

        public IList<IDevicePacket> Datas { get; set; } = new List<IDevicePacket>();

        //public int InvalidCheckSum;

        public override bool Deserializer()
        {
            if (_memory != null && _memory.Length < 10) return false;

            Serial = ReadInt64();
            var len = ReadInt16();

            //InvalidCheckSum = 0;

            for (var i = 0; i < len; i++)
            {
                var opcode = ReadInt16();//2
                var pLen = ReadInt16();//2

                if (pLen < 0) return false;

                var pData = ReadBytes(pLen);
                var csum = ReadUInt32();

                //if (Crc32.ComputeChecksum(pData) != csum) InvalidCheckSum++;

                // todo: tính checksum chỗ này
                Datas.Add(new NodeBasePacket(opcode, pData));
            }

            return true;
        }
    }
}