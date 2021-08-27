using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(307)]
    public class P307AddCompanyIdRouteTable : NodeSharePacketModel
    {
        public P307AddCompanyIdRouteTable(byte[] data) : base(data)
        {
        }

        public P307AddCompanyIdRouteTable()
        {
        }
        
        //2 byte định danh chiều dài của mãng company
        public List<long> CompanyIdList { get; set; } = new List<long>();
        //id của database Center chứa company
        public string DataCenterId { get; set; }

        public override bool Deserializer()
        {
            int lenght = ReadInt16();
            for (int i = 0; i < lenght; i++)
            {
                CompanyIdList.Add(ReadInt64());
            }
            DataCenterId = ReadString(50);
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(CompanyIdList.Count);
            foreach (var company in CompanyIdList)
            {
                WriteInt64(company);
            }
            WriteString(DataCenterId,50);
            return base.Serializer();
        }
    }
}
