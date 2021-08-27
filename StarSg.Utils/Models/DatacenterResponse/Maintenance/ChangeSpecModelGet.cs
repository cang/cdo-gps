#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : ChangeSpecModelGet.cs
// Time Create : 1:40 PM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.Maintenance;

namespace StarSg.Utils.Models.DatacenterResponse.Maintenance
{
    public class ChangeSpecModelGet:BaseResponse
    {
         public IList<MaintenanceHistoryTranfer> Datas { get; set; }
    }
}