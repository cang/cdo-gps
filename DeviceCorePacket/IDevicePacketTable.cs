#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : IPacketTable.cs
// Time Create : 4:22 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace CorePacket
{
    public interface IDevicePacketTable
    {
        Type GetPacket(int opcode);
        int GetOpcode(Type type);
    }
}