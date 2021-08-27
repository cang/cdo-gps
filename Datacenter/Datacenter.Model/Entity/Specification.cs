#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : Specification.cs
// Time Create : 8:50 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Entity
{
    [Table]
    public class Specification:IEntity
    {
        /// <summary>
        ///     số serial thiết bị
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Foreign, ForeignKey = "Device")]
        public virtual long Serial { set; get; }

        /// <summary>
        ///     thông tin thiết bị
        /// </summary>
        [HasOneColumn(Type = HasOneType.Child)]
        public virtual Device Device { get; set; }

        /// <summary>
        /// 
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

        #region Implementation of IEntity

        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {

        }

        #endregion 
    }
}