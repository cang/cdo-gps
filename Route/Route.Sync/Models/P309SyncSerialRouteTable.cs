using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(309)]
    public class P309SyncSerialRouteTable : NodeSharePacketModel
    {
        public P309SyncSerialRouteTable(byte[] data) : base(data)
        {
        }

        public P309SyncSerialRouteTable()
        {
        }

        /// <summary>
        ///     2 byte định nghĩa chiều dài của dictionary
        ///     từng node của dictionary:
        ///     50 byte định nghĩa DataCenterId
        ///     list serial tương ứng: 2 byte định nghĩa chiều dài list
        /// </summary>
        public Dictionary<Guid, List<long>> SerialDictionary { get; set; } = new Dictionary<Guid, List<long>>();

        public override bool Deserializer()
        {
            int dictionaryLenght = ReadInt16();
            for (var i = 0; i < dictionaryLenght; i++)
            {
                var dataCenterId = ReadString(50);
                int lenghtList = ReadInt16();
                var serialList = new List<long>();
                for (var j = 0; j < lenghtList; j++)
                {
                    serialList.Add(ReadInt64());
                }
                SerialDictionary.Add(Guid.Parse(dataCenterId), serialList);
            }
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(SerialDictionary.Count);
            foreach (var it in SerialDictionary)
            {
                WriteString(it.Key.ToString(), 50);
                WriteInt16(it.Value.Count);
                foreach (var serial in it.Value)
                {
                    WriteInt64(serial);
                }
            }
            return base.Serializer();
        }
    }
}