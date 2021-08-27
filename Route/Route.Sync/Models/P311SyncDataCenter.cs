using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;
using Route.Core;

namespace Route.Sync.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(311)]
    public class P311SyncDataCenter : NodeSharePacketModel
    {
        public P311SyncDataCenter(byte[] data) : base(data)
        {
        }

        public P311SyncDataCenter()
        {
        }

        public IList<DataCenterInfo> DataCenterList { get; set; } = new List<DataCenterInfo>();

        public override bool Deserializer()
        {
            int lenght = ReadInt16();
            for (var i = 0; i < lenght; i++)
            {
                var dataCenterInfo = new DataCenterInfo();
                dataCenterInfo.Id = Guid.Parse(ReadString(50));
                dataCenterInfo.Ip = ReadString(32);
                dataCenterInfo.Port = ReadInt32();
                dataCenterInfo.NodeName = ReadString(32);
                DataCenterList.Add(dataCenterInfo);
            }
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(DataCenterList.Count);
            foreach (var it in DataCenterList)
            {
                WriteString(it.Id.ToString(), 50);
                WriteString(it.Ip, 32);
                WriteInt32(it.Port);
                WriteString(it.NodeName, 32);
            }
            return base.Serializer();
        }
    }
}