using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(303)]
    public class P303RemoveDataCenter : NodeSharePacketModel
    {
        public P303RemoveDataCenter(byte[] data) : base(data)
        {
        }
        public P303RemoveDataCenter()
        {
        }

        //delete datacenter theo Id
        public string Id { get; set; }
        public override bool Deserializer()
        {
            Id = ReadString(50);
            return true;
        }
        public override byte[] Serializer()
        {
            WriteString(Id, 50);
            return base.Serializer();
        }
    }
}
