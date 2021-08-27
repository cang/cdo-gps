using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.Qc31;

namespace StarSg.Utils.Models.DatacenterResponse.Qc31
{
    /// <summary>
    /// </summary>
    public class DeviceTripLogGet : BaseResponse
    {
        public IList<DeviceTripLogTranfer> Datas { get; set; }
    }
}