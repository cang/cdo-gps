using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(305)]
    public class P305AddSerialRouteTable : NodeSharePacketModel
    {
        public P305AddSerialRouteTable(byte[] data) : base(data)
        {
        }
        public P305AddSerialRouteTable()
        {
        }
        
        public List<long> SerialList { get; set; } = new List<long>();
        public string DataCenterId { get; set; }

        //2 byte đầu định nghĩa chiều dài mãng serial, mỗi serial dài 32 byte
        public override bool Deserializer()
        {
            int lenghtSerial = ReadInt16();
            for (int i = 0; i < lenghtSerial; i++)
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
            WriteString(DataCenterId,50);
            return base.Serializer();
        }
    }
}
