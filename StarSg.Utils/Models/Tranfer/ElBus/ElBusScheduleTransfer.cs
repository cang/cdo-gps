using StarSg.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Models.Tranfer.ElBus
{
    public class ElBusScheduleTransfer
    {
        public int id { get; set; }
        public long company_id { get; set; }
        public long group_id { get; set; }
        public int approved_by { get; set; }
        public int drived_by { get; set; }
        public int created_by { get; set; }
        public int route_id { get; set; }
        public String type { get; set; }
        public float km { get; set; }
        public int time { get; set; }
        public DateTime time_start { get; set; }
        public DateTime time_end { get; set; }
        public float price { get; set; }
        public bool active { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public String note { get; set; }
    }

    public class ElBusScheduleBaseResponse : BaseResponse
    {
        public ElBusScheduleTransfer Data;
    }

    public class ElBusScheduleBaseResponses : BaseResponse
    {
        public List<ElBusScheduleTransfer> Data = new List<ElBusScheduleTransfer>();
    }
}
