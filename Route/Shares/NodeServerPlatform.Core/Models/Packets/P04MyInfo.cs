#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : P04MyInfo.cs
// Time Create : 4:19 PM 28/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Core.Models.Packets
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(4)]
    public class P04MyInfo : NodeSharePacketModel
    {
        public P04MyInfo(byte[] data) : base(data)
        {
        }

        public P04MyInfo()
        {
        }

        public string Ip { get; set; }
        public short Port { get; set; }
        public string Name { get; set; }

        #region Overrides of PacketModel

        public override bool Deserializer()
        {
            Ip = ReadString(16);
            Port = ReadInt16();
            Name = ReadString(32);
            return true;
        }

        #region Overrides of PacketModel

        public override byte[] Serializer()
        {
            WriteString(Ip, 16);
            WriteInt16(Port);
            WriteString(Name, 32);
            return base.Serializer();
        }

        #endregion

        #endregion
    }
}