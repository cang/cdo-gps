#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : Area.cs
// Time Create : 8:03 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table]
    public class Area:IEntity,ICacheModel
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual int Id { get; set; }

        [BasicColumn]
        public virtual string Name { get; set; }

        [BasicColumn]
        public virtual string Description { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }

        [BasicColumn]
        public virtual int MaxSpeed { get; set; }

        [BasicColumn]
        public virtual int MaxDevice { get; set; }

        [BasicColumn(Length = int.MaxValue)]
        public virtual string Points { get; set; }

        [BasicColumn]
        public virtual DateTime CreateTime { get; set; }

        [BasicColumn]
        public virtual string Type { get; set; }

        [BasicColumn]
        public virtual string Address { get; set; }

        public virtual void FixNullObject()
        {
            CreateTime = CreateTime.Fix();
        }
    }
}