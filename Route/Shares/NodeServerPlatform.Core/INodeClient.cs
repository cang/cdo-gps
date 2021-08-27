#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Core
// FileName : INodeClient.cs
// Time Create : 10:57 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace NodeServerPlatform.Core
{
    public interface INodeClient : IDisposable
    {
        string Name { get; set; }
        bool Send(INodeShareSendPacket p);
        void Start();
        string GetRemoteIp();
        event Action OnConected;
    }
}