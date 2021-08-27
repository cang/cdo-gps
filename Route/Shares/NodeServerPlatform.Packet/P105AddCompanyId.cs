using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Core.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(105)]
    public class P105AddCompanyId : NodeSharePacketModel
    {
        public P105AddCompanyId(byte[] data) : base(data)
        {
        }

        public P105AddCompanyId()
        {
        }

        public List<long> CompanyIdList { get; set; } = new List<long>();

        /// <summary>
        ///     id của database Center chứa company
        /// </summary>
        public string DataCenterId { get; set; }

        public override bool Deserializer()
        {
            //2 byte định danh chiều dài của mãng company
            int lenght = ReadInt16();
            for (var i = 0; i < lenght; i++)
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
            WriteString(DataCenterId, 50);
            return base.Serializer();
        }
    }
}