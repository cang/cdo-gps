#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DeviceModel.cs
// Time Create : 1:54 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Entity
{
    [Table]
    public class DeviceModel:IEntity,ICacheModel
    {

        /// <summary>
        ///     Tên model
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual string Name { get; set; }

        /// <summary>
        ///  số ghê hoặc trọng tải
        /// </summary>
        [BasicColumn]
        public virtual string Sheat { get; set; }

        /// <summary>
        ///     dung tích xilanh
        /// </summary>
        [BasicColumn]
        public virtual int Xilanh { get; set; }

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