#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : PointAreaTraceLog.cs
// Time Create : 1:53 PM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Log
{
    [Table]
    public class PointTraceLog:IDbLog
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual int PointId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual DateTime BeginTime { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual DateTime EndTime { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long DeviceId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long DriverId { get; set; }

        #region Implementation of IEntity

        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
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