using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model.Log
{
    /// <summary>
    /// TraceType.Stop
    /// TraceType.Door
    /// TraceType.Machine
    /// TraceType.AirMachine
    /// TraceType.Stop15    (chưa có dữ liệu)
    /// TraceType.Run5  (chưa có dữ liệu)
    /// </summary>
    [Table]
    public class DeviceTraceLog : DeviceTraceLogBase
    {
    }

    /// <summary>
    ///     TraceType.HasGuest
    ///     TraceType.NoGuest
    /// </summary>
    [Table]
    public class DeviceGuestLog : DeviceTraceLogBase
    {
    }

    /// <summary>
    ///TraceType.ChangeDriver
    /// </summary>
    [Table]
    public class DeviceChangeDriverLog : DeviceTraceLogBase
    {
    }

    /// <summary>
    /// TraceType.ChangeSim (chưa có dữ liệu)
    /// </summary>
    [Table]
    public class DeviceChangeSimLog : DeviceTraceLogBase
    {
    }

    /// <summary>
    /// TraceType.LostGsm (chưa có dữ liệu)
    /// </summary>
    [Table]
    public class DeviceLostGsmLog : DeviceTraceLogBase
    {
    }

}

//using System;
//using DaoDatabase.AutoMapping.Enums;
//using DaoDatabase.AutoMapping.MapAtribute;
//using Datacenter.Model.Components;
//using Datacenter.Model.Utils;

//namespace Datacenter.Model.Log
//{
//    [Table]
//    public class DeviceTraceLog : IndexLogDevice, IDbLog
//    {
//        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
//        public virtual long Id { get; set; }
//        [ComponentColumn(Index = 0)]
//        public virtual GpsLocation BeginLocation { set; get; }
//        [ComponentColumn(Index = 1)]
//        public virtual GpsLocation EndLocation { set; get; }
//        [BasicColumn(IsIndex = true)]
//        public virtual DateTime BeginTime { set; get; }
//        [BasicColumn(IsIndex = true)]
//        public virtual DateTime EndTime { set; get; }
//        [BasicColumn(IsIndex = true)]
//        public virtual TraceType Type { set; get; }
//        [BasicColumn(IsIndex = true)]
//        public virtual long DriverId { set; get; }
//        [BasicColumn]
//        public virtual string Note { set; get; }

//        // các trường check ( ko lưu database)

//        public virtual DateTime DriverTime { get; set; }

//        public virtual void FixNullObject()
//        {
//            BeginTime = BeginTime.Fix();
//            EndTime = EndTime.Fix();

//        }
//        [BasicColumn]
//        public virtual int DbId { get; set; }

//        //todo: chưa cài đặt
//        [BasicColumn]
//        public virtual long Distance { get; set; }
//    }
//}