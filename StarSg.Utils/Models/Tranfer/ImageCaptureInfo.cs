using System;
using System.IO;
using System.Runtime.Serialization;

namespace Core.Models.Tranfer
{
    /// <summary>
    ///     thông tin hình ảnh sau khi chụp xong
    /// </summary>
    [DataContract]
    public class ImageCaptureInfo : IDisposable
    {
        private readonly MemoryStream _stream = new MemoryStream();
        private BinaryWriter _bwrite;

        /// <summary>
        ///     serial
        /// </summary>
        [DataMember]
        public string Serial { get; set; }

        /// <summary>
        ///     Dữ liệu
        /// </summary>
        [DataMember]
        public byte[] Data { get; set; }

        /// <summary>
        ///     id của camera
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        ///     thời gian chụp
        /// </summary>
        [DataMember]
        public DateTime Time { get; set; }

        /// <summary>
        ///     chứa thông tin Id của hàng đợi timeout ( không cần quan tâm nó)
        /// </summary>
        public long DelayId { get; set; }

        public void Push(byte[] data)
        {
            if (_bwrite == null) _bwrite = new BinaryWriter(_stream);
            _bwrite.Write(data);
            Data = _stream.ToArray();
        }

        public bool Complete(int len)
        {
            return _stream.Length >= len;
        }

        public void Dispose()
        {
            _bwrite?.Dispose();
            _stream?.Dispose();
        }

    }
}