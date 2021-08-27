using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(310)]
    public class P310SyncCompanyRouteTable : NodeSharePacketModel
    {
        public P310SyncCompanyRouteTable(byte[] data) : base(data)
        {
        }

        public P310SyncCompanyRouteTable()
        {
        }

        /// <summary>
        /// 2 byte định nghĩa chiều dài của dictionary
        /// từng node của dictionary: 
        /// 50 byte định nghĩa DataCenterId
        /// list company tương ứng: 2 byte định nghĩa chiều dài list
        /// </summary>
        public Dictionary<Guid, List<long>> CompanyDictionary { get; set; } = new Dictionary<Guid, List<long>>();
        public override bool Deserializer()
        {
            int dictionaryLenght = ReadInt16();
            for (int i = 0; i < dictionaryLenght; i++)
            {
                string dataCenterId = ReadString(50);
                int lenghtList = ReadInt16();
                List<long> companyList = new List<long>();
                for (int j = 0; j < lenghtList; j++)
                {
                    companyList.Add(ReadInt64());
                }
                CompanyDictionary.Add(Guid.Parse(dataCenterId), companyList);
            }
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(CompanyDictionary.Count);
            foreach (var it in CompanyDictionary)
            {
                WriteString(it.Key.ToString(), 50);
                WriteInt16(it.Value.Count);
                foreach (var companyId in it.Value)
                {
                    WriteInt64(companyId);
                }
            }
            return base.Serializer();
        }
    }
}
