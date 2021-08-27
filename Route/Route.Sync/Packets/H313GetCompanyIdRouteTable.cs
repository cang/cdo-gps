#region header

// /*********************************************************************************************/
// Project :Route.Sync
// FileName : H313GetCompanyIdRouteTable.cs
// Time Create : 4:14 PM 03/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;
using Route.Core;
using Route.Sync.Models;

namespace Route.Sync.Packets
{
    [Export(typeof (INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(313)]
    public class H313GetCompanyIdRouteTable: INodeShareHandlePacket
    {
        [Import] private ICompanyRouteTable _companyRouteTable;
        [Import] private ILog _log;

        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P313GetCompanyIdRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P313GetCompanyIdRouteTable p)
        {
            var tmp = _companyRouteTable.GetByDataId(p.IdDatacenter);
            if (tmp == null)
            {
                _log.Warning("PACKET", $"Không tồn tại bảng định tuyến ứng với datacenter id :{p.IdDatacenter}");
                return;
            }

            client.Send(new P310SyncCompanyRouteTable
            {
                CompanyDictionary = new Dictionary<Guid, List<long>>() { { p.IdDatacenter, tmp.ToList() } }
            });
        }

        #endregion
    }
}