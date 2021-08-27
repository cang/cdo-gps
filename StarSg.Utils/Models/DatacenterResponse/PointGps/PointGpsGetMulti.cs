#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : PointGpsGetMulti.cs
// Time Create : 1:54 PM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using Core.Models.Tranfer.GpsCheckPoint;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.PointGps
{
    public class PointGpsGetMulti:BaseResponse
    {
         public IList<GpsCheckPointTranfer> Points { set; get; }
    }
}