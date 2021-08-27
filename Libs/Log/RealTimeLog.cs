#region header
// /*********************************************************************************************/
// Project :APICoreImplement
// FileName : RealTimeLog.cs
// Time Create : 9:46 AM 20/08/2015
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Log
{
    public class TagLog
    {
        private long _index;
        private const long LimitLogCount = 5000;
        public long Index => _index;
        public ConcurrentDictionary<long, string> Data { get; } =
            new ConcurrentDictionary<long, string>();

        public void AddLog(string log)
        {
            Interlocked.Increment(ref _index);
            if (Data.Count > LimitLogCount)
            {
                lock (Data)
                {
                    if (Data.Count > LimitLogCount)
                    {
                        Data.Clear();
                        while (!Data.IsEmpty)
                        {
                            Data.Clear();
                        }
                    }
                }
            }
            Data.TryAdd(_index, log);
        }

        public Tuple<long, IList<string>> GetLog(long index)
        {
            if (index == 0)
                return new Tuple<long, IList<string>>(Index, Data.Values.ToList());
            if (index > Index)
                return new Tuple<long, IList<string>>(index, Data.Values.ToList());
            if (index == Index)
                return new Tuple<long, IList<string>>(index, new List<string>());
            return new Tuple<long, IList<string>>(Index,
                Data.ToList().Where(m => m.Key > index).Select(m => m.Value).ToList());
        }
    }
    //[Export(typeof(IAttackLog))]
    //[Export(typeof(RealTimeLog))]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class RealTimeLog:IAttackLog
    {
        private readonly TagLog _alllog = new TagLog();

        private readonly ConcurrentDictionary<string, TagLog>  _allTaglog =
            new ConcurrentDictionary<string, TagLog>();
#pragma warning disable 1998
        public async Task<bool> Writer(string tag,LogType type, string log)
#pragma warning restore 1998
        {
            _alllog.AddLog(log);

            TagLog data;
            if (!_allTaglog.TryGetValue(tag, out data))
            {
                data = new TagLog();
                    _allTaglog.TryAdd(tag, data);
            }
            data.AddLog(log);
            return true;
        }
        

        public Tuple<long, IList<string>> GetLog(long index)
        {
            return _alllog.GetLog(index);
        }

        public Tuple<long, IList<string>> GetTagLog(string tag, long index)
        {
            TagLog data;
            if (!_allTaglog.TryGetValue(tag, out data))
            {
                return new Tuple<long, IList<string>>(0, new List<string>());
            }
            return data.GetLog(index);
        }
        public void SetPath(string path)
        {
            //throw new System.NotImplementedException();
        }
    }

    
}