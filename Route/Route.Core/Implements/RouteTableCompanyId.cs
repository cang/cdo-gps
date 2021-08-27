#region header

// /*********************************************************************************************/
// Project :Route.Core
// FileName : RouteTableCompanyId.cs
// Time Create : 8:58 AM 29/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Route.Core.Implements
{
    [Export(typeof (ICompanyRouteTable))]
    [Export(typeof (ICompanyRouteTableEvent))]
    [Export(typeof (ICompanyRouteTableUpdate))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class RouteTableCompanyId : BaseRouteTable<long>, ICompanyRouteTable, ICompanyRouteTableEvent,
        ICompanyRouteTableUpdate
    {
    }
}