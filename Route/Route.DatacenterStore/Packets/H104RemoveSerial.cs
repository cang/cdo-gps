using System;
using System.ComponentModel.Composition;
using Core.Models;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;
using Route.Core;

namespace Route.DatacenterStore.Packets
{
    [Export(typeof (INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(104)]
    internal class H104RemoveSerial : INodeShareHandlePacket
    {
        [Import] private IDeviceRouteTableUpdate _deviceRouteTableUpdate;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P104RemoveSerial>(Handle);
        }

        private void Handle(INodeClient client, P104RemoveSerial p)
        {
            _log.Debug("PACKET", $"DataCenter gửi gói tin xóa serial");
            foreach (var serial in p.SerialList)
            {
                _log.Debug("PACKET",
                    _deviceRouteTableUpdate.Remove(serial)
                        ? $"Đã xóa serial : {serial} thành công"
                        : $"Đã xóa serial : {serial} thất bại");
            }
        }
    }
}