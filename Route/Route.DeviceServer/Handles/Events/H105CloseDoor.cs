#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H105CloseDoor.cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Events;
using Log;
using ServerCore;

#endregion

namespace Route.DeviceServer.Handles.Events
{
    [Export(typeof (IDeviceHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(105)]
    public class H105CloseDoor : BaseHandle,IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P105CloseDoor>(Handle);
        }

        private void Handle(IClient client, P105CloseDoor p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"Time : {p.TimeUpdate.ToString("G")}");
            _log.Warning("PACKET", $"Lat : {p.GpsInfo.Lat}");
            _log.Warning("PACKET", $"Lng : {p.GpsInfo.Lng}");
            _log.Warning("PACKET", $"Speed : {p.GpsInfo.Speed}");

            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin đóng cửa qua máy chủ xử lý : {(ForwardCloseDoor(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }
}