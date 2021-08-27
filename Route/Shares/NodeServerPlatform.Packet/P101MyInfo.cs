using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Packet
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(101)]
    public class P101MyInfo : NodeSharePacketModel
    {
        public P101MyInfo(byte[] data) : base(data)
        {
        }

        public P101MyInfo()
        {
        }

        public string Ip { get; set; }
        public string Id { get; set; }
        public int Port { get; set; }
        public string NodeName { get; set; }
        public int ReportCount { get; set; }

        public override bool Deserializer()
        {
            Ip = ReadString(32);
            Id = ReadString(50);
            Port = ReadInt32();
            NodeName = ReadString(32);
            ReportCount = ReadInt32();
            return true;
        }

        public override byte[] Serializer()
        {
            WriteString(Ip, 32);
            WriteString(Id, 50);
            WriteInt32(Port);
            WriteString(NodeName, 32);
            WriteInt32(ReportCount);
            return base.Serializer();
        }
    }
}