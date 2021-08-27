#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : NodeServer.cs
// Time Create : 3:15 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.ComponentModel.Composition;
using NodeServerPlatform.Core;

namespace NodeServerPlatform.Server
{
    [Export(typeof(INodeShareServer))]
    [Export(typeof(NodeShareServer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NodeShareServer: BaseNodeServer
    {
        
    }
}