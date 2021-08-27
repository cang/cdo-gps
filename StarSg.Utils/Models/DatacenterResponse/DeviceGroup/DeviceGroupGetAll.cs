using System.Collections;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.DeviceGroup
{
    public class DeviceGroupGetAll:BaseResponse
    {
        public IList<DeviceGroupGet> Groups { get; set; }=new List<DeviceGroupGet>();
    }
}