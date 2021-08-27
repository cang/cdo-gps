#region header

// /*********************************************************************************************/
// Project :Route.Core
// FileName : RouteTableDevice.cs
// Time Create : 8:44 AM 29/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Route.Core.Implements
{
    [Export(typeof (IDeviceRouteTable))]
    [Export(typeof (IDeviceRouteTableEvent))]
    [Export(typeof (IDeviceRouteTableUpdate))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class RouteTableDevice : BaseRouteTable<long>, IDeviceRouteTable, IDeviceRouteTableEvent,
        IDeviceRouteTableUpdate
    {
        #region Implementation of IDeviceRouteTable

        public IList<long> GetAll()
        {
            var tmp=AllData.GetEnumerator();
            var result = new List<long>();
            while (tmp.MoveNext())
            {
                result.Add(tmp.Current.Key);
            }
            return result;
        }

        #endregion
    }
}