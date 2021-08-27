﻿using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Log.ZipLog
{
    [Table]
    public class DriverTraceSessionLogZip : IZip
    {
        [BasicColumn(IsIndex = true)]
        public virtual long DriverId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(Length = int.MaxValue)]
        public virtual byte[] Data { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime TimeUpdate { get; set; }

        public virtual void FixNullObject()
        {
            TimeUpdate = TimeUpdate.Fix();
        }

        [BasicColumn]
        public virtual int DbId { get; set; }
    }
}