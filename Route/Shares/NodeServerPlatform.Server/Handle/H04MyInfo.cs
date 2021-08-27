#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : P04MyInfo.cs
// Time Create : 4:22 PM 28/01/2016
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
    [NodeShareOpCode(4)]
    public class H04MyInfo : INodeShareHandlePacket
    {
        [Import] private NodeServerFactory _global;
        [Import] private ILog _log;

        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P04MyInfo>(Handle);
        }

        private void Handle(INodeClient client, P04MyInfo p)
        {
            // mở kêt nối qua node mới kết nối.
            _log.Debug("PACKET", $"Nhận được thông tin kết nối mới từ node {p.Name} {client.GetRemoteIp()}");
            client.Name = p.Name;
            _global.ConnectToNewNeighbor(p.Name, p.Ip, p.Port);
        }

        #endregion
    }
}