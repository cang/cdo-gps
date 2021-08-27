#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : P03YourInfo.cs
// Time Create : 4:15 PM 28/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.ComponentModel.Composition;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Models.Packets;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Server.Handle
{
    [Export(typeof (INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(3)]
    public class H03YourInfo : INodeShareHandlePacket
    {
        [Import] private NodeServerFactory _global;
        [Import] private ILog _log;

        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P03YourInfo>(Hanlde);
        }

        private void Hanlde(INodeClient client, P03YourInfo p)
        {
            _log.Debug("PACKET", $"{client.GetRemoteIp()}  Yêu cầu lấy thông tin kết nối");
            client.Send(new P04MyInfo
            {
                Ip = _global.Config.Ip,
                Port = _global.Config.Port,
                Name = _global.Config.Name
            });
        }

        #endregion
    }
}