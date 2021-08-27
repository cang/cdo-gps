using System.ComponentModel.Composition;
using CorePacket;
using CorePacket.Utils;

namespace DevicePacketModels
{
    public enum ResponseStatus:byte
    {
        Error=0,
        Ok=1,
        NotFound=2,
        InvalidOpCode=3,
        InvalidData=4,
        WrongChecksum=5,
        NotImplement=100
    }
    [DeviceOpCode(2)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class P02ResponsePacket:DevicePacketModel
    {
        
        /// <summary>
        /// thông tin xử lý  : 0: Lỗi , 1 :OK ,2 :Chưa khai báo
        /// </summary>
        public ResponseStatus Status { get; set; }
        public override bool Deserializer()
        {
            var tmp = ReadByte();
            switch (tmp)
            {
                case 0:
                    Status=ResponseStatus.Error;
                    break;
                case 1:
                    Status=ResponseStatus.Ok;
                    break;
                case 2:
                    Status=ResponseStatus.NotFound;
                    break;
                case 3:
                    Status=ResponseStatus.InvalidOpCode;
                    break;
                default:
                    Status=ResponseStatus.NotImplement;
                    break;
            }
            return true;
        }

        public override byte[] Serializer()
        {
            var tmp = (byte) Status;
            WriteByte(tmp);
            return base.Serializer();
        }

        public P02ResponsePacket(byte[] data) : base(data)
        {
        }

        public P02ResponsePacket()
        {
        }
        public P02ResponsePacket(ResponseStatus st):this()
        {
            Status = st;
        }
    }
}