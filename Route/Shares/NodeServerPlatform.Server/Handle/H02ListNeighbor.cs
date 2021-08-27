#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : H02ListNeighbor.cs
// Time Create : 1:49 PM 29/01/2016
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
    [NodeShareOpCode(2)]
    public class H02ListNeighbor : INodeShareHandlePacket
    {
        [Import] private NodeServerFactory _global;
        [Import] private ILog _log;

        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P02ListNeighbor>(Handle);
        }

        private void Handle(INodeClient client, P02ListNeighbor p)
        {
            _log.Debug("PACKET",
                $"Có gói tin báo danh sách node hàng xóm từ {client.GetRemoteIp()} số lượng node {p.Neighbors.Count}");
            try
            {
                foreach (var info in p.Neighbors.Values)
                {
                    _global.ConnectToNewNeighbor(info.Name, info.Ip, info.Port);
                }
            }
            catch (Exception ex)
            {
                _log.Exception("PACKET", ex, "xử lý gói tin cập nhật danh sách node liền kề lỗi");
            }
        }

        #endregion
    }
}