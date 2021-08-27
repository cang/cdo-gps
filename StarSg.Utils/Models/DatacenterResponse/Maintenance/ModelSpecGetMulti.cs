#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : ModelSpecGetMulti.cs
// Time Create : 8:34 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Maintenance
{
    public class ModelSpecGetMulti:BaseResponse
    {
         public IList<ModelSpecificationTranfer> Datas { get; set; } 
    }
}