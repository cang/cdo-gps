#region header

// /*********************************************************************************************/
// Project :Core
// FileName : UserInfoTranfer.cs
// Time Create : 8:40 AM 30/05/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;

namespace StarSg.Utils.Models.Auth
{
    public class UserInfoTranfer
    {
        /// <summary>
        ///     Tên tài khoản
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     cấp bậc
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        ///     thông tin công ty được quản lý
        /// </summary>
        public IList<long> CompanyId { get; set; }

        /// <summary>
        ///     thông tin thiết bị được quản lý
        /// </summary>
        public IList<long> DeviceSerial { get; set; }

        /// <summary>
        ///     tài khoản hợp lệ
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Mã chi nhánh, dành cho website riêng của đại lý
        /// </summary>
        public string BranchCode { get; set; }

        public string DisplayName { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// Sử dụng phân quyền cấp thấp, tạm thời cho quyền gia hạn
        /// </summary>
        public string GroupUserId { get; set; }

    }
}