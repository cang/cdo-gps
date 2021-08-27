#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : S01GetListNeighbor.cs
// Time Create : 4:29 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Core.Models.Packets
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(2)]
    public class P02ListNeighbor : NodeSharePacketModel
    {
        public P02ListNeighbor(byte[] data) : base(data)
        {
        }

        public P02ListNeighbor()
        {
        }

        public IDictionary<string, NeighborInfo> Neighbors { get; } = new Dictionary<string, NeighborInfo>();

        #region Overrides of RecvPacket

        public override byte[] Serializer()
        {
            WriteInt32(Neighbors.Count);
            foreach (var neighborInfo in Neighbors)
            {
                WriteString(neighborInfo.Value.Name, 32);
                WriteString(neighborInfo.Value.Ip, 16);
                WriteInt16(neighborInfo.Value.Port);
            }
            return base.Serializer();
        }

        public override bool Deserializer()
        {
            var len = ReadInt32();
            for (var i = 0; i < len; i++)
            {
                var name = ReadString(32);
                var ip = ReadString(16);
                var port = ReadInt16();
                if (!Neighbors.ContainsKey(name))
                    Neighbors.Add(name, new NeighborInfo {Ip = ip, Name = name, Port = port});
            }

            return true;
        }

        #endregion
    }
}