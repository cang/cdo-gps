using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Log;

namespace Route.Api.Auth.Core
{
    /// <summary>
    /// quản lý các sự kiện có timeout
    /// </summary>
    [Export(typeof(IRequestManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class RequestManager : IRequestManager, IPartImportsSatisfiedNotification
    {
        private long _requestIndex = 0;

        private readonly ConcurrentDictionary<long, RequestInfo> _allRequest =
            new ConcurrentDictionary<long, RequestInfo>();


        [Import] private ILog _log;
        #region Implementation of IRequestManager

        public long CreateRequest(Delegate callback, TimeSpan timeout)
        {
            Interlocked.Increment(ref _requestIndex);
            try
            {
                _allRequest.TryAdd(_requestIndex,
                    new RequestInfo { Delegate = callback, TimeOut = timeout, TimeRegister = DateTime.Now });
               // _log.Debug("RequestManager", $"Cài đặt thành công request :{_requestIndex}  Timeout : {timeout}");
            }
            catch (Exception ex)
            {
                _log.Exception("RequestManager", ex, "Create Request Error !");
                return -1;
            }

            return _requestIndex;
        }

        public bool ResetTimeOut(long id)
        {
            RequestInfo info;
            if (!_allRequest.TryGetValue(id, out info))
                return false;
            info.TimeRegister = DateTime.Now;
            return true;
        }

        public bool ExcuteReponse(long id, Action<Delegate> action)
        {
            RequestInfo info;
            if (!_allRequest.TryGetValue(id, out info))
                return false;
            info.Lock = true;
            try
            {
                action.Invoke(info.Delegate);
            }
            catch (Exception ex)
            {
                _log.Exception("RequestManager", ex, $"Xử lý callback request {id} lỗi !");
            }
            info.Lock = false;
            RemoveRequest(id);

            return true;
        }

        public bool RemoveRequest(long id)
        {
            RequestInfo info;

            if (_allRequest.TryRemove(id, out info))
            {
                _log.Debug("RequestManager", $"Gỡ bỏ thành công request : {id}");
                
                return true;
            }
            else
            {
                if (_allRequest.ContainsKey(id))
                {
                    _log.Warning("RequestManager", $"Gỡ bỏ không thành công requets : {id}");
                    return RemoveRequest(id);
                }
            }
            return false;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> Wait(long id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            while (_allRequest.ContainsKey(id))
            {
                Thread.Sleep(10);
            }
            return true;
        }

        public bool Contain(long id)
        {
            return _allRequest.ContainsKey(id);
        }

        #endregion

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            Task.Factory.StartNew(ManagerTimeOutRequest);
        }

        private async void ManagerTimeOutRequest()
        {
            while (true)
            {
                try
                {
                    var tmp = _allRequest.GetEnumerator();
                    while (tmp.MoveNext())
                    {
                        if (tmp.Current.Value.IsTimeOut && !tmp.Current.Value.Lock)
                        {
                            ExcuteReponse(tmp.Current.Key, m => m.DynamicInvoke());
                            //RemoveRequest(tmp.Current.Key);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Exception("RequestManager", ex, "Manager Timeout Error");
                }
                await Task.Delay(500);
            }
        }

        #endregion
    }
}