#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : DeviceModelGetMulti.cs
// Time Create : 3:09 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using System.Net.NetworkInformation;
using Core.Models.Tranfer;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.DeviceModel
{
    public class DeviceModelGetMulti:BaseResponse
    {
         public IList<DeviceModelTranfer> Models { get; set; } 
    }
}