#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H207DeviceSimPhoneInfo .cs
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
    [DeviceOpCode(207)]
    public class H207DeviceSimPhoneInfo : IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P207DeviceSimPhoneInfo>(Handle);
        }

        private void Handle(IClient client, P207DeviceSimPhoneInfo p)
        {
            _log.Debug("PACKET", $"Serial  : {p.Serial}");
            _log.Debug("PACKET", $"TimeUpdate  : {p.TimeUpdate.ToString("G")}");
            _log.Debug("PACKET", $"Phone  : {p.Phone}");
        }
    }
}