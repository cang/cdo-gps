using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class RouteGps : IEntity, ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn]
        public virtual string Name { get; set; }

        [BasicColumn]
        public virtual int Type { get; set; }

        [BasicColumn(CustomSqlType = "varchar(max)", Length = int.MaxValue)]
        public virtual string DataPath { get; set; }

        //index is TransportType
        [BasicColumn]
        public virtual string MaxSpeed { get; set; }

        //index is TransportType
        [BasicColumn]
        public virtual string MinSpeed { get; set; }

        [BasicColumn]
        public virtual string Description { get; set; }

        [BasicColumn]
        public virtual DateTime CreateTime { get; set; }

        #region Implementation of IEntity

        public virtual void FixNullObject()
        {
            CreateTime = CreateTime.Fix();
        }

        #endregion 
    }
}