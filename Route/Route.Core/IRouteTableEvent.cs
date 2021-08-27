#region header
// /*********************************************************************************************/
// Project :Route.Core
// FileName : IRouterTableEvent.cs
// Time Create : 2:05 PM 26/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Generic;

namespace Route.Core
{
    public interface IRouteTableEvent<T>
    {
        event Action<DataCenterInfo,T> OnAdd;
        event Action<DataCenterInfo,IList<T>> OnAdds;
        event Action<DataCenterInfo, T> OnRemove;
    }
}