#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DriverStatusInfo.cs
// Time Create : 10:10 AM 23/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.IO;
using DaoDatabase;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Components
{
    [Serializable()]
    public class DriverStatusInfo : ISerializerModal
    {

        [BasicColumn]
        public virtual long DriverId { get; set; }
        [BasicColumn]
        public virtual string Name { get; set; }
        [BasicColumn]
        public virtual string Gplx { get; set; }
        [BasicColumn]
        public virtual DateTime TimeBeginWorkInSession { get; set; }
        [BasicColumn]
        public virtual int TimeWorkInDay { set; get; }
        [BasicColumn]
        public virtual int TimeWork { set; get; }
        public virtual DateTime LastUpdateOverSpeed { get; set; }
        public virtual int TimeWorkInSession => (int) (DateTime.Now - TimeBeginWorkInSession).TotalMinutes;
        public virtual int OverSpeedCount { get; set; }

        #region Implementation of IEntity

        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            TimeBeginWorkInSession = TimeBeginWorkInSession.Fix();
        }

        #endregion

        public void Deserializer(BinaryReader stream, int version)
        {
            DriverId = stream.ReadInt64();
            Name = stream.ReadString();
            Gplx = stream.ReadString();
            TimeBeginWorkInSession = DateTime.FromBinary(stream.ReadInt64());
            TimeWorkInDay = stream.ReadInt32();
            TimeWork = stream.ReadInt32();
            LastUpdateOverSpeed = DateTime.FromBinary(stream.ReadInt64());
            OverSpeedCount = stream.ReadInt32();
        }

        public void Serializer(BinaryWriter stream)
        {
            stream.Write(DriverId);
            stream.Write(Name??String.Empty);
            stream.Write(Gplx ?? String.Empty);
            stream.Write(TimeBeginWorkInSession.ToBinary());
            stream.Write(TimeWorkInDay);
            stream.Write(TimeWork);
            stream.Write(LastUpdateOverSpeed.ToBinary());
            stream.Write(OverSpeedCount);
        }

    }
}