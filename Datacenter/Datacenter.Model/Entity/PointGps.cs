#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : PointGps.cs
// Time Create : 11:14 AM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table(DbType = DbSupportType.MicrosoftSqlServer)]
    public class PointGps : IEntity, ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual int Id { get; set; }

        [BasicColumn]
        public virtual string Name { get; set; }

        [ComponentColumn(Index = 0)]
        public virtual GpsLocation Location { get; set; }

        [BasicColumn]
        public virtual int Radius { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }

        [BasicColumn]
        public virtual string Description { get; set; }

        [BasicColumn]
        public virtual DateTime CreateTime { get; set; }

        [BasicColumn]
        public virtual string Type { get; set; }

        #region Implementation of IEntity

        public virtual void FixNullObject()
        {
            CreateTime = CreateTime.Fix();
        }

        #endregion
    }
}