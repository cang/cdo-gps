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
    [NodeShareOpCode(303)]
    public class H303RemoveDataCenter : INodeShareHandlePacket
    {
        [Import]
        private ILog _log;
        [Import]
        private IDataCenterStore _dataCenterStore;
        public Delegate GetHandle()
        {
            return new Action<INodeClient, P303RemoveDataCenter>(Handle);
        }

        private void Handle(INodeClient client, P303RemoveDataCenter p)
        {
            _log.Debug("PACKET", $"Có gói tin xóa datacenter");
            _log.Debug("PACKET",
                _dataCenterStore.DelNoneBroadCast(Guid.Parse(p.Id))
                    ? $"Xóa datacenter {p.Id} thành công"
                    : $"Xóa datacenter {p.Id} thất bại");
        }
    }
}
