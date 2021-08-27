using System;
using System.IO;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Components
{
    /// <summary>
    /// thông tin tọa độ
    /// </summary>
    /// 
    [Serializable()]
    public class GpsLocation : ISerializerModal
    {
        /// <summary>
        /// vĩ độ
        /// </summary>
        [BasicColumn]
        public virtual float Lat { get; set; }
        /// <summary>
        /// kinh độ
        /// </summary>
        [BasicColumn]
        public virtual float Lng { get; set; }
        /// <summary>
        /// địa chỉ
        /// </summary>
        [BasicColumn]
        public virtual string Address { get; set; }


        public static double operator -(GpsLocation p1, GpsLocation p2)
        {
            var d = p1.Lat * 0.017453292519943295;
            var num3 = p1.Lng * 0.017453292519943295;
            var num4 = p2.Lat * 0.017453292519943295;
            var num5 = p2.Lng * 0.017453292519943295;
            var num6 = num5 - num3;
            var num7 = num4 - d;
            var num8 = Math.Pow(Math.Sin(num7 / 2.0), 2.0) + Math.Cos(d) * Math.Cos(num4) * Math.Pow(Math.Sin(num6 / 2.0), 2.0);
            var num9 = 2.0 * Math.Atan2(Math.Sqrt(num8), Math.Sqrt(1.0 - num8));
            return Math.Abs(6376500.0 * num9);
        }

        public void Deserializer(BinaryReader stream,int version)
        {
            Lat = stream.ReadSingle();
            Lng = stream.ReadSingle();
            Address = stream.ReadString();
        }

        public void Serializer(BinaryWriter stream)
        {
            stream.Write(Lat);
            stream.Write(Lng);
            stream.Write(Address??String.Empty);
        }

        public bool IsEmptyAddress
        {
            get
            {
                return String.IsNullOrEmpty(Address);
            }
        }

        public bool IsValid()
        {
            return (Lat >= -90 && Lat <= 90 
                && Lng >= -180 && Lng <= 180);
        }

    }
}

