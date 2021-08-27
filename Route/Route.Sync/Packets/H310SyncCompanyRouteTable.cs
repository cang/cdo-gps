using System;
using System.ComponentModel.Composition;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;
using Route.Core;
using Route.Sync.Models;

namespace Route.Sync.Packets
{
    [Export(typeof (INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(310)]
    internal class H310SyncCompanyRouteTable : INodeShareHandlePacket
    {
        [Import] private ICompanyRouteTableUpdate _companyRouteTableUpdate;

        [Import] private IDataCenterStore _dataCenterStore;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P310SyncCompanyRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P310SyncCompanyRouteTable p)
        {
            _log.Debug("PACKET", $"Có gói tin đồng bộ company giữa các node");
            foreach (var it in p.CompanyDictionary)
            {
                var dataCenterInfo = _dataCenterStore.Get(it.Key);
                if (dataCenterInfo != null)
                {
                    _log.Debug("PACKET",
                        _companyRouteTableUpdate.PushNoneBroadCast(dataCenterInfo, it.Value)
                            ? $"Đã thêm list company vào datacenter: {dataCenterInfo.Ip} thành công"
                            : $"Đã thêm list company vào datacenter: {dataCenterInfo.Ip} thất bại");
                }
                else
                {
                    _log.Debug("PACKET", $"DataCenter {it.Key} không tồn tại");
                }
            }
        }
    }
}