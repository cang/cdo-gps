#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : ModelSpecGetSingle.cs
// Time Create : 8:35 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Maintenance
{
    public class ModelSpecGetSingle:BaseResponse
    {
         public ModelSpecificationTranfer Data { get; set; }
    }
}