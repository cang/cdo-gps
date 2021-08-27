using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    /// <summary>
    ///     Quản lý thông tin công ty / đại lý
    /// </summary>
    [Table]
    public class Company : IEntity, ICacheModel
    {
        private CompanySetting _setting;

        /// <summary>
        /// id
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        /// <summary>
        /// tên công ty
        /// </summary>
        [BasicColumn]
        public virtual string Name { get; set; }
        /// <summary>
        /// tên viết tắt
        /// </summary>
        [BasicColumn]
        public virtual string ShortName { get; set; }
        /// <summary>
        /// địa chỉ
        /// </summary>
        [ComponentColumn]
        public virtual GpsLocation Location { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        [BasicColumn(Length = 1000)]
        public virtual string Description { get; set; }
        /// <summary>
        /// thời gian khai báo công ty
        /// </summary>
        [BasicColumn]
        public virtual DateTime TimeCreate { get; set; }

        /// <summary>
        /// thông tin cài đặt
        /// </summary>
        [HasOneColumn(Type = HasOneType.Parent)]
        public virtual CompanySetting Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }

        /// <summary>
        ///     database id
        /// </summary>
        [BasicColumn]
        public virtual int DbId { get; set; }
        public virtual void FixNullObject()
        {
            TimeCreate = TimeCreate.Fix();
            Setting?.FixNullObject();
        }

        
        /// <summary>
        /// 0 mac dinh, 1 : xe dien
        /// </summary>
        [BasicColumn()]
        public virtual byte Type { get; set; }


    }
}