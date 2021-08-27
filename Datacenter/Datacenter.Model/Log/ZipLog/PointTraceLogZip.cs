#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : PointTraceLogZip.cs
// Time Create : 3:51 PM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Log.ZipLog
{
    [Table]
    public class PointTraceLogZip : IZip
    {
        [BasicColumn(IsIndex = true)]
        public virtual int PointId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }

        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(Length = int.MaxValue)]
        public virtual byte[] Data { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime TimeUpdate { get; set; }

        #region Implementation of IEntity

        /// <summary>
        ///     sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            TimeUpdate = TimeUpdate.Fix();
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