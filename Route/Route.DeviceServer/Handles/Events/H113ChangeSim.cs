#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H113ChangeSim.cs
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
using DevicePacketModels.Setups;
using Log;
using ServerCore;

#endregion

namespace Route.DeviceServer.Handles.Events
{
    [Export(typeof (IDeviceHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(113)]
    public class H113ChangeSim :BaseHandle, IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P113ChangeSim>(Handle);
        }

        private void Handle(IClient client, P113ChangeSim p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"Time : {p.TimeUpdate.ToString("G")}");
            client.Send(new P206ReadSimPhone());
            client.Send(new P208ReadSimMoney());

            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin đổi sim qua máy chủ xử lý : {(ForwardChangeSim(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }
}