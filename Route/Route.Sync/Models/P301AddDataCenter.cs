#region header

// /*********************************************************************************************/
// Project :Route.DatacenterStore
// FileName : P101AddDataCenter .cs
// Time Create : 9:32 AM 24/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(301)]
    public class P301AddDataCenter : NodeSharePacketModel
    {
        public P301AddDataCenter(byte[] data) : base(data)
        {
        }

        public P301AddDataCenter()
        {
        }

        public string Ip { get; set; }
        public string Id { get; set; }
        public int Port { get; set; }
        public string NodeName { get; set; }
        #region Overrides of PacketModel

        public override bool Deserializer()
        {
            Ip = ReadString(32);
            Id = ReadString(50);
            Port = ReadInt32();
            NodeName = ReadString(32);
            return true;
        }

        #region Overrides of PacketModel

        public override byte[] Serializer()
        {
            WriteString(Ip, 32);
            WriteString(Id, 50);
            WriteInt32(Port);
            WriteString(NodeName, 32);
            return base.Serializer();
        }

        #endregion

        #endregion
    }
}