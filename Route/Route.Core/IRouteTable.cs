using System;
using System.Collections.Generic;

namespace Route.Core
{
    public interface IRouteTable<T>
    {
        bool Check(T key);
        DataCenterInfo GetDataCenter(T key);
        IList<T> GetByDataId(Guid id);
    }
}