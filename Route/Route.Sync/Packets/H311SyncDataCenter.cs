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
    [NodeShareOpCode(311)]
    public class H311SyncDataCenter : INodeShareHandlePacket
    {
        [Import]
        private IDataCenterStore _dataCenterStore;
        [Import]
        private ILog _log;
        public Delegate GetHandle()
        {
            return new Action<INodeClient, P311SyncDataCenter>(Handle);
        }

        private void Handle(INodeClient client, P311SyncDataCenter p)
        {
            _log.Debug("PACKET", $"Có gói tin đồng bộ DataCenter");
            foreach (var it in p.DataCenterList)
            {
                _log.Debug("PACKET",
                            _dataCenterStore.SaveOrUpdateNoneBroadCast(it)
                                ? $"Đã thêm DataCenter : {it.Ip}:{it.Port} thành công"
                                : $"Đã thêm DataCenter : {it.Ip}:{it.Port} thất bại");
            }
        }
    }
}
