#region header
// /*********************************************************************************************/
// Project :Route.Api
// FileName : IWClient.cs
// Time Create : 10:16 AM 30/09/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using StarSg.Utils.Models.DatacenterResponse.Status;

namespace Route.Api.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWClient
    {
        /// <summary>
        /// cập nhật thiết bị
        /// </summary>
        /// <param name="device"></param>
        void Update(StatusDeviceTranfer device);
    }
}