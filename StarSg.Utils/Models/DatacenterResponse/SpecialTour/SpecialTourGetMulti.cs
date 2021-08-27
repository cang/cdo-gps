using StarSg.Core;
using StarSg.Utils.Models.Tranfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Models.DatacenterResponse.SpecialTour
{
    public class SpecialTourGetMulti : BaseResponse
    {
        public IList<SpecialTourTranfer> Datas { get; set; }
    }

    public class SpecialTourGet : BaseResponse
    {
        public SpecialTourTranfer Data { get; set; }
    }

}
