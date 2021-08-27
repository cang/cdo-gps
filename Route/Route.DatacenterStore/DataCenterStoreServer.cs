#region header

// /*********************************************************************************************/
// Project :Route.DatacenterStore
// FileName : DataCenterStoreServer.cs
// Time Create : 8:39 AM 24/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Server;

namespace Route.DatacenterStore
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class DataCenterStoreServer : BaseNodeServer
    {
    }
}