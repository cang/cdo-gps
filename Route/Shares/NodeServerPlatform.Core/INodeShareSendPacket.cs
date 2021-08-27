#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Core
// FileName : ISendPacket.cs
// Time Create : 11:14 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace NodeServerPlatform.Core
{
    public interface INodeShareSendPacket
    {
        byte[] Serializer();
    }
}