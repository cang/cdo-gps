#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : PacketModel.cs
// Time Create : 2:40 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.IO;
using System.Text;

namespace NodeServerPlatform.Core
{
    public abstract class NodeSharePacketModel : IDisposable, INodeShareSendPacket, INodeShareRecvPacket
    {
        private readonly MemoryStream _memory;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _write;

        protected NodeSharePacketModel()
        {
            _memory = new MemoryStream();
            _reader = new BinaryReader(_memory);
            _write = new BinaryWriter(_memory);
        }

        protected NodeSharePacketModel(byte[] data)
        {
            _memory = new MemoryStream(data);
            _reader = new BinaryReader(_memory);
            _write = new BinaryWriter(_memory);
        }

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _write?.Dispose();
            _reader?.Dispose();
            _memory?.Dispose();
        }

        #endregion

        #region Implementation of IRecvPacket

        public abstract bool Deserializer();

        #endregion

        #region Implementation of ISendPacket

        public virtual byte[] Serializer()
        {
            return _memory.ToArray();
        }

        #endregion

        protected int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        protected short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        protected long ReadInt64()
        {
            return _reader.ReadInt64();
        }

        protected uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        protected ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        protected ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        protected double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        protected float ReadFloat()
        {
            return _reader.ReadSingle();
        }

        protected bool ReadBool()
        {
            return ReadByte() != 0;
        }

        protected string ReadString(int len)
        {
            var tmp = ReadBytes(len);
            return Encoding.ASCII.GetString(tmp).Replace("\0", "");
        }

        protected string ReadString(int len, Encoding endcode)
        {
            var tmp = ReadBytes(len);
            return endcode.GetString(tmp).Replace("\0", "");
        }

        protected byte[] ReadBytes(int len)
        {
            return _reader.ReadBytes(len);
        }

        protected byte ReadByte()
        {
            return _reader.ReadByte();
        }


        protected void WriteInt32(int val)
        {
            _write.Write(val);
        }

        protected void WriteInt16(int val)
        {
            _write.Write((short) val);
        }

        protected void WriteInt64(long val)
        {
            _write.Write(val);
        }

        protected void WriteUInt64(ulong val)
        {
            _write.Write(val);
        }

        protected void WriteUInt32(uint val)
        {
            _write.Write(val);
        }

        protected void WriteUInt16(uint val)
        {
            _write.Write((ushort) val);
        }

        protected void WriteDouble(double val)
        {
            _write.Write(val);
        }

        protected void WriteFloat(float val)
        {
            _write.Write(val);
        }

        protected void WriteString(string val)
        {
            var tmp = Encoding.ASCII.GetBytes(val);
            WriteBytes(tmp);
        }

        protected void WriteString(string val, int len)
        {
            if (len <= 0) throw new InvalidDataException();
            var tmp = Encoding.ASCII.GetBytes(val);
            var buf = new byte[len];
            Buffer.BlockCopy(tmp, 0, buf, 0, tmp.Length >= len ? len : tmp.Length);
            WriteBytes(buf);
        }

        protected void WriteString(string val, Encoding endcode)
        {
            var tmp = endcode.GetBytes(val);
            WriteBytes(tmp);
        }

        protected void WriteByte(byte val)
        {
            _write.Write(val);
        }

        protected void WriteBytes(byte[] val)
        {
            _write.Write(val);
        }

        protected void WriteBool(bool val)
        {
            WriteByte((byte) (val ? 1 : 0));
        }
    }
}