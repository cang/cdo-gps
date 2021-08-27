using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Core.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(106)]
    public class P106RemoveCompanyId : NodeSharePacketModel
    {
        public P106RemoveCompanyId(byte[] data) : base(data)
        {
        }

        public P106RemoveCompanyId()
        {
        }

        public List<long> CompanyIdList { get; set; } = new List<long>();

        public override bool Deserializer()
        {
            //2 byte đầu tiên là chiều dài của list
            int lenght = ReadInt16();
            for (var i = 0; i < lenght; i++)
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