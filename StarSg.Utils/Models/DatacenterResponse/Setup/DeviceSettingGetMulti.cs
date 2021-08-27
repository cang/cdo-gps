using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Setup
{
    public class DeviceSettingGetMulti:BaseResponse
    {
         public IList<DeviceSettingInfo> Data { get; set; } 
    }
}