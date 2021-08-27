#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H104OpenDoor.cs
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
    [DeviceOpCode(104)]
    public class H104OpenDoor : BaseHandle,IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P104OpenDoor>(Handle);
        }

        private void Handle(IClient client, P104OpenDoor p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"Time : {p.TimeUpdate.ToString("G")}");
            _log.Warning("PACKET", $"Lat : {p.GpsInfo.Lat}");
            _log.Warning("PACKET", $"Lng : {p.GpsInfo.Lng}");
            _log.Warning("PACKET", $"Speed : {p.GpsInfo.Speed}");

            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin mở cửa qua máy chủ xử lý : {(ForwardOpenDoor(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }
}