#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : PacketFactory.cs
// Time Create : 3:22 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.IO;
using CorePacket.Models;
using CorePacket.Utils;

namespace CorePacket
{
    /// <summary>
    ///     Lớp đóng gói và tạo các gói tin trao đổi
    ///     Sẽ được sử dụng ở lớp cuối cùng truyền nhận dữ liệu TCP/IP
    /// </summary>
    public static class DevicePacketFactory
    {
        /// <summary>
        ///     Tạo mới thông tin của packet từ một chuỗi byte
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IDevicePacket CreatePacket(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var bRead = new BinaryReader(memory))
                {
                    var opcode = bRead.ReadInt16();
                    var len = bRead.ReadInt16();
                    var buf = bRead.ReadBytes(len);

                    //var checksum = bRead.ReadUInt32();
                    //var testchecksum = Crc32.ComputeChecksum(buf);
                    
                    if (bRead.ReadUInt32() != Crc32.ComputeChecksum(buf))
                    {
                        long serial = 0;
                        using (MemoryStream ms = new MemoryStream(buf))
                        {
                            using (BinaryReader r = new BinaryReader(ms))
                            {
                                serial = r.ReadInt64();
                            }
                        }
                        throw new ChecksumException(opcode,serial,"INVALID CHECKSUM");
                    }

                    return new NodeBasePacket(opcode, buf);
                }
            }
        }

        /// <summary>
        ///     Tạo 1 stream từ thông tin packet để truyền qua client
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static byte[] CreateStream(IDevicePacket p)
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

        public class ChecksumException : Exception
        {
            public int Opcode;
            public long Serial;
            public ChecksumException(int opcode, long serial,String msg) : base(msg)
            {
                this.Opcode = opcode;
                this.Serial = serial;
            }
        }

    }


    
}