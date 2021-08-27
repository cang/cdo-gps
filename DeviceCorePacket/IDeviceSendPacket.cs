#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : ISendPacket.cs
// Time Create : 11:14 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace CorePacket
{
    public interface IDeviceSendPacket
    {
        byte[] Serializer();
    }
}