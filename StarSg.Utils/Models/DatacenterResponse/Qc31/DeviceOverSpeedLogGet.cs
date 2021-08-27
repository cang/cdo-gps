using System.Collections.Generic;
using Core.Models.Tranfer;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Qc31
{
    
    public class DeviceOverSpeedLogGet:BaseResponse
    {
         public IList<OverSpeedLogTranfer> Datas { get; set; } 
    }
}