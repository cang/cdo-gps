using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datacenter.Model.Entity
{
    [Table]
    public class ElBusPrice : IEntity //, ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual int id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long company_id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long group_id { get; set; }

        [BasicColumn]
        public virtual float price_by_km { get; set; }

        [BasicColumn]
        public virtual float price_by_time { get; set; }


        public void FixNullObject()
        {
        }
    }
}
