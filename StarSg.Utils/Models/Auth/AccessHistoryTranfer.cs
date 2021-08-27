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
    public class AccessHistoryTranfer
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Tại thời điểm
        /// </summary>
        public virtual DateTime AtTime { get; set; }

        /// <summary>
        /// Thuộc cty, nếu = 0 thì tất cả
        /// </summary>
        public virtual long CompanyId { get; set; }

        /// <summary>
        /// Thuộc nhóm, nếu = 0 thì tất cả
        /// </summary>
        public virtual long GroupId { get; set; }

        /// <summary>
        /// Serial thiết bị , = 0 nếu không có
        /// </summary>
        public virtual long Serial { get; set; }

        /// <summary>
        /// Phương thức đổi Add,Edit,Delete,Setup,Get,List...
        /// </summary>
        public virtual string Method { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Ghi Chú
        /// </summary>
        public virtual string Note { get; set; }
    }


    public class SetupDeviceTranfer
    {
        public virtual int Id { get; set; }

        public virtual string Username { get; set; }

        public virtual DateTime AtTime { get; set; }

        public virtual long CompanyId { get; set; }

        public virtual long GroupId { get; set; }

        public virtual long Serial { get; set; }

        public virtual short opcode { get; set; }

        public virtual string Note { get; set; }

        public virtual int Retry { get; set; }

    }


}