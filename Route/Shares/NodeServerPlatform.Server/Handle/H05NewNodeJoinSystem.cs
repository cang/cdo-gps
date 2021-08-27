#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : H05NewNodeJoinSystem.cs
// Time Create : 1:58 PM 29/01/2016
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
    [NodeShareOpCode(5)]
    public class H05NewNodeJoinSystem : INodeShareHandlePacket
    {
        [Import] private NodeServerFactory _global;
        [Import] private ILog _log;

        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P05NewNodeJoinSystem>(Handle);
        }

        private void Handle(INodeClient client, P05NewNodeJoinSystem p)
        {
            _log.Debug("PACKET", $"Có node mới kết nối vào hệ thống  :{p.Name}");
            _global.ConnectToNewNeighbor(p.Name, p.Ip, p.Port);
        }

        #endregion
    }
}