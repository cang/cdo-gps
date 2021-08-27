#region header

// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : DriverGetMulti.cs
// Time Create : 4:13 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using Core.Models.Tranfer.Driver;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Driver
{
    public class DriverGetMulti : BaseResponse
    {
        public IList<DriverTranfer> Drivers { get; set; }
    }
}