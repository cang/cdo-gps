using System.Security.Cryptography.X509Certificates;
using CorePacket;

namespace ServerCore
{
    public interface IClientCachePacket
    {
        /// <summary>
        /// lấy packet ra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        byte[] Pop(long id);
        /// <summary>
        /// ghi packet vào
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        bool Push(long id, byte[] p);

        /// <summary>
        /// xóa bỏ thông tin trong cache
        /// </summary>
        /// <param name="id"></param>
        void Clear(long id);

        bool RemoveUnknownDevice(long serial);
        bool TryRemoveUnknownDevice(long serial);

        //bool ContainRawLogSerial(long serial);
        //void TrackRawLogSerial(long serial);

    }
}