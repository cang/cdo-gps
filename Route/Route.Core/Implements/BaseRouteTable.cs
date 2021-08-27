#region header

// /*********************************************************************************************/
// Project :Route.Core
// FileName : BaseRouteTable.cs
// Time Create : 9:00 AM 29/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Route.Core.Implements
{
    public class BaseRouteTable<T>
    {
        protected ConcurrentDictionary<T, DataCenterInfo> AllData { get; } =
            new ConcurrentDictionary<T, DataCenterInfo>();

        public bool Check(T key)
        {
            return AllData.ContainsKey(key);
        }

        public DataCenterInfo GetDataCenter(T key)
        {
            DataCenterInfo val;
            if (AllData.TryGetValue(key, out val))
                return val;
            return null;
        }


        public event Action<DataCenterInfo, T> OnAdd;
        public event Action<DataCenterInfo, T> OnRemove;
        public event Action<DataCenterInfo, IList<T>> OnAdds;

        protected bool Push(DataCenterInfo datacenter, T key, bool broadcast)
        {
            var data = GetDataCenter(key);
            if (data != null)
                return AllData.TryUpdate(key, datacenter, data);
            if (AllData.TryAdd(key, datacenter))
            {
                if (broadcast)
                    OnAdd?.Invoke(datacenter, key);
                return true;
            }
            return false;
        }

        protected bool Push(DataCenterInfo datacenter, IList<T> key, bool broadcast)
        {
            var tmp=new List<T>();
            foreach (var k in key)
            {
                var data = GetDataCenter(k);
                if (data != null)
                {
                    AllData.TryUpdate(k, datacenter, data);
                    continue;
                }
                if (AllData.TryAdd(k, datacenter))
                {
                    tmp.Add(k);
                }
            }
            if (broadcast && tmp.Count > 0)
                OnAdds?.Invoke(datacenter, tmp);
          
            return true;
        }

        public bool Push(DataCenterInfo datacenter, T key)
        {
            return Push(datacenter, key, true);
        }

        public bool Push(DataCenterInfo datacenter, IList<T> key)
        {
            return Push(datacenter, key, true);
        }

        public bool PushNoneBroadCast(DataCenterInfo datacenter, T key)
        {
            return Push(datacenter, key, false);
        }

        public bool PushNoneBroadCast(DataCenterInfo datacenter, IList<T> key)
        {
            return Push(datacenter, key, false);
        }

        protected bool Remove(T key, bool broadcast)
        {
            if (Check(key))
            {
                DataCenterInfo val;
                if (AllData.TryRemove(key, out val))
                {
                    if (broadcast)
                        OnRemove?.Invoke(val, key);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(T key)
        {
            return Remove(key, true);
        }

        public bool RemoveNoneBroadCast(T key)
        {
            return Remove(key, false);
        }


        public bool CleanByDatacenterId(Guid id)
        {
            try
            {

                var tmp = GetByDataId(id);
                if (tmp == null) return false;
                foreach (var keyValuePair in tmp)
                {
                    DataCenterInfo t;
                    AllData.TryRemove(keyValuePair, out t);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<T> GetByDataId(Guid id)
        {
            try
            {
                var tmp = AllData.ToArray().Where(m => m.Value.Id == id);
                return tmp.Select(m => m.Key).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        

    }
}