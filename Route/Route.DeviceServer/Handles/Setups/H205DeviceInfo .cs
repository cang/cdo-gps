#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H205DeviceInfo .cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Setups;
using Log;
using ServerCore;

#endregion

namespace Route.DeviceServer.Handles.Setups
{
    [Export(typeof (IDeviceHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(205)]
    public class H205DeviceInfo :BaseHandle, IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P205DeviceInfo>(Handle);
        }

        private void Handle(IClient client, P205DeviceInfo p)
        {
            _log.Debug("PACKET", $"Serial  : {p.Serial}");
            _log.Debug("PACKET", $"OverSpeed  : {p.OverSpeed}");
            _log.Debug("PACKET", $"FirmWareVersion  : {p.FirmWareVersion}");
            _log.Debug("PACKET", $"HardWareVersion  : {p.HardWareVersion}");
            _log.Debug("PACKET", $"OverTimeInDay  : {p.OverTimeInDay}");
            _log.Debug("PACKET", $"OverTimeInSession  : {p.OverTimeInSession}");
            _log.Debug("PACKET", $"TimeUpdate  : {p.TimeUpdate.ToString("G")}");
            foreach (var s in p.PhoneSystemControl)
            {
                _log.Debug("PACKET", $"  Phone  : {s}");
            }
            _log.Debug("PACKET",
               $"Chuyển tiếp thông tin thông tin thiêt bị qua máy chủ xử lý : {(ForwardDeviceInfo(p.Serial, p) ? "thành công" : "thất bại")}");
        }

    }
}