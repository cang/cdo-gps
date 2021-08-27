using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(308)]
    public class P308RemoveCompanyRouteTable : NodeSharePacketModel
    {
        public P308RemoveCompanyRouteTable(byte[] data) : base(data)
        {
        }

        public P308RemoveCompanyRouteTable()
        {
        }
     
        //2 byte đầu tiên là chiều dài của list
        public List<long> CompanyIdList { get; set; } = new List<long>();
        public override bool Deserializer()
        {
            int lenght = ReadInt16();
            for (int i = 0; i < lenght; i++)
            {
                CompanyIdList.Add(ReadInt64());
            }
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(CompanyIdList.Count);
            foreach (var companyId in CompanyIdList)
            {
                WriteInt64(companyId);
            }
            return base.Serializer();
        }
    }
}
