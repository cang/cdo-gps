#region header
// /*********************************************************************************************/
// Project :Route.Sync
// FileName : H312GetSerialRouteTable.cs
// Time Create : 3:32 PM 03/03/2016
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
    [Export(typeof(INodeShareHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [NodeShareOpCode(312)]
    public class H312GetDeviceRouteTable:INodeShareHandlePacket
    {
        [Import] private IDeviceRouteTable _deviceRoute;
        [Import] private ILog _log;
        #region Implementation of IHandlePacket

        public Delegate GetHandle()
        {
            return new Action<INodeClient, P312GetSerialRouteTable>(Handle);
        }

        private void Handle(INodeClient client, P312GetSerialRouteTable p)
        {
            var tmp = _deviceRoute.GetByDataId(p.IdDatacenter);
            if (tmp == null)
            {
                _log.Warning("PACKET", $"Không tồn tại bảng định tuyến ứng với datacenter id :{p.IdDatacenter}");
                return;
            }

            client.Send(new P309SyncSerialRouteTable()
            {
                SerialDictionary = new Dictionary<Guid, List<long>>() {{p.IdDatacenter, tmp.ToList() } }
            });
        }

        #endregion
    }
}