using System.Collections;
using System.Collections.Generic;
using Core.Models.Tranfer.Qc31;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.Qc31;

namespace StarSg.Utils.Models.DatacenterResponse.Qc31
{
    public class AllDeviceReportGet:BaseResponse
    {
         public IList<GeneralReport> Datas { get; set; }
    }
}