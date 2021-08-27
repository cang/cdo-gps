using System;
using System.Linq;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table]
    public class SpecialTour : IEntity //, ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        /// <summary>
        /// Mã số xe
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        /// <summary>
        /// Ngày dự kiến : không xét giờ hoặc lúc 0 giờ
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual DateTime Date { get; set; }

        /// <summary>
        ///     Đi ngoài/lần
        /// </summary>
        [BasicColumn]
        public virtual String HowTimes { get; set; }

        /// <summary>
        ///     Đi tỉnh/địa điểm
        /// </summary>
        [BasicColumn]
        public virtual String Address { get; set; }

        /// <summary>
        ///     KM dự kiến
        /// </summary>
        [BasicColumn]
        public virtual int KmOnPlan { get; set; }

        /// <summary>
        ///     Ghi chú
        /// </summary>
        [BasicColumn]
        public virtual String Note { get; set; }

        /// <summary>
        /// Thời gian ghi
        /// </summary>
        [BasicColumn]
        public virtual DateTime UpdateTime { get; set; }

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


        public virtual void FixNullObject()
        {
            Date = Date.Fix();
            UpdateTime = UpdateTime.Fix();
        }

    }
}