using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Log;

namespace Route.Api.Auth.Core
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DelayActionManager : IDisposable, IPartImportsSatisfiedNotification
    {
        private readonly IList<Tuple<long, TimeSpan, DateTime>> _allEvents = new List<Tuple<long, TimeSpan, DateTime>>();
        private readonly CancellationTokenSource _cancelTaskHandle = new CancellationTokenSource();
        private readonly object _lockEvent = new object();
        [Import] private ILog _log;
        [Import] private IRequestManager _requestManager;

        public void Dispose()
        {
            _cancelTaskHandle.Cancel();
        }

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            //Task.Factory.StartNew(Handle);
        }

        #endregion

        public long AddAction(Action action, TimeSpan time)
        {
            lock (_lockEvent)
            {
                var id = _requestManager.CreateRequest(action, time);
                _allEvents.Add(new Tuple<long, TimeSpan, DateTime>(id, time, DateTime.Now));
                return id;
            }
        }

        public int Count() => _allEvents.Count;

        private async void Handle()
        {
            while (true)
            {
                try
                {
                    var tmp = _allEvents.ToList(); // capture
                    var removes = new List<Tuple<long, TimeSpan, DateTime>>();

                    foreach (var t in tmp.Where(t => DateTime.Now - t.Item3 >= t.Item2))
                    {
                        _requestManager.ExcuteReponse(t.Item1, m => m.DynamicInvoke());
                        removes.Add(t);
                    }

                    lock (_lockEvent)
                    {
                        foreach (var r in removes)
                        {
                            _allEvents.Remove(r);
                        }
                    }

                    removes.Clear();
                }
                catch (Exception ex)
                {
                    _log.Exception("SYSTEM", ex, "Delay action error");
                }

                if (_cancelTaskHandle.IsCancellationRequested)
                    return;
                try
                {
                    // khi cancel task nếu đang nằm trong hàm delay sẽ phát sinh exception
                    // nên ta try catch bỏ qua
                    await Task.Delay(10, _cancelTaskHandle.Token);
                }
                catch
                {
                }
            }
        }

        public void ResetTimeOut(long id)
        {
            _requestManager.ResetTimeOut(id);
        }

        public void RemoveAction(long id)
        {
            _requestManager.RemoveRequest(id);
        }
    }
}