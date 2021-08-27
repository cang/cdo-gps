#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : AreaGetMulti.cs
// Time Create : 2:48 PM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using Core.Models.Tranfer.CheckZone;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.CheckZone;

namespace StarSg.Utils.Models.DatacenterResponse.Area
{
    public class AreaGetMulti:BaseResponse
    {
         public IList<CheckZoneTranfer> Areas { get; set; } 
    }
}