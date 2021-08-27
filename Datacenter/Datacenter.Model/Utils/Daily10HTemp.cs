using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datacenter.Model.Utils
{
    /// <summary>
    /// ghi lại thông tin ghi nhận quá 10 trong ngày
    /// 1/ Báo cáo ra table khác
    ///2/ bắt đầu khi gói 114 xuất hiện, xét khi qua 0 giờ.
    ///3/ kết thúc khi gói 114 xuất hiện, xét khi qua 0 giờ.
    ///4/ Khi đổi tài xế
    /// </summary>
    public class Daily10HTemp : ISerializerModal
    {
        /// <summary>
        /// Mã số tài xế hiện tại (có thể lấy từ Driver sẳng có)
        /// </summary>
        //public virtual int DriverId { get; set; }

        /// <summary>
        /// Thời gian bắt đầu tính cho cuốc quá 10h trong ngày
        /// </summary>
        public virtual DateTime BeginTime { get; set; } = DateTime.Now;

        /// <summary>
        /// vĩ độ
        /// </summary>
        public virtual float Lat { get; set; }
        /// <summary>
        /// kinh độ
        /// </summary>
        public virtual float Lng { get; set; }


        /// <summary>
        /// Tổng số giây trước đó
        /// </summary>
        public virtual int TotalSeconds { get; set; }


        public void Deserializer(BinaryReader stream, int version)
        {
            BeginTime = DateTime.FromBinary(stream.ReadInt64());
            Lat = stream.ReadSingle();
            Lng = stream.ReadSingle();
            TotalSeconds = stream.ReadInt32();
        }

        public void Serializer(BinaryWriter stream)
        {
            stream.Write(BeginTime.ToBinary());
            stream.Write(Lat);
            stream.Write(Lng);
            stream.Write(TotalSeconds);
        }

    }
}
