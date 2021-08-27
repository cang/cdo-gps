using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Compnay
{
    public class CompanyGetAll:BaseResponse
    {
        public IList<CompanyGet> Companies { get; set; }
    }
}