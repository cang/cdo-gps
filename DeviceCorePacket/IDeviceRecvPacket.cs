#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : IRecvPacket.cs
// Time Create : 2:46 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace CorePacket
{
    public interface IDeviceRecvPacket
    {
        long Serial { get; }
        bool Deserializer();
    }
}