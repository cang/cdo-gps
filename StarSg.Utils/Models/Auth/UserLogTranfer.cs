#region header

// /*********************************************************************************************/
// Project :Authentication
// FileName : LoginReq.cs
// Time Create : 3:53 PM 14/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace Core.Models.Auth
{
    /// <summary>
    ///     user log
    /// </summary>
    public class UserLogTranfer
    {
        /// <summary>
        ///     id trong database
        /// </summary>
        public long Id { set; get; }

        /// <summary>
        ///     Token
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        ///     nội dung ghi log
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     loại log ghi vào lấy từ enum AuthLogType
        /// </summary>
        public int TypeLog { get; set; }

        /// <summary>
        ///     thời gian xử lý
        /// </summary>
        public int TimeHandle { get; set; }

        /// <summary>
        ///     ngày ghi log
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}