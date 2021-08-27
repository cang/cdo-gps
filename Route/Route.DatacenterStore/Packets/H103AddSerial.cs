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
    [NodeShareOpCode(103)]
    internal class H103AddSerial : INodeShareHandlePacket
    {
        [Import] private IDataCenterStore _dataCenterStore;

        [Import] private IDeviceRouteTableUpdate _deviceRouteTableUpdate;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P103AddSerial>(Handle);
        }

        private void Handle(INodeClient client, P103AddSerial p)
        {
            _log.Debug("PACKET", $"DataCenter gửi gói tin thêm serial mới");

            var dataCenterInfo = _dataCenterStore.Get(Guid.Parse(p.DataCenterId));
            if (dataCenterInfo != null)
            {
                _log.Debug("PACKET",
                    _deviceRouteTableUpdate.Push(dataCenterInfo, p.SerialList)
                        ? $"Thêm danh sách thiết bị vào bảng định tuyến: {dataCenterInfo.Ip} thành công"
                        : $"Thêm danh sách thiết bị vào bảng định tuyến: {dataCenterInfo.Ip} thất bại");

                //track lai trong truong hop khoi tao lai app tu dong
                if (p.SerialList != null && p.SerialList.Count > 1)
                {
                    _log.Error("PACKET", $"Total devices {p.SerialList.Count}");
                }
            }
            else
            {
                _log.Error("PACKET", $"DataCenter {p.DataCenterId} không tồn tại");
            }
        }

    }
}