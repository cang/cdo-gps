#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : NodeBasePacket.cs
// Time Create : 10:39 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace NodeServerPlatform.Core.Models.Packets
{
    public class NodeBasePacket : INodeSharePacket
    {
        public NodeBasePacket(int opcode, byte[] data)
        {
            Opcode = opcode;
            Data = data;
        }

        #region Implementation of IPacket

        /// <summary>
        ///     Id của gói tin truyền
        /// </summary>
        public int Opcode { get; set; }

        /// <summary>
        ///     Nội dung của gói tin
        /// </summary>
        public byte[] Data { get; set; }

        #endregion
    }
}