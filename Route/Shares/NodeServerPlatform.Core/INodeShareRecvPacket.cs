#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Core
// FileName : IRecvPacket.cs
// Time Create : 2:46 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace NodeServerPlatform.Core
{
    public interface INodeShareRecvPacket
    {
        bool Deserializer();
    }
}