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
    public class GeneralReportLog : IDbLog , ISerializerModal
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
        ///     số lần quá tốc độ
        /// </summary>
        [BasicColumn]
        public virtual int InvalidSpeedCount { get; set; }

        /// <summary>
        ///     số lần dừng đỗ
        /// </summary>
        [BasicColumn]
        public virtual int PauseCount { get; set; }

        /// <summary>
        ///     số lần mở cửa
        /// </summary>
        [BasicColumn]
        public virtual int OpenDoorCount { get; set; }

        /// <summary>
        ///     số lần mở máy lạnh
        /// </summary>
        [BasicColumn]
        public virtual int OnAirMachineCount { get; set; }

        /// <summary>
        ///     km lái xe trong ngày (đơn vị mét)
        /// </summary>
        [BasicColumn]
        public virtual long KmOnDay { get; set; }

        /// <summary>
        ///     nhiên liệu đầu ngày chạy
        /// </summary>
        [BasicColumn]
        public virtual float BeginDateFuel { get; set; }

        /// <summary>
        ///     nhiên liệu thêm vào trong ngày
        /// </summary>
        [BasicColumn]
        public virtual float AddFuel { get; set; }

        /// <summary>
        ///     nhiên liệu thất thoát trong ngày
        /// </summary>
        [BasicColumn]
        public virtual float LostFuel { get; set; }

        /// <summary>
        ///     nhiên liệu thất còn lại
        /// </summary>
        [BasicColumn]
        public virtual float RemainFuel { get; set; }

        /// <summary>
        ///     thời gian lái xe trong ngày, tính bằng phút
        /// </summary>
        [BasicColumn]
        public virtual int OverTimeInday { get; set; }

        /// <summary>
        ///     số lần quá thời gian trong ngày >10h
        /// </summary>
        [BasicColumn]
        public virtual int OverTimeIndayCount { get; set; }

        /// <summary>
        ///     số lần quá thời gian liên tục trong ngày
        /// </summary>
        [BasicColumn]
        public virtual int InvalidOverTimeCount { get; set; }

        //[BasicColumn]
        //public virtual bool UseGuest { get; set; }

        ///// <summary>
        /////     thời gian lái xe chạy có khách trong ngày (tính bằng giây)
        ///// </summary>
        //[BasicColumn]
        //public virtual int GuestTimeInday { get; set; }


        ///// <summary>
        /////     khoảng cách lái xe có khách trong ngày (đơn vị mét)
        ///// </summary>
        //[BasicColumn]
        //public virtual int KmGuestOnDay { get; set; }

        /// <summary>
        ///     thời gian báo cáo ( thời gian này luôn lúc 0 giờ và tính cho cả ngày)
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
            //DbId = stream.ReadInt32();
            //Id = stream.ReadInt64();
            //GuidId = Guid.Parse(stream.ReadString());
            //CompanyId = stream.ReadInt64();
            InvalidSpeedCount = stream.ReadInt32();
            PauseCount = stream.ReadInt32();
            OpenDoorCount = stream.ReadInt32();
            KmOnDay = stream.ReadInt64();
            //KmCoOnDay = stream.ReadInt64();
            BeginDateFuel = stream.ReadSingle();
            AddFuel = stream.ReadSingle();

            if(version>=4)
            {
                LostFuel = stream.ReadSingle();
                if (LostFuel < 0) LostFuel = -LostFuel;
                RemainFuel = stream.ReadSingle();
            }

            OverTimeInday = stream.ReadInt32();
            OverTimeIndayCount = stream.ReadInt32();
            InvalidOverTimeCount = stream.ReadInt32();
            UpdateTime = DateTime.FromBinary(stream.ReadInt64());

            if (version >= 5 && version < 7) //read and ignore old data
            {
                bool UseGuest = stream.ReadBoolean();
                if (UseGuest)
                {
                    //GuestTimeInday = 
                        stream.ReadInt32();
                    //KmGuestOnDay = 
                        stream.ReadInt32();

                    ////fix for wrong old data
                    //if (KmGuestOnDay > KmOnDay) KmGuestOnDay = (int)KmOnDay;
                    //if (GuestTimeInday > OverTimeInday * 60) GuestTimeInday = OverTimeInday * 60;
                }
            }
        }

        public virtual void Serializer(BinaryWriter stream)
        {
            //stream.Write(DbId);
            //stream.Write(Id);
            //stream.Write(GuidId.ToString());
            //stream.Write(CompanyId);
            stream.Write(InvalidSpeedCount);
            stream.Write(PauseCount);
            stream.Write(OpenDoorCount);
            stream.Write(KmOnDay);
            //stream.Write(KmCoOnDay);
            stream.Write(BeginDateFuel);
            stream.Write(AddFuel);
            stream.Write(LostFuel);//version 4
            stream.Write(RemainFuel);//version 4
            stream.Write(OverTimeInday);
            stream.Write(OverTimeIndayCount);
            stream.Write(InvalidOverTimeCount);
            UpdateTime.Fix(); stream.Write(UpdateTime.ToBinary());

            //stream.Write(UseGuest);
            //if (UseGuest)
            //{
            //    stream.Write(GuestTimeInday);
            //    stream.Write(KmGuestOnDay);
            //}

        }

    }
}