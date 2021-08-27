#region

using System;
using System.IO;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

#endregion

namespace Datacenter.Model.Log
{
    /// <summary>
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class GeneralGuestLog : IDbLog , ISerializerModal
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        /// <summary>
        ///     serial thiết bị
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual Guid GuidId { get; set; }

        /// <summary>
        ///     id công ty
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     id nhóm
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }

        /// <summary>
        ///     thời gian lái xe chạy có khách trong ngày (tính bằng giây)
        /// </summary>
        [BasicColumn]
        public virtual int GuestTimeInday { get; set; }


        /// <summary>
        ///     khoảng cách lái xe có khách trong ngày (đơn vị mét)
        /// </summary>
        [BasicColumn]
        public virtual int KmGuestOnDay { get; set; }

        /// <summary>
        ///     thời gian lái xe chạy không khách trong ngày (tính bằng giây)
        /// </summary>
        [BasicColumn]
        public virtual int NoGuestTimeInday { get; set; }


        /// <summary>
        ///     khoảng cách lái xe không khách trong ngày (đơn vị mét)
        /// </summary>
        [BasicColumn]
        public virtual int KmNoGuestOnDay { get; set; }

        /// <summary>
        ///     Ghi Chú
        /// </summary>
        [BasicColumn]
        public virtual String Note { get; set; }


        /// <summary>
        ///     thời gian báo cáo ( thời gian này luôn lúc 5 giờ sáng và tính cho cả ngày)
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual DateTime UpdateTime { get; set; }

        public virtual void FixNullObject()
        {
            UpdateTime.Fix();
        }

        [BasicColumn]
        public virtual int DbId { get; set; }

        public virtual void Deserializer(BinaryReader stream, int version)
        {
            UpdateTime = DateTime.FromBinary(stream.ReadInt64());
            GuestTimeInday = stream.ReadInt32();
            KmGuestOnDay = stream.ReadInt32();
            NoGuestTimeInday = stream.ReadInt32();
            KmNoGuestOnDay = stream.ReadInt32();
        }

        public virtual void Serializer(BinaryWriter stream)
        {
            UpdateTime.Fix(); stream.Write(UpdateTime.ToBinary());
            stream.Write(GuestTimeInday);
            stream.Write(KmGuestOnDay);
            stream.Write(NoGuestTimeInday);
            stream.Write(KmNoGuestOnDay);
        }


    }
}