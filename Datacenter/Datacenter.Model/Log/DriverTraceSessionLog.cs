#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DriverTraceSessionLog.cs
// Time Create : 8:04 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;

#endregion

namespace Datacenter.Model.Log
{
    [Table]
    public class DriverTraceDaily10HLog : DriverTraceSessionLog
    {
        //chưa sử dụng Distance nên chưa gán
    }


    [Table]
    public class DriverTraceSessionLog : IDbLog
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long DriverId { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long Serial { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual Guid Indentity { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual long CompanyId { get; set; }

        [ComponentColumn]
        public virtual GpsLocation BeginLocation { get; set; }

        [ComponentColumn(Index = 1)]
        public virtual GpsLocation EndLocation { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime BeginTime { get; set; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime EndTime { get; set; }

        [BasicColumn]
        public virtual TimeSpan OverTime { get; set; }

        [BasicColumn]
        public virtual long Distance { get; set; }

        public virtual void FixNullObject()
        {
            BeginTime = BeginTime.Fix();
            EndTime = EndTime.Fix();
        }

        [BasicColumn]
        public virtual int DbId { get; set; }
    }
}