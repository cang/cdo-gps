using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Log
{
    [Table]
    public class FuelTraceLog : IDbLog
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long DeviceId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long DriverId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime Time { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime TimeBegin { get; set; }

        /// <summary>
        /// Vị trí thực hiện
        /// </summary>
        [ComponentColumn]
        public virtual GpsLocation Location { set; get; }

        /// <summary>
        /// Giá trị hiện hành (ml)
        /// </summary>
        [BasicColumn]
        public virtual int CurrentValue { set; get; }

        /// <summary>
        /// Giá trị đã thay đổi (ml)
        /// </summary>
        [BasicColumn]
        public virtual int Delta { set; get; }

        #region Implementation of IEntity

        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            Time = Time.Fix();
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