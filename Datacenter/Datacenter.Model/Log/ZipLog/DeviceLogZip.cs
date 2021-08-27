using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Log.ZipLog
{
    [Table]
    public class DeviceLogZip: IDeviceZip
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual Guid Indentity { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }
        [BasicColumn(IsIndex = true)]
        public virtual long GroupId { get; set; }
        public virtual void FixNullObject()
        {
        }
        [BasicColumn]
        public virtual int DbId { get; set; }

        #region Implementation of IZip

        [BasicColumn(Length = int.MaxValue)]
        public virtual byte[] Data { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime TimeUpdate { get; set; }

        #endregion
    }
}