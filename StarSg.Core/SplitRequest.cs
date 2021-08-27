#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Api
// TIME CREATE : 1:04 PM 18/12/2016
// FILENAME: SplitRequest.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Log;

#endregion

namespace Datacenter.Api.Core
{
    /// <summary>
    /// </summary>
    [Export(typeof(ISplitRequest))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SplitRequest : ISplitRequest
    {
        /// <summary>
        ///     nếu các diều kiện > số này thì bắt đầu chia ra.
        /// </summary>
        private const int LogicSplitLimit = 50;

        [Import] private ILog _log;

        #region Implementation of ISplitRequest

        /// <summary>
        ///     Chia ra các request nhỏ tới datacenter
        ///     Dữ liệu có thể bị xáo trộn do chay multithread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="logicWhere"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IList<T> Split<T, T1>(IList<T1> logicWhere, Func<IList<T1>, IList<T>> callback)
        {
            return Split(logicWhere, callback, true);
        }

        /// <summary>
        ///     Chia ra các request nhỏ tới datacenter
        ///     Dữ liệu có thể bị xáo trộn do chay multithread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="logicWhere"></param>
        /// <param name="callback"></param>
        /// <param name="multiTask"></param>
        /// <returns></returns>
        public IList<T> Split<T, T1>(IList<T1> logicWhere, Func<IList<T1>, IList<T>> callback, bool multiTask)
        {
            var result = new List<T>();
            if (callback == null)
            {
                _log.Warning("SplitRequest", "Callback is null");
                return result;
            }
            if ((logicWhere == null) || (logicWhere.Count <= LogicSplitLimit))
            {
                result.AddRange(callback(logicWhere));
                return result;
            }

            // cắt logic ra những đoạn = nhau
            var array = new List<IList<T1>>();
            var current = 0;

            var tmp = new List<T1>();
            while (current != logicWhere.Count)
            {
                if (tmp.Count == LogicSplitLimit)
                {
                    var tm = new List<T1>();
                    tmp.ForEach(m => tm.Add(m));
                    array.Add(tm);
                    tmp.Clear();
                }
                tmp.Add(logicWhere[current]);
                current++;
            }
            if (tmp.Count > 0)
                array.Add(tmp);

            //todo: nên để time out ở đây
            var tasks = new List<Task>();
            array.ForEach(m =>
            {
                if (!multiTask)
                    result.AddRange(callback(m));
                else
                {
                    var t = new Task(() => result.AddRange(callback(m)));
                    tasks.Add(t);
                    t.Start();
                }
            });
            if (multiTask)
                Task.WaitAll(tasks.ToArray());

            _log.Debug("SplitRequest", "GetDatacomplete");
            return result;
        }

        #endregion
    }
}