#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : MaintenanceReportGet.cs
// Time Create : 8:55 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.Maintenance;

namespace StarSg.Utils.Models.DatacenterResponse.Maintenance
{
    public class MaintenanceReportGet:BaseResponse
    {
         public IList<MaintenanceReportTranfer> Datas { get; set; } 
    }
}