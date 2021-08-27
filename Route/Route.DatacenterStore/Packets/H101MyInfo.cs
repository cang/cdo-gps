using System;
using System.ComponentModel.Composition;
using Core.Models;
using Log;
using NodeServerPlatform.Core;
using Route.Core;
using NodeServerPlatform.Core.Utils;
using NodeServerPlatform.Packet;

namespace Route.DatacenterStore.Packets
{
    [Export(typeof (INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(101)]
    internal class H101MyInfo : INodeShareHandlePacket
    {
        [Import] private IDataCenterStore _dataCenterStore;

        [Import] private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P101MyInfo>(Handle);
        }

        private void Handle(INodeClient client, P101MyInfo p)
        {
            _log.Debug("PACKET", $"DataCenter gửi gói tin đăng ký mới");
            var dataCenterInfo = new DataCenterInfo
            {
                Id = Guid.Parse(p.Id),
                Ip = p.Ip,
                Port = p.Port,
                NodeName = p.NodeName,
                ReportCount = p.ReportCount
            };
            client.Name = p.Id;
            if (_dataCenterStore.SaveOrUpdate(dataCenterInfo))
            {
                //send gói 102 
                client.Send(new P102AcceptYourInfo {IsAccept = true});
                _log.Debug("PACKET", $"Chấp nhận kết nối DataCenter {p.Ip}");
            }
            else
            {
                //send gói 102
                client.Send(new P102AcceptYourInfo {IsAccept = false});
                _log.Debug("PACKET", $"Không chấp nhận kết nối DataCenter {p.Ip} ");
            }
        }
    }
}