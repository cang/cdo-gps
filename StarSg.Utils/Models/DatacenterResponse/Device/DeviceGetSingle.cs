#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : DeviceGetSingle.cs
// Time Create : 10:44 AM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Device
{
    public class DeviceGetSingle:BaseResponse
    {
         public DeviceTranfer Device { get; set; }
    }
}