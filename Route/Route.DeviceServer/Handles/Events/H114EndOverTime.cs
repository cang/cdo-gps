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
    [DeviceOpCode(114)]
    public class H114EndOverTime : BaseHandle, IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P114EndOvertime>(Handle);
        }

        private void Handle(IClient client, P114EndOvertime p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"BeginTime : {p.BeginTime.ToString("G")}");
            _log.Warning("PACKET", $"Lat : {p.GpsEnd}");
            _log.Warning("PACKET", $"Lng : {p.GpsBegin}");
            _log.Warning("PACKET", $"EndTime : {p.EndTime.ToString("G")}");
            _log.Warning("PACKET", $"Distnace : {p.Distance}");

            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin mở máy qua máy chủ xử lý : {(ForwardEndOverTime(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }
}