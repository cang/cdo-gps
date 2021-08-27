#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Model
// TIME CREATE : 1:18 PM 18/12/2016
// FILENAME: Class1.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.IO;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;

#endregion

namespace Datacenter.Model.Log
{
    /// <summary>
    ///     báo cáo quá tốc độ theo thông tư 09
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    [Serializable]
    public class OverSpeedLog09 : IDbLog , ISerializerModal
    {
        /// <summary>
        ///     id
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        /// <summary>
        ///     serial thiết bị
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        /// <summary>
        ///     Id đội xe
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }

        /// <summary>
        ///     id tài xế
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long DriverId { get; set; }

        /// <summary>
        ///     id công ty
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     tốc độ giới hạn của cung đường
        /// </summary>
        [BasicColumn]
        public virtual int LimitSpeed { get; set; }

        /// <summary>
        ///     vận tốc tối đa của thiết bị trong quá trinh quá vận tốc
        /// </summary>
        [BasicColumn]
        public virtual int MaxSpeed { get; set; }

        /// <summary>
        ///     vận tốc trung bình của thiết bị trong quá trinh quá vận tốc
        /// </summary>
        [BasicColumn]
        public virtual int AverageSpeed { get; set; }

        /// <summary>
        ///     định danh device để đổi thiết bị
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual Guid Indentity { get; set; }

        /// <summary>
        ///     thời gian bắt đầu
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual DateTime BeginTime { get; set; }

        /// <summary>
        ///     thời gian kết thúc
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        ///     vị trí bắt đầu
        /// </summary>
        [ComponentColumn(Index = 0)]
        public virtual GpsLocation BeginPoint { get; set; }

        /// <summary>
        ///     vị trí kết thúc
        /// </summary>
        [ComponentColumn(Index = 1)]
        public virtual GpsLocation EndPoint { get; set; }

        /// <summary>
        ///     tổng tốc độ của record
        /// </summary>
        public virtual int TotalSpeed { get; set; }

        /// <summary>
        ///     thời gian chạy quá tốc độ, tính bằng giây
        /// </summary>
        [BasicColumn]
        public virtual int TotalTimeOver { get; set; }

        /// <summary>
        ///     khoảng cách chạy quá tố độ tính bằng mét
        /// </summary>
        [BasicColumn]
        public virtual int TotalDistance { get; set; }

        /// <summary>
        ///     số record quá tốc độ
        /// </summary>
        public virtual int CountSpeed { get; set; }

        public virtual void FixNullObject()
        {
            BeginTime = BeginTime.Fix();
            EndTime = EndTime.Fix();
        }

        public virtual void Deserializer(BinaryReader stream, int version)
        {
            Id = stream.ReadInt64();
            Serial = stream.ReadInt64();
            GroupId = stream.ReadInt64();
            DriverId = stream.ReadInt64();
            CompanyId = stream.ReadInt64();
            LimitSpeed = stream.ReadInt32();
            MaxSpeed = stream.ReadInt32();
            AverageSpeed = stream.ReadInt32();
            Indentity = Guid.Parse(stream.ReadString());
            BeginTime = DateTime.FromBinary(stream.ReadInt64());
            BeginPoint = new GpsLocation();
            BeginPoint.Lat = stream.ReadSingle();
            BeginPoint.Lng = stream.ReadSingle();
            TotalSpeed = stream.ReadInt32();
            TotalTimeOver = stream.ReadInt32();
            TotalDistance = stream.ReadInt32();
            CountSpeed = stream.ReadInt32();
        }

        public virtual void Serializer(BinaryWriter stream)
        {
            stream.Write(Id);
            stream.Write(Serial);
            stream.Write(GroupId);
            stream.Write(DriverId);
            stream.Write(CompanyId);
            stream.Write(LimitSpeed);
            stream.Write(MaxSpeed);
            stream.Write(AverageSpeed);
            stream.Write(Indentity.ToString());
            stream.Write(BeginTime.ToBinary());
            stream.Write(BeginPoint?.Lat??0f);
            stream.Write(BeginPoint?.Lng??0f);
            stream.Write(TotalSpeed);
            stream.Write(TotalTimeOver);
            stream.Write(TotalDistance);
            stream.Write(CountSpeed);
        }

        [BasicColumn]
        public virtual int DbId { get; set; }
    }
}