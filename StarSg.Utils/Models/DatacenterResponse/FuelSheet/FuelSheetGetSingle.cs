#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : DeviceModelSingle.cs
// Time Create : 3:08 PM 20/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using Core.Models.Tranfer;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.DeviceModel
{
    public class FuelSheetGetSingle : BaseResponse
    {
        public FuelSheetTranfer FuelSheet { set; get; }
    }
}