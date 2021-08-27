#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : DeviceGetMulti.cs
// Time Create : 10:45 AM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Device
{
    public class DeviceGetMultiEx:BaseResponse
    {
         public IList<DeviceTranferEx> Devices { get; set; } 
    }
}