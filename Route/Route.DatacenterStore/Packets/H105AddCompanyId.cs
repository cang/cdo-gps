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
    [NodeShareOpCode(105)]
    internal class H105AddCompanyId : INodeShareHandlePacket
    {
        [Import] private ICompanyRouteTableUpdate _companyRouteTableUpdate;

        [Import] private IDataCenterStore _dataCenterStore;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P105AddCompanyId>(Handle);
        }

        private void Handle(INodeClient client, P105AddCompanyId p)
        {
            _log.Debug("PACKET", $"DataCenter gửi gói tin thêm công ty");
            var dataCenterInfo = _dataCenterStore.Get(Guid.Parse(p.DataCenterId));
            _log.Debug("PACKET",
                _companyRouteTableUpdate.Push(dataCenterInfo, p.CompanyIdList)
                    ? $"Thêm danh sách công ty : thành công"
                    : $"Thêm công ty :  thất bại");
            
        }
    }
}