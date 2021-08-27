using StarSg.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Models.Tranfer.ElBus
{
    public class ElBusRouteTransfer
    {
        public int id { get; set; }
        public long company_id { get; set; }
        public long group_id { get; set; }
        public string name { get; set; }
        public byte[] data { set; get; }
        public float km { get; set; }
        public float price { get; set; }
        public DateTime created_at { get; set; }
        public string note { get; set; }
    }

    public class ElBusRouteBaseResponse : BaseResponse
    {
        public ElBusRouteTransfer Data;
    }

    public class ElBusRouteBaseResponses : BaseResponse
    {
        public List<ElBusRouteTransfer> Data = new List<ElBusRouteTransfer>();
    }

}
