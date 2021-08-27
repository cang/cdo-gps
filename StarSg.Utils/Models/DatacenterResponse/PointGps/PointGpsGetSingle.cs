#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : PointGpsGetSingle.cs
// Time Create : 1:54 PM 21/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Security.Cryptography.X509Certificates;
using Core.Models.Tranfer.GpsCheckPoint;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.PointGps
{
    public class PointGpsGetSingle:BaseResponse
    {
         public GpsCheckPointTranfer Point { get; set; }
    }
}