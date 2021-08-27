using System;
using System.Collections.Generic;

namespace Route.Core
{
    public interface IDataCenterStoreEvent
    {
        event Action<DataCenterInfo> OnAdd;
        event Action<DataCenterInfo> OnRemove;
        event Action<IList<DataCenterInfo>> OnSync;
        event Action<IList<DataCenterInfo>> OnSyncSerial;
        event Action<IList<DataCenterInfo>> OnSyncCompanyId;
    }
}