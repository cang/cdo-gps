using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Route.Api.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentEnumerator<T> : IEnumerator<T>
    {
        #region Constructor

        public ConcurrentEnumerator(IEnumerable<T> inner, ReaderWriterLockSlim @lock)
        {
            _lock = @lock;
            _lock.EnterReadLock();
            _inner = inner.GetEnumerator();
        }

        #endregion

        #region Fields

        private readonly IEnumerator<T> _inner;
        private readonly ReaderWriterLockSlim _lock;

        #endregion

        #region Methods

        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

        public void Reset()
        {
            _inner.Reset();
        }

        public void Dispose()
        {
            _lock.ExitReadLock();
        }

        #endregion

        #region Properties

        public T Current => _inner.Current;

        object IEnumerator.Current => _inner.Current;

        #endregion
    }
}