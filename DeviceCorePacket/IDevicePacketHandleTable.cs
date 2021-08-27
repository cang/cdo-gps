#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : IHandleTable.cs
// Time Create : 1:56 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace CorePacket
{
    public interface IDevicePacketHandleTable
    {
        IDeviceHandlePacket GetHandle(int opcode);
    }
}