namespace StarSg.Utils.Models.Tranfer.Maintenance
{
    public class MaintenanceReportTranfer
    {
        public long Serial { get; set; }
        public string Bs { get; set; }
        public string ModelName { get; set; }
        public long KmDaoLopCurrent { get; set; }
        public long KmDaoLopLimit { get; set; }
        public long KmThayVoCurrent { get; set; }
        public long KmThayVoLimit { get; set; }
        public long KmThayNhotCurrent { get; set; }
        public long KmThayNhotLimit { get; set; }
        public long KmThayLocDauCurrent { get; set; }
        public long KmThayLocDauLimit { get; set; }
        public long KmThayLocGioCurrent { get; set; }
        public long KmThayLocGioLimit { get; set; }
        public long KmThayLocNhotCurrent { get; set; }
        public long KmThayLocNhotLimit { get; set; }
    }
}