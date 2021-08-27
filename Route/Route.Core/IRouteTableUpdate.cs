using System;
using System.Collections.Generic;

namespace Route.Core
{
    public interface IRouteTableUpdate<T>
    {
        bool Push(DataCenterInfo datacenter, T key);
        bool Push(DataCenterInfo datacenter, IList<T> key);
        /// <summary>
        /// thêm mới nhưng ko chạy sự kiện 
        /// </summary>
        /// <param name="datacenter"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool PushNoneBroadCast(DataCenterInfo datacenter, T key);
        bool PushNoneBroadCast(DataCenterInfo datacenter, IList<T> key);
        bool Remove(T key);
        bool CleanByDatacenterId(Guid id);
        /// <summary>
        /// gỡ bỏ nhưng ko chạy sự kiện 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveNoneBroadCast(T key);
    }
}