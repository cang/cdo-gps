using System;
using System.Linq;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    /// <summary>
    /// thiết bị
    /// </summary>
    [Table]
    public class FixDevice : IEntity, ICacheModel
    {
        /// <summary>
        ///     serial thiết bị
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual long Serial { get; set; }

        [BasicColumn]
        public virtual long OriginalSerial { get; set; }

        /// <summary>
        ///     Id của thiết bị
        /// </summary>
        [BasicColumn]
        public virtual string Id { get; set; }

        /// <summary>
        ///     Thời gian khai sua thiet bi
        /// </summary>
        [BasicColumn]
        public virtual DateTime FixTime { get; set; }

        /// <summary>
        ///     Thời gian khai báo thiết bị
        /// </summary>
        [BasicColumn]
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        ///     Thời gian hết hạn
        /// </summary>
        [BasicColumn]
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        ///     Ngày đóng phí
        /// </summary>
        [BasicColumn]
        public virtual DateTime PaidFee { get; set; }

        /// <summary>
        ///     Id công ty
        /// </summary>
        [BasicColumn]
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     Id đội xe
        /// </summary>
        [BasicColumn]
        public virtual long GroupId { get; set; }

        /// <summary>
        ///     Biển số
        /// </summary>
        [BasicColumn]
        public virtual string Bs { get; set; }

        private string _ModelName;
        /// <summary>
        ///     tên loại xe
        /// </summary>
        [BasicColumn]
        public virtual string ModelName
        {
            get { return _ModelName; }
            set
            {
                _ModelName = value;
                //UpdateDeviceType();
            }
        }

        [BasicColumn()]
        public virtual string Phone { get; set; }
        [BasicColumn()]
        public virtual string Vin { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [BasicColumn]
        public virtual string Note { get; set; }

        /// <summary>
        /// Người lắp đặt
        /// </summary>
        [BasicColumn]
        public virtual string Installer { get; set; }

        /// <summary>
        /// Người quản lý/bão trì
        /// </summary>
        [BasicColumn]
        public virtual string Maintaincer { get; set; }

        /// <summary>
        /// Sổ điện thoại của chủ xe
        /// </summary>
        [BasicColumn]
        public virtual string OwnerPhone { get; set; }

        /// <summary>
        /// <summary>
        /// email của chủ xe
        /// </summary>
        [BasicColumn]
        public virtual string EmailAddess{ get; set; }


        public virtual void FixNullObject()
        {
        }

    }
}