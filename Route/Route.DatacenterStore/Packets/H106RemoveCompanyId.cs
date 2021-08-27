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
    [NodeShareOpCode(106)]
    internal class H106RemoveCompanyId : INodeShareHandlePacket
    {
        [Import] private ICompanyRouteTableUpdate _companyRouteTableUpdate;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P106RemoveCompanyId>(Handle);
        }

        private void Handle(INodeClient client, P106RemoveCompanyId p)
        {
            _log.Debug("PACKET", $"DataCenter gửi gói tin xóa công ty");
            foreach (var companyId in p.CompanyIdList)
            {
                _log.Debug("PACKET",
                    _companyRouteTableUpdate.Remove(companyId)
                        ? $"Đã xóa công ty : {companyId} thành công"
                        : $"Đã xóa công ty : {companyId} thất bại");
            }
        }
    }
}