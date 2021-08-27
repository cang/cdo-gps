using System;
using System.Collections.Generic;
using System.Linq;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    [Table]
    public class DeviceSetupInfo : IEntity
    {
        [HasOneColumn(Type = HasOneType.Child)]
        public virtual Device Device { get; set; }

        [PrimaryKey(KeyGenerateType = KeyGenerateType.Foreign, ForeignKey = "Device")]
        public virtual long Serial { get; set; }

        [BasicColumn]
        public virtual DateTime TimeUpdate { get; set; }

        [BasicColumn]
        public virtual int TimeSync { get; set; }

        [BasicColumn]
        public virtual short OverTimeInSession { get; set; }

        [BasicColumn]
        public virtual short OverTimeInDay { get; set; }

        //Cấu hình vận tốc tối đa hiện hành
        [BasicColumn]
        public virtual byte OverSpeed { get; set; }

        //Cang thêm cho cấu hình vận tốc tối đa cài đặt mặc định
        [BasicColumn]
        public virtual byte OverSpeedDefault { get; set; }

        public virtual IList<string> PhoneSystemControl
            => string.IsNullOrWhiteSpace(AllPhoneSystem) ? new List<string>() : AllPhoneSystem.Split('|').ToList();

        [BasicColumn]
        public virtual string FirmWareVersion { get; set; }

        [BasicColumn]
        public virtual string HardWareVersion { get; set; }

        [BasicColumn]
        public virtual string   AllPhoneSystem { get; set; }

        //Runtime value, only memory
        public virtual byte[]   RequestInforCommand { get; set; }

        public virtual void FixNullObject()
        {
            TimeUpdate = TimeUpdate.Fix();
            if (AllPhoneSystem == null)
                AllPhoneSystem = "";
        }
    }
}