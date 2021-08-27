using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Entity
{
    /// <summary>
    /// các thông số cài đặt của công ty/ đại lý
    /// </summary>
    [Table]
    public class CompanySetting:IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Foreign,ForeignKey = "Company")]
        public virtual long Id { get; set; }
        [HasOneColumn(Type = HasOneType.Child)]
        public virtual Company Company { get; set; }

        [BasicColumn]
        public virtual int TimeoutHidenDevice { get; set; }

        /// <summary>
        ///     thời gian tính xe mất liên lạc, mặc định 2h, tính theo phút
        /// </summary>
        [BasicColumn]
        public virtual int TimeoutLostDevice { get; set; } = 120;
        public virtual void FixNullObject()
        {
            if (Company == null) throw new Exception("CompanySetting : Thông tin công ty null");
            if (TimeoutLostDevice == 0) TimeoutLostDevice = 120;
            if (TimeoutHidenDevice == 0) TimeoutHidenDevice = 10080;
        }
    }
}