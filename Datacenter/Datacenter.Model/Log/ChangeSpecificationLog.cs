#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : ChangeSpecificationLog.cs
// Time Create : 8:21 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Log
{
    /// <summary>
    ///     bảng log thay đổi giá trị bảo trì
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class ChangeSpecificationLog:IDbLog
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        /// <summary>
        ///     tra các trường hợp trong class BasicUtils
        /// </summary>
        [BasicColumn(Name = "OptionName")]
        public virtual OptionNameType OptionName { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime TimeUpdate { get; set; }

        /// <summary>
        /// km thời điểm reset hạn mức
        /// </summary>
        [BasicColumn]
        public virtual long KmReset { get; set; }

        public virtual void FixNullObject()
        {
            TimeUpdate = DateTime.Now;
        }

        [BasicColumn]
        public virtual int DbId { get; set; }
    }
}