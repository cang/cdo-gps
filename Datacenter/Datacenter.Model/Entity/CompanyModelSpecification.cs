#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : CompanyModelSpecification.cs
// Time Create : 8:07 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Entity
{
    /// <summary>
    ///     bảng giá trị bảo trì thiết bị theo công ty
    /// </summary>
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class CompanyModelSpecification:IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn]
        public virtual string Name { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     Số km đảo lốp
        /// </summary>
        [BasicColumn]
        public virtual long KmDaoLop { get; set; }

        /// <summary>
        ///     Số km thay vỏ
        /// </summary>
        [BasicColumn]
        public virtual long KmThayVo { get; set; }

        /// <summary>
        ///     số km thay lọc nhớt
        /// </summary>
        [BasicColumn]
        public virtual long KmThayNhot { get; set; }

        /// <summary>
        ///     số km thay lọc dầu
        /// </summary>
        [BasicColumn]
        public virtual long KmThayLocDau { get; set; }

        /// <summary>
        ///     số km thay lọc gió
        /// </summary>
        [BasicColumn]
        public virtual long KmThayLocGio { get; set; }

        /// <summary>
        ///     số km thay lọc nhớt
        /// </summary>
        [BasicColumn]
        public virtual long KmThayLocNhot { get; set; }

        public virtual void FixNullObject()
        {
        }
    }
}