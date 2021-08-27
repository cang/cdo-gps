#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : PacketFactory.cs
// Time Create : 3:22 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.IO;
using NodeServerPlatform.Core.Models.Packets;

namespace NodeServerPlatform.Core
{
    /// <summary>
    ///     Lớp đóng gói và tạo các gói tin trao đổi
    ///     Sẽ được sử dụng ở lớp cuối cùng truyền nhận dữ liệu TCP/IP
    /// </summary>
    public static class NodeSharePacketFactory
    {
        /// <summary>
        ///     Tạo mới thông tin của packet từ một chuỗi byte
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static INodeSharePacket CreatePacket(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var bRead = new BinaryReader(memory))
                {
                    var opcode = bRead.ReadInt16();
                    var len = bRead.ReadInt16();
                    var buf = bRead.ReadBytes(len);
                    return new NodeBasePacket(opcode, buf);
                }
            }
        }

        /// <summary>
        ///     Tạo 1 stream từ thông tin packet để truyền qua client
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static byte[] CreateStream(INodeSharePacket p)
        {
            using (var memory = new MemoryStream())
            {
                using (var bWrite = new BinaryWriter(memory))
                {
                    bWrite.Write((short)p.Opcode);
                    bWrite.Write((short)p.Data.Length);
                    bWrite.Write(p.Data);
                    return memory.ToArray();
                }
            }
        }

    }
}