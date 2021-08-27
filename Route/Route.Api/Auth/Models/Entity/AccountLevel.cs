#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : AccountLevel.cs
// Time Create : 9:00 AM 22/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion
namespace Route.Api.Auth.Models.Entity
{
    /// <summary>
    /// cấp bậc tài khoản
    /// </summary>
    public enum AccountLevel
    {
        /// <summary>
        /// tài khoản quản trị
        /// </summary>
        Root = 0,

        /// <summary>
        /// quản lý
        /// </summary>
        Administrator = 1,

        /// <summary>
        /// tài khoản quản trị khách hàng
        /// </summary>
        CustomerMaster = 2,



        /// <summary>
        /// tài khoản bình thường
        /// </summary>
        Customer = 3,

        /// <summary>
        /// tài khoản bình thường đơn giản
        /// </summary>
        Guest = 3001,

        /// <summary>
        /// tài khoản bình thường đơn giản chỉ đọc
        /// </summary>
        ReadonlyGuest = 3002

    }
}