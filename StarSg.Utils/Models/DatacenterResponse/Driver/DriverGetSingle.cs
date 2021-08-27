#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : DriverGetSingle.cs
// Time Create : 4:12 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using Core.Models.Tranfer.Driver;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Driver
{
    public class DriverGetSingle:BaseResponse
    {
         public DriverTranfer Driver { get; set; }
    }
}