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
    [NodeShareOpCode(306)]
    public class H306RemoveSerialRouteTable : INodeShareHandlePacket
    {
        [Import]
        private ILog _log;
        [Import]
        private IDeviceRouteTableUpdate _deviceRouteTableUpdate;
        public Delegate GetHandle()
        {
            return new Action<INodeClient, P306RemoveSerialRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P306RemoveSerialRouteTable p)
        {
            _log.Debug("PACKET", $"Có gói tin xóa serial");
            foreach (var serial in p.SerialList)
            {
                _log.Debug("PACKET",
                    _deviceRouteTableUpdate.RemoveNoneBroadCast(serial)
                        ? $"Đã xóa serial : {serial} thành công"
                        : $"Đã xóa serial : {serial} thất bại");
            }
        }
    }
}
