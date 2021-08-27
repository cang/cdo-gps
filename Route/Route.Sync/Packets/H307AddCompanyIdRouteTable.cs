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
    [NodeShareOpCode(307)]
    internal class H307AddCompanyIdRouteTable : INodeShareHandlePacket
    {
        [Import] private ICompanyRouteTableUpdate _companyRouteTableUpdate;

        [Import] private IDataCenterStore _dataCenterStore;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P307AddCompanyIdRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P307AddCompanyIdRouteTable p)
        {
            _log.Debug("PACKET", $"Có gói tin thêm công ty");
            var dataCenterInfo = _dataCenterStore.Get(Guid.Parse(p.DataCenterId));
            _log.Debug("PACKET",
                _companyRouteTableUpdate.PushNoneBroadCast(dataCenterInfo, p.CompanyIdList)
                    ? $"Thêm danh sách công ty : thành công"
                    : $"Thêm công ty :  thất bại");
        }
    }
}