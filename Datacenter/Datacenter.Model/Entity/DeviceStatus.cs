#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DeviceStatus.cs
// Time Create : 9:35 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.IO;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;

namespace Datacenter.Model.Entity
{
    [Table]
    public class DeviceStatus : IEntity, ISerializerModal
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Foreign, ForeignKey = "Device")]
        public virtual long Serial { get; set; }

        [HasOneColumn(Type = HasOneType.Child)]
        public virtual Device Device { get; set; }

        [ComponentColumn]
        public virtual DeviceStatusInfo BasicStatus { get; set; }

        [ComponentColumn]
        public virtual DriverStatusInfo DriverStatus { get; set; }

        [BasicColumn]
        public virtual long LastTotalKmUsingOnDay { get; set; }
        [BasicColumn]
        public virtual short PauseCount { get; set; }

        /// <summary>
        /// KHông thấy sử dụng, dường như nó tương tự LastTotalKmUsingOnDay
        /// </summary>
        [BasicColumn]
        public virtual long TotalKmBeginDate { get; set; }


        #region Implementation of IEntity

        /// <summary>
        ///     sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            if (BasicStatus == null)
            {
                BasicStatus = new DeviceStatusInfo();
            }
            BasicStatus.FixNullObject();

            if (DriverStatus == null)
            {
                DriverStatus = new DriverStatusInfo();
                
            }
            DriverStatus.FixNullObject();
        }

        #endregion

        public virtual void Serializer(BinaryWriter stream)
        {
            stream.Write(Serial);
            if (BasicStatus != null)
            {
                stream.Write(true);
                BasicStatus.Serializer(stream);
            }
            else
                stream.Write(false);

            if (DriverStatus != null)
            {
                stream.Write(true);
                DriverStatus.Serializer(stream);
            }
            else
                stream.Write(false);

            stream.Write(LastTotalKmUsingOnDay);
            stream.Write(PauseCount);
            stream.Write(TotalKmBeginDate);
        }

        public virtual void Deserializer(BinaryReader stream, int version)
        {
            Serial = stream.ReadInt64();
            if (stream.ReadBoolean())
            {
                BasicStatus = new DeviceStatusInfo();
                BasicStatus.Deserializer(stream, version);
            }
            if (stream.ReadBoolean())
            {
                DriverStatus = new DriverStatusInfo();
                DriverStatus.Deserializer(stream, version);
            }

            LastTotalKmUsingOnDay = stream.ReadInt64();
            PauseCount = stream.ReadInt16();
            TotalKmBeginDate = stream.ReadInt64();
        }


    }
}