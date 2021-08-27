using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;
using Route.Core;
using Route.Sync.Models;

namespace Route.Sync.Packets
{
    [Export(typeof(INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(309)]
    public class H309SyncSerialRouteTable : INodeShareHandlePacket
    {
        [Import]
        private IDeviceRouteTableUpdate _deviceRouteTableUpdate;
        [Import]
        private IDataCenterStore _dataCenterStore;
        [Import]
        private ILog _log;
        public Delegate GetHandle()
        {
            return new Action<INodeClient, P309SyncSerialRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P309SyncSerialRouteTable p)
        {
            _log.Debug("PACKET", $"Có gói tin đồng bộ serial giữa các node");
            foreach (var it in p.SerialDictionary)
            {
                DataCenterInfo dataCenterInfo = _dataCenterStore.Get(it.Key);
                if (dataCenterInfo != null)
                {
                    _deviceRouteTableUpdate.CleanByDatacenterId(dataCenterInfo.Id);
                    _log.Debug("PACKET",
                            _deviceRouteTableUpdate.PushNoneBroadCast(dataCenterInfo, it.Value)
                                ? $"Đã thêm list serial vào datacenter: {dataCenterInfo.Ip} thành công"
                                : $"Đã thêm list serial vào datacenter: {dataCenterInfo.Ip} thất bại");
                }
                else
                {
                    _log.Debug("PACKET", $"DataCenter {it.Key} không tồn tại");
                }
            }
        }
    }
}
