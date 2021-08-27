﻿#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : IPacket.cs
// Time Create : 3:19 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace CorePacket
{
    /// <summary>
    ///     Cấu trúc của gói tin truyền và nhận trong cụm server
    /// </summary>
    public interface IDevicePacket
    {
        /// <summary>
        ///     Id của gói tin truyền
        /// </summary>
        int Opcode { get; set; }

        /// <summary>
        ///     Nội dung của gói tin
        /// </summary>
        byte[] Data { get; set; }
    }
}