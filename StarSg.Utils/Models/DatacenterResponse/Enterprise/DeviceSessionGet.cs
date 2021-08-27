using System.Collections;
using System.Collections.Generic;
using Core.Models.Tranfer.DeviceManager;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.DeviceManager;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class DeviceSessionGet:BaseResponse
    {
         public IList<DeviceSessionLogTranfer> Datas { get; set; }
    }
}