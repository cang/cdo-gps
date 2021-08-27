#region header
// /*********************************************************************************************/
// Project :Route.DatacenterStore
// FileName : H101AddDataCenter.cs
// Time Create : 9:36 AM 24/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.ComponentModel.Composition;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;
using Route.Core;
using Route.Sync.Models;

namespace Route.Sync.Packets
{
    [Export(typeof(INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(301)]
    public class H301AddDataCenter:INodeShareHandlePacket
    {
        [Import] private ILog _log;

        [Import] private IDataCenterStore _dataCenterStore;
        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P301AddDataCenter>(Handle);
        }

        private void Handle(INodeClient client, P301AddDataCenter p)
        {
            _log.Debug("PACKET", $"Có gói tin đăng ký datacenter");
            DataCenterInfo dataCenterInfo = new DataCenterInfo();
            dataCenterInfo.Id = Guid.Parse(p.Id);
            dataCenterInfo.Ip = p.Ip;
            dataCenterInfo.Port = p.Port;
            dataCenterInfo.NodeName = p.NodeName;
            _log.Debug("PACKET",
                _dataCenterStore.SaveOrUpdateNoneBroadCast(dataCenterInfo)
                    ? $"Đã thêm datacenter {p.Ip} thành công"
                    : $"Đã thêm datacenter {p.Ip} thất bại");
        }

        #endregion
    }
}