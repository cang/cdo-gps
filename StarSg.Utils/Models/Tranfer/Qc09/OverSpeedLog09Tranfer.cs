using System;
using System.Runtime.Serialization;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer.Qc09
{
    [DataContract]
    public class OverSpeedLog09Tranfer
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long Serial { get; set; }
        [DataMember]
        public long GroupId { get; set; }
        [DataMember]
        public long DriverId { get; set; }
        [DataMember]
        public long CompanyId { get; set; }
        [DataMember]
        public int LimitSpeed { get; set; }
        [DataMember]
        public int MaxSpeed { get; set; }
        [DataMember]
        public int OverSpeed { get; set; }
        [DataMember]
        public DateTime BeginTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }
        [DataMember]
        public DateTime TimeUpdate { get; set; }
        [DataMember]
        public GpsPoint BeginPoint { get; set; }
        [DataMember]
        public GpsPoint EndPoint { get; set; }
        [DataMember]
        public TimeSpan TotalTime { get; set; }
        [DataMember]
        public double TotalDistance { get; set; }
        [DataMember]
        public string Bs { get; set; }
        [DataMember]
        public string DriverName { get; set; }
        [DataMember]
        public string Gplx { get; set; }
        [DataMember]
        public int ActivityType { get; set; }
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public int AverageSpeed { get; set; }
    }
}
