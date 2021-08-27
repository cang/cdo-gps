#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : P01GetListNeighbor.cs
// Time Create : 3:09 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Core.Models.Packets
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(1)]
    public class P01GetListNeighbor : NodeSharePacketModel
    {
        public P01GetListNeighbor(byte[] data) : base(data)
        {
        }

        public P01GetListNeighbor()
        {
        }

        #region Overrides of PacketModel

        public override bool Deserializer()
        {
            return true;
        }

        #endregion
    }
}