using StarSg.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Models.Tranfer.ElBus
{
    public class ElBusPriceTransfer
    {
        public int id { get; set; }
        public long company_id { get; set; }
        public long group_id { get; set; }
        public float price_by_km { get; set; }
        public float price_by_time { get; set; }
    }

    public class ElBusPriceBaseResponse : BaseResponse
    {
        public ElBusPriceTransfer Data;
    }

    public class ElBusPriceBaseResponses : BaseResponse
    {
        public List<ElBusPriceTransfer> Data = new List<ElBusPriceTransfer>();
    }
}
