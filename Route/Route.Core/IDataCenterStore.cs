#region header
// /*********************************************************************************************/
// Project :Route.Core
// FileName : IDataCenterStore.cs
// Time Create : 4:07 PM 24/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Generic;

namespace Route.Core
{
    public interface IDataCenterStore
    {
        bool SaveOrUpdate(DataCenterInfo center);
        bool SaveOrUpdateNoneBroadCast(DataCenterInfo center);
        bool Del(Guid id);
        bool DelNoneBroadCast(Guid id);
        //bool Del(DataCenterInfo center);
        DataCenterInfo Get(Guid id);
        IList<DataCenterInfo> GetAll();
    }
}