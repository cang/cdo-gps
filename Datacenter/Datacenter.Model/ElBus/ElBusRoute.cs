using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;
using System;

namespace Datacenter.Model.Entity
{
    [Table]
    public class ElBusRoute : IEntity //, ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual int id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long company_id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long group_id { get; set; }

        [BasicColumn]
        public virtual string name { get; set; }

        [BasicColumn()]//Length = 2048)]
        public virtual byte[] data { set; get; }

        [BasicColumn]
        public virtual float  km { get; set; }

        [BasicColumn]
        public virtual float price { get; set; }

        [BasicColumn]
        public virtual DateTime created_at { get; set; }

        [BasicColumn]
        public virtual string note { get; set; }

        public virtual void FixNullObject()
        {
            created_at = created_at.Fix();
        }
    }


}
