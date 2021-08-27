#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : AreaGetSingle.cs
// Time Create : 2:47 PM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using Core.Models.Tranfer.CheckZone;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.CheckZone;

namespace StarSg.Utils.Models.DatacenterResponse.Area
{
    public class AreaGetSingle:BaseResponse
    {
         public CheckZoneTranfer Area { set; get; }
    }
}