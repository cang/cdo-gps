using System.Collections;
using System.Collections.Generic;
using Core.Models.Tranfer;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Qc31
{
    public class DeviceSpeedTraceLogGet:BaseResponse
    {
         public IList<DeviceSpeedLogTranfer> Datas { get; set; }
    }
}