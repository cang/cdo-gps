#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : Driver.cs
// Time Create : 3:42 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table]
    public class Driver : IEntity, ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        /// <summary>
        ///     tên tài xế
        /// </summary>
        [BasicColumn]
        public virtual string Name { get; set; }

        /// <summary>
        ///     giấy phép
        /// </summary>
        [BasicColumn]
        public virtual string Gplx { get; set; }

        /// <summary>
        ///     mã nhân viên
        /// </summary>
        [BasicColumn]
        public virtual string Mnv { get; set; }

        /// <summary>
        ///     id công ty
        /// </summary>
        [BasicColumn]
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     id đội xe
        /// </summary>
        [BasicColumn]
        public virtual long GroupId { get; set; }

        /// <summary>
        ///     năm sinh
        /// </summary>
        [BasicColumn]
        public virtual int Born { get; set; }

        /// <summary>
        ///     chứng minh nhân dân
        /// </summary>
        [BasicColumn]
        public virtual string Cmnd { get; set; }

        /// <summary>
        ///     địa chỉ nơi sinh
        /// </summary>
        [BasicColumn]
        public virtual string Address { get; set; }

        /// <summary>
        ///     giới tính
        /// </summary>
        [BasicColumn]
        public virtual int Sex { get; set; }

        /// <summary>
        ///     ngày cấp giấy phép lái xe
        /// </summary>
        [BasicColumn]
        public virtual DateTime CreateDateOfGplx { get; set; }

        /// <summary>
        ///     ngày hết hạn giấy phép lái xe
        /// </summary>
        [BasicColumn]
        public virtual DateTime EndDateOfGplx { get; set; }

        /// <summary>
        ///     nơi cấp giấy phép lái xe
        /// </summary>
        [BasicColumn]
        public virtual string AddressOfGplx { get; set; }

        /// <summary>
        ///     số điện thoại tài xế
        /// </summary>
        [BasicColumn]
        public virtual string Phone { get; set; }

        #region Implementation of IEntity

        /// <summary>
        ///     sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            CreateDateOfGplx.Fix();
            EndDateOfGplx.Fix();
        }

        #endregion
    }
}