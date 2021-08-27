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
    public class ElBusSchedule : IEntity
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual int id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long company_id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long group_id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual int approved_by { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual int drived_by { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual int created_by { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual int route_id { get; set; }

        [BasicColumn()]
        public virtual String type { get; set; }

        [BasicColumn()]
        public virtual float km { get; set; }

        [BasicColumn()]
        public virtual int time { get; set; }

        [BasicColumn()]
        public virtual DateTime time_start { get; set; }

        [BasicColumn()]
        public virtual DateTime time_end { get; set; }

        [BasicColumn()]
        public virtual float price { get; set; }

        [BasicColumn()]
        public virtual bool active { get; set; }

        [BasicColumn()]
        public virtual DateTime created_at { get; set; } = DateTime.Now;

        [BasicColumn()]
        public virtual String note { get; set; }

        public void FixNullObject()
        {
        }
    }
}
