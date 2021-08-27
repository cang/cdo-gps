using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table]
    public class DeviceSimInfo : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Foreign, ForeignKey = "Device")]
        public virtual long Serial { set; get; }

        [HasOneColumn(Type = HasOneType.Child)]
        public virtual Device Device { set; get; }

        [BasicColumn]
        public virtual string Phone { set; get; }

        [BasicColumn]
        public virtual string Money { set; get; }

        [BasicColumn]
        public virtual DateTime PhoneUpdate { set; get; }

        [BasicColumn]
        public virtual DateTime MoneyUpdate { set; get; }

        [BasicColumn]
        public virtual bool SimNgoai { set; get; }

        public virtual void FixNullObject()
        {
            PhoneUpdate = PhoneUpdate.Fix();
            MoneyUpdate = MoneyUpdate.Fix();
        }
    }
}