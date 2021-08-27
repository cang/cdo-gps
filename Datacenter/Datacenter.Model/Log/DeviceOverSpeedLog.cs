using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Log
{
    [Table]
    public class DeviceOverSpeedLog:IDbLog
    {
        /// <summary>
        ///     id
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        /// <summary>
        /// mã định danh của thiết bị
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual Guid Indentity { get; set; }
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
        ///     vị trí đạt vận tốc tối đa
        /// </summary>
        [ComponentColumn]
        public virtual GpsLocation Point { get; set; }

        //[ComponentColumn]
        [BasicColumn]
        public virtual int Distance { get; set; }

        #region Implementation of IEntity

        /// <summary>
        ///     sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            BeginTime = BeginTime.Fix();
            EndTime = EndTime.Fix();
        }

        #endregion

        #region Implementation of IDbLog

        /// <summary>
        ///     Id của máy chủ lưu trữ dữ liệu log
        /// </summary>

        [BasicColumn]
        public virtual int DbId { get; set; }

        #endregion 
    }
}