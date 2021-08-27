using System;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Compnay
{
    public class CompanyGet : CompanyTranfer
    {
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
}