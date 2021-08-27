#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H209DeviceSimeMoneyInfo .cs
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
    [DeviceOpCode(209)]
    public class H209DeviceSimeMoneyInfo : IDeviceHandlePacket
    {
        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P209DeviceSimeMoneyInfo>(Handle);
        }

        private void Handle(IClient client, P209DeviceSimeMoneyInfo p)
        {
            _log.Debug("PACKET", $"Serial  : {p.Serial}");
            _log.Debug("PACKET", $"TimeUpdate  : {p.TimeUpdate.ToString("G")}");
            _log.Debug("PACKET", $"Money  : {p.Money}");
        }
    }
}