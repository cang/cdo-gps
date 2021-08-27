using System;

namespace Route.Core
{
    public class DataCenterInfo
    {
        public Guid Id { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string NodeName { get; set; }
        public int ReportCount { get; set; }
    }
    
}