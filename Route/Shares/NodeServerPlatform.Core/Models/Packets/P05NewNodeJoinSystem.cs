#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : P05NewNodeJoinSystem.cs
// Time Create : 1:54 PM 29/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Core.Models.Packets
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(5)]
    public class P05NewNodeJoinSystem : NodeSharePacketModel
    {
        public P05NewNodeJoinSystem(byte[] data) : base(data)
        {
        }

        public P05NewNodeJoinSystem()
        {
        }

        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }

        #region Overrides of PacketModel

        public override bool Deserializer()
        {
            Name = ReadString(32);
            Ip = ReadString(16);
            Port = ReadInt16();
            return true;
        }

        #region Overrides of PacketModel

        public override byte[] Serializer()
        {
            WriteString(Name, 32);
            WriteString(Ip, 16);
            WriteInt16(Port);
            return base.Serializer();
        }

        #endregion

        #endregion
    }
}