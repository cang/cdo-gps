#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : NodeBasePacket.cs
// Time Create : 10:39 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace CorePacket.Models
{
    public class NodeBasePacket : IDevicePacket
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