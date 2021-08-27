#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : IHandlePacket.cs
// Time Create : 11:19 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace CorePacket
{
    public interface IDeviceHandlePacket
    {
        Delegate GetHandle();
    }
}