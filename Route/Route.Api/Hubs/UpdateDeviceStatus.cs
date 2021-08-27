#region header
// /*********************************************************************************************/
// Project :Route.Api
// FileName : UpdateDeviceStatus.cs
// Time Create : 11:04 AM 30/09/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using StarSg.Utils.Models.DatacenterResponse.Status;

namespace Route.Api.Hubs
{
    /// <summary>
    /// hàm cập nhật cho trạng thái thiết bị từ bên datacenter
    /// </summary>
    public class UpdateDeviceStatus:Hub
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        public void Update(IList<StatusDeviceTranfer> status)
        {
            // cập nhật cho các websocket đang kết nối
            foreach (var st in status)
            {
                var tmp = DeviceStatusHub.ClientManager.GetAllSocketBySerial(st.Serial);
                foreach (var wClient in tmp)
                {
                   // Task.Factory.StartNew(() =>
                    {
                        wClient.Update(st);
                    }//);
                }
            }
        }
    }
}