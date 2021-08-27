#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Core
// FileName : INodeServer.cs
// Time Create : 2:46 PM 28/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace NodeServerPlatform.Core
{
    public interface INodeShareServer
    {
        string Name { get; }
        void SendAll(INodeShareSendPacket p);
        void SendTo(string nodeName, INodeShareSendPacket p);
    }
}