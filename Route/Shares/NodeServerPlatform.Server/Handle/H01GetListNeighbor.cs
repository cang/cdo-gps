#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : P01GetListNeighbor.cs
// Time Create : 11:22 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.ComponentModel.Composition;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Models;
using NodeServerPlatform.Core.Models.Packets;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Server.Handle
{
    [Export(typeof (INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(1)]
    public class H01GetListNeighbor : INodeShareHandlePacket, IPartImportsSatisfiedNotification
    {
        [Import] private NodeServerFactory _global;
        [Import] private ILog _log;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P01GetListNeighbor>(Handle);
        }

        private void Handle(INodeClient client, P01GetListNeighbor p)
        {
            //lấy danh sách các node mà mình biết gưởi cho client
            _log.Debug("PACKET", $"Có yêu cầu lấy thông tin các node lân cận từ {client.GetRemoteIp()}");
            var packet = new P02ListNeighbor();
            foreach (var info in _global.Config.Neighbor.Values)
            {
                packet.Neighbors.Add(info.Name,
                    new NeighborInfo {Ip = info.Ip, Name = info.Name, Port = info.Port});
            }
            //packet.Neighbors.Add("Test", new NeighborInfo {Ip = "127.0.0.1", Name = "Test", Port = 1300});
            //packet.Neighbors.Add("Test1", new NeighborInfo {Ip = "127.0.0.1", Name = "Test1", Port = 1301});
            //packet.Neighbors.Add("Test2", new NeighborInfo {Ip = "127.0.0.1", Name = "Test2", Port = 1302});
            client.Send(packet);
        }

        #endregion
    }
}