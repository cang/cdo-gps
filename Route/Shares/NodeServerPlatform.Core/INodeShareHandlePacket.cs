#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Core
// FileName : IHandlePacket.cs
// Time Create : 11:19 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace NodeServerPlatform.Core
{
    public interface INodeShareHandlePacket
    {
        Delegate GetHandle();
    }
}