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
    [NodeShareOpCode(305)]
    public class H305AddSerialRouteTable : INodeShareHandlePacket
    {
        [Import]
        private ILog _log;
        [Import]
        private IDataCenterStore _dataCenterStore;
        [Import]
        private IDeviceRouteTableUpdate _deviceRouteTableUpdate;
        public Delegate GetHandle()
        {
            return new Action<INodeClient, P305AddSerialRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P305AddSerialRouteTable p)
        {
            _log.Debug("PACKET", $"Có gói tin thêm serial mới");
            DataCenterInfo dataCenterInfo = _dataCenterStore.Get(Guid.Parse(p.DataCenterId));
            if (dataCenterInfo != null)
            {
                _log.Debug("PACKET",
                        _deviceRouteTableUpdate.PushNoneBroadCast(dataCenterInfo, p.SerialList)
                            ? $"Thêm danh sách thiết bị vào bảng định tuyến: {dataCenterInfo.Ip} thành công"
                            : $"Thêm danh sách thiết bị vào bảng định tuyến: {dataCenterInfo.Ip} thất bại");
            }
            else
            {
               _log.Debug("PACKET", $"DataCenter {p.DataCenterId} không tồn tại");
            }
        }
    }
}
