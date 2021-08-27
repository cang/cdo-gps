#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Core
// FileName : IHandleTable.cs
// Time Create : 1:56 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace NodeServerPlatform.Core
{
    public interface INodeShareHandleTable
    {
        INodeShareHandlePacket GetHandle(int opcode);
    }
}