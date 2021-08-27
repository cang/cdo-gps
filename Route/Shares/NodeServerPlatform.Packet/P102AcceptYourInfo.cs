using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Core.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(102)]
    public class P102AcceptYourInfo : NodeSharePacketModel
    {
        public P102AcceptYourInfo(byte[] data) : base(data)
        {
        }

        public P102AcceptYourInfo()
        {
        }

        public bool IsAccept { get; set; }

        public override bool Deserializer()
        {
            IsAccept = ReadBool();
            return true;
        }

        public override byte[] Serializer()
        {
            WriteBool(IsAccept);
            return base.Serializer();
        }
    }
}