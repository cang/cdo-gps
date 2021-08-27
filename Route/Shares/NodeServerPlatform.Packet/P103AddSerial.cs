using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Core.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(103)]
    public class P103AddSerial : NodeSharePacketModel
    {
        public P103AddSerial(byte[] data) : base(data)
        {
        }

        public P103AddSerial()
        {
        }

        public List<long> SerialList { get; set; } = new List<long>();
        public string DataCenterId { get; set; }

        public override bool Deserializer()
        {
            //2 byte đầu định nghĩa chiều dài mãng serial, mỗi serial dài 32 byte
            int lenghtSerial = ReadInt16();
            for (var i = 0; i < lenghtSerial; i++)
            {
                SerialList.Add(ReadInt64());
            }
            DataCenterId = ReadString(50);
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(SerialList.Count);
            foreach (var serial in SerialList)
            {
                WriteInt64(serial);
            }
            WriteString(DataCenterId, 50);
            return base.Serializer();
        }
    }
}