#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : P03YourInfo.cs
// Time Create : 4:15 PM 28/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Core.Models.Packets
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(3)]
    public class P03YourInfo : NodeSharePacketModel
    {
        public P03YourInfo(byte[] data) : base(data)
        {
        }

        public P03YourInfo()
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