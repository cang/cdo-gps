#region header

// /*********************************************************************************************/
// Project :Route.Sync
// FileName : SyncFactory.cs
// Time Create : 11:28 AM 29/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Log;
using NodeServerPlatform.Core;
using Route.Core;
using Route.Sync.Models;
using StarSg.Core;

namespace Route.Sync
{
    [Export(typeof (IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SyncFactory : IModuleFactory, IPartImportsSatisfiedNotification
    {
        [Import] private ICompanyRouteTableEvent _companyRouteTableEvent;
        [Import] private IDataCenterStoreEvent _dataCenterStoreEvent;
        [Import] private IDeviceRouteTableEvent _deviceRouteTableEvent;
        [Import] private ILog _log;
        [Import] private INodeShareServer _nodeServer;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            _deviceRouteTableEvent.OnAdd += _deviceRouteTableEvent_OnAdd;
            _deviceRouteTableEvent.OnRemove += _deviceRouteTableEvent_OnRemove;
            _deviceRouteTableEvent.OnAdds += _deviceRouteTableEvent_OnAdds;
            _companyRouteTableEvent.OnAdd += _companyRouteTableEvent_OnAdd;
            _companyRouteTableEvent.OnRemove += _companyRouteTableEvent_OnRemove;
            _companyRouteTableEvent.OnAdds += _companyRouteTableEvent_OnAdds;
            _dataCenterStoreEvent.OnAdd += _dataCenterStoreEvent_OnAdd;
            _dataCenterStoreEvent.OnRemove += _dataCenterStoreEvent_OnRemove;
            _dataCenterStoreEvent.OnSync += _dataCenterStoreEvent_OnSync;
            _dataCenterStoreEvent.OnSyncSerial += _dataCenterStoreEvent_OnSyncSerial;
            _dataCenterStoreEvent.OnSyncCompanyId += _dataCenterStoreEvent_OnSyncCompanyId;
        }

        private void _dataCenterStoreEvent_OnSyncCompanyId(IList<DataCenterInfo> listDatacenter)
        {
            if (listDatacenter == null || listDatacenter.Count == 0) return;
            foreach (var info in listDatacenter)
            {
                _nodeServer.SendTo(info.Id.ToString(), new P312GetSerialRouteTable {IdDatacenter = info.Id});
            }
        }

        private void _dataCenterStoreEvent_OnSyncSerial(IList<DataCenterInfo> listDatacenter)
        {
            if (listDatacenter == null || listDatacenter.Count == 0) return;
            foreach (var info in listDatacenter)
            {
                _nodeServer.SendTo(info.Id.ToString(), new P313GetCompanyIdRouteTable {IdDatacenter = info.Id});
            }
        }

        private void _dataCenterStoreEvent_OnSync(IList<DataCenterInfo> listDatacenter)
        {
            // gửi gói tin đồng bộ datacenter tới tất cả các node.
            if (listDatacenter == null || listDatacenter.Count == 0) return;
            _nodeServer.SendAll(new P311SyncDataCenter {DataCenterList = listDatacenter});
        }

        /// <summary>
        ///     gửi danh sách serial cho node khác
        /// </summary>
        /// <param name="dataCenterInfo"></param>
        /// <param name="serialsList"></param>
        private void _deviceRouteTableEvent_OnAdds(DataCenterInfo dataCenterInfo, IList<long> serialsList)
        {
            if (dataCenterInfo == null || serialsList == null || serialsList.Count == 0) return;
            var addListDevice = new P305AddSerialRouteTable
            {
                DataCenterId = dataCenterInfo.Id.ToString(),
                SerialList = serialsList.ToList()
            };
            _nodeServer.SendAll(addListDevice);
        }

        /// <summary>
        ///     gửi danh sách company cho node khác
        /// </summary>
        /// <param name="dataCenter"></param>
        /// <param name="companyIds"></param>
        private void _companyRouteTableEvent_OnAdds(DataCenterInfo dataCenter, IList<long> companyIds)
        {
            if (dataCenter == null || companyIds == null || companyIds.Count == 0) return;
            var addCompanyList = new P307AddCompanyIdRouteTable
            {
                DataCenterId = dataCenter.Id.ToString(),
                CompanyIdList = companyIds.ToList()
            };
            _log.Debug("Sync-data",
                $"Đồng bộ danh sách công ty {dataCenter.NodeName}:{companyIds.Count} qua các node khác");
            _nodeServer.SendAll(addCompanyList);
        }

        /// <summary>
        ///     remove 1 datacenter ra khỏi node
        /// </summary>
        /// <param name="dataCenter"></param>
        private void _dataCenterStoreEvent_OnRemove(DataCenterInfo dataCenter)
        {
            if (dataCenter == null) return;
            var removeDataCenter = new P303RemoveDataCenter {Id = dataCenter.Id.ToString()};
            _nodeServer.SendAll(removeDataCenter);
        }

        /// <summary>
        ///     add 1 datacenter vào node
        /// </summary>
        /// <param name="dataCenter"></param>
        private void _dataCenterStoreEvent_OnAdd(DataCenterInfo dataCenter)
        {
            if (dataCenter == null) return;
            var addDataCenter = new P301AddDataCenter
            {
                Id = dataCenter.Id.ToString(),
                Ip = dataCenter.Ip,
                Port = dataCenter.Port
            };
            _nodeServer.SendAll(addDataCenter);
        }

        /// <summary>
        ///     remove 1 company ra khỏi node
        /// </summary>
        /// <param name="dataCenter"></param>
        /// <param name="compnayId"></param>
        private void _companyRouteTableEvent_OnRemove(DataCenterInfo dataCenter, long compnayId)
        {
            var listCompany = new List<long>();
            listCompany.Add(compnayId);
            var removeCompany = new P308RemoveCompanyRouteTable
            {
                CompanyIdList = listCompany
            };
            _nodeServer.SendAll(removeCompany);
        }

        /// <summary>
        ///     add 1 company mới vào node
        /// </summary>
        /// <param name="dataCenter"></param>
        /// <param name="compnayId"></param>
        private void _companyRouteTableEvent_OnAdd(DataCenterInfo dataCenter, long compnayId)
        {
            if (dataCenter == null) return;
            var listCompany = new List<long>();
            listCompany.Add(compnayId);
            var addCompany = new P307AddCompanyIdRouteTable
            {
                CompanyIdList = listCompany,
                DataCenterId = dataCenter.Id.ToString()
            };
            _nodeServer.SendAll(addCompany);
        }

        /// <summary>
        ///     remove 1 serial ra khỏi node
        /// </summary>
        /// <param name="datacenter"></param>
        /// <param name="serial"></param>
        private void _deviceRouteTableEvent_OnRemove(DataCenterInfo datacenter, long serial)
        {
            var listSerial = new List<long>();
            listSerial.Add(serial);
            var removeSerial = new P306RemoveSerialRouteTable
            {
                SerialList = listSerial
            };
            _nodeServer.SendAll(removeSerial);
        }

        /// <summary>
        ///     add thêm 1 serial vào node
        /// </summary>
        /// <param name="datacenter"></param>
        /// <param name="serial"></param>
        private void _deviceRouteTableEvent_OnAdd(DataCenterInfo datacenter, long serial)
        {
            if (datacenter == null) return;
            var listSerial = new List<long>();
            listSerial.Add(serial);
            var addSerial = new P305AddSerialRouteTable
            {
                SerialList = listSerial,
                DataCenterId = datacenter.Id.ToString()
            };
            _nodeServer.SendAll(addSerial);
        }

        #endregion
    }
}