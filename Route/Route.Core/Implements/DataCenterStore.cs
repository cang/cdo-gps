#region header

// /*********************************************************************************************/
// Project :Route.Core
// FileName : DataCenterStore.cs
// Time Create : 8:31 AM 29/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Log;

namespace Route.Core.Implements
{
    [Export(typeof (IDataCenterStoreEvent))]
    [Export(typeof (IDataCenterStore))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DataCenterStore : IDataCenterStore, IDataCenterStoreEvent, IPartImportsSatisfiedNotification,IDisposable
    {
        private readonly ConcurrentDictionary<Guid, DataCenterInfo> _allDataCenter =
            new ConcurrentDictionary<Guid, DataCenterInfo>();

        private bool _dispose = false;
        [Import] private ILog _log;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            //throw new NotImplementedException();
            //cài đặt sự kiện đồng bộ datacenter.
            Task.Factory.StartNew(SyncDataCenter);
            //cài đặt sự kiện đồng bộ bảng định tuyến serial
            Task.Factory.StartNew(SyncSerialRouteTable);
            //cài đặt sự kiện đồng bộ bảng định tuyến companyId
            Task.Factory.StartNew(SyncCompanyIdRouteTable);
        }

        private async void SyncCompanyIdRouteTable()
        {
            while (!_dispose)
            {
                try
                {
                    _log.Debug("", "Đồng bộ bảng định tuyến serial ....");
                    OnSyncSerial?.Invoke(_allDataCenter.Values.ToList());
                }
                catch (Exception ex)
                {
                    _log.Exception("", ex, "Task đồng bộ thông tin datacenter lỗi");
                }
                await Task.Delay(10000);
            }
        }

        private async void SyncSerialRouteTable()
        {
            while (!_dispose)
            {
                try
                {
                    _log.Debug("", "Đồng bộ bảng định tuyến công ty ....");
                    OnSyncCompanyId?.Invoke(_allDataCenter.Values.ToList());
                }
                catch (Exception ex)
                {
                    _log.Exception("", ex, "Task đồng bộ thông tin datacenter lỗi");
                }
                await Task.Delay(10000);
            }
        }

        private async void SyncDataCenter()
        {
            while (!_dispose)
            {
                try
                {
                    _log.Debug("", "Đồng bộ datacenter....");
                    OnSync?.Invoke(_allDataCenter.Values.ToList());
                }
                catch (Exception ex)
                {
                    _log.Exception("", ex, "Task đồng bộ thông tin datacenter lỗi");
                }
                await Task.Delay(10000);
            }
        }

        #endregion

        #region Implementation of IDataCenterStore

        public bool SaveOrUpdate(DataCenterInfo center)
        {

            return SaveOrUpdate(center, true);
            //throw new NotImplementedException();
        }

        private bool SaveOrUpdate(DataCenterInfo center,bool broadcast)
        {
            DataCenterInfo val;
            if (_allDataCenter.TryGetValue(center.Id, out val))
            {
                _log.Debug("", $"Có cập nhật thông tin datacenter ip:{center.Ip}:{center.Port} ,{center.Id} ");
                return _allDataCenter.TryUpdate(center.Id, center, val);
            }
            if (_allDataCenter.TryAdd(center.Id, center))
            {
                _log.Debug("", $"Thêm mới datacenter ip :{center.Ip}:{center.Port} , {center.Id}");
                if (broadcast)
                    OnAdd?.Invoke(center);
                return true;
            }
            return false;
            //throw new NotImplementedException();
        }
        public bool SaveOrUpdateNoneBroadCast(DataCenterInfo center)
        {
            return SaveOrUpdate(center, false);
        }

        public bool Del(Guid id)
        {
            return Del(id, true);
            // throw new NotImplementedException();
        }
        private bool Del(Guid id, bool broadcast)
        {
            DataCenterInfo val;
            if (_allDataCenter.TryRemove(id, out val))
            {
                if (broadcast)
                    OnRemove?.Invoke(val);
                _log.Debug("", $"Xóa bỏ thông tin datacenter {val.Ip}:{val.Port}  {val.Id}");
                return true;
            }
            return false;
            // throw new NotImplementedException();
        }

        public bool DelNoneBroadCast(Guid id)
        {
            return Del(id, false);
        }
        
        public DataCenterInfo Get(Guid id)
        {
            DataCenterInfo val;
            _allDataCenter.TryGetValue(id, out val);
            return val;
        }

        public IList<DataCenterInfo> GetAll()
        {
            return _allDataCenter.Values.ToList();
        }

        #endregion

        #region Implementation of IDataCenterStoreEvent

        public event Action<DataCenterInfo> OnAdd;
        public event Action<DataCenterInfo> OnRemove;
        public event Action<IList<DataCenterInfo>> OnSync;
        public event Action<IList<DataCenterInfo>> OnSyncSerial;
        public event Action<IList<DataCenterInfo>> OnSyncCompanyId;

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _dispose = true;
        }

        #endregion
    }
}