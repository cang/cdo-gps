using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(306)]
    public class P306RemoveSerialRouteTable : NodeSharePacketModel
    {
        public P306RemoveSerialRouteTable(byte[] data) : base(data)
        {
        }
        public P306RemoveSerialRouteTable()
        {
        }
        
        //2 byte đầu định nghĩa chiều dài mãng serial, mỗi serial dài 32 byte
        public List<long> SerialList { get; set; } = new List<long>();
        public override bool Deserializer()
        {
            int lenght = ReadInt16();
            for (int i = 0; i < lenght; i++)
            {
               SerialList.Add(ReadInt64()); 
            }
            return true;
        }
        public override byte[] Serializer()
        {
            WriteInt16(SerialList.Count);
            foreach (var serial in SerialList)
            {
                WriteInt64(serial);
            }
            return base.Serializer();
        }
    }
}
