#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H108BeginStop.cs
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
    [DeviceOpCode(108)]
    public class H108BeginStop :BaseHandle, IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P108BeginStop>(Handle);
        }

        private void Handle(IClient client, P108BeginStop p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"Time : {p.TimeUpdate.ToString("G")}");
            _log.Warning("PACKET", $"Lat : {p.GpsInfo.Lat}");
            _log.Warning("PACKET", $"Lng : {p.GpsInfo.Lng}");
            _log.Warning("PACKET", $"Speed : {p.GpsInfo.Speed}");


            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin bắt đầu dừng đỗ qua máy chủ xử lý : {(ForwardBeginStop(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }
}