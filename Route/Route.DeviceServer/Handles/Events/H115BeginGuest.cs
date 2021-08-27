#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.DeviceServer
// TIME CREATE : 7:10 PM 01/11/2016
// FILENAME: H114EndOverTime.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

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
    [DeviceOpCode(115)]
    public class H115BeginGuest : BaseHandle, IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P115BeginGuest>(Handle);
        }

        private void Handle(IClient client, P115BeginGuest p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"BeginTime : {p.TimeUpdate.ToString("G")}");
            _log.Warning("PACKET", $"Lat : {p.GpsInfo.Lat}");
            _log.Warning("PACKET", $"Lng : {p.GpsInfo.Lng}");
            _log.Warning("PACKET", $"Speed : {p.GpsInfo.Speed}");

            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin mở máy qua máy chủ xử lý : {(ForwardBeginGuest(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }

}