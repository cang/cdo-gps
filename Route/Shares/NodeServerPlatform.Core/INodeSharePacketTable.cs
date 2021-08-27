#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : IPacketTable.cs
// Time Create : 4:22 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace NodeServerPlatform.Core
{
    public interface INodeSharePacketTable
    {
        Type GetPacket(int opcode);
        int GetOpcode(Type type);
    }
}