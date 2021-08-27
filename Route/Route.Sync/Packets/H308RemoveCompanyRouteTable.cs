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
    [NodeShareOpCode(308)]
    public class H308RemoveCompanyRouteTable : INodeShareHandlePacket
    {
        [Import]
        private ILog _log;
        [Import]
        private ICompanyRouteTableUpdate _companyRouteTableUpdate;
        public Delegate GetHandle()
        {
            return new Action<INodeClient, P308RemoveCompanyRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P308RemoveCompanyRouteTable p)
        {
            _log.Debug("PACKET", $"Có gói tin xóa công ty");
            foreach (var companyId in p.CompanyIdList)
            {
                _log.Debug("PACKET",
                   _companyRouteTableUpdate.RemoveNoneBroadCast(companyId)
                       ? $"Đã xóa công ty : {companyId} thành công"
                       : $"Đã xóa công ty : {companyId} thất bại");
            }
        }
    }
}
