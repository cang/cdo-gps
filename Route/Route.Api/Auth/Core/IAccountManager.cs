#region header

// /*********************************************************************************************/
// Project :Authentication
// FileName : IAccountManager.cs
// Time Create : 8:38 AM 18/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Req;
using System;
using System.Collections.Generic;

namespace Route.Api.Auth.Core
{
    /// <summary>
    ///     quản lý thông tin đăng nhập của user
    /// </summary>
    public interface IAccountManager
    {
        /// <summary>
        ///     kiểm tra token của tài khoản
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool ValidToken(string token);

        /// <summary>
        ///     tạo mới 1 token theo tên tài khoản đăng nhập
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="pwd">mật khẩu</param>
        /// <returns></returns>
        string Login(string username, string pwd);

        /// <summary>
        ///     bắt buộc tài khoản đăng xuất
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool ForceLogout(string username);

        /// <summary>
        ///     bắt buộc tài khoản có token này đăng xuất
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool ForceLogoutWithToken(string token);

        /// <summary>
        ///     user logout
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool Logout(string token);

        /// <summary>
        ///     thay đổi mật khẩu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        bool ChangePassword(string username, string pwd);

        /// <summary>
        /// lấy user online
        /// </summary>
        /// <returns></returns>
        List<AccountTranfer> GetUserOnline();

        /// <summary>
        /// lấy thông tin user theo token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Models.Entity.Account GetUser(string token);

        /// <summary>
        /// thêm user log
        /// </summary>
        /// <param name="userLog"></param>
        void AddUserLogCache(UserLog userLog);

        /// <summary>
        /// lấy list userlog
        /// </summary>
        /// <returns></returns>
        List<UserLog> GetUserLogs();

        void CreateStaticToken();

        IList<String> GetUsersOfGroup(long companyid,long groupid);

        IList<String> GetUsersOfSerial(long serial);

        /// <summary>
        /// Cập nhật branchCode cho companyId
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        bool EnsureBranch(long companyId, String branchCode);

        /// <summary>
        /// Lấy branchCode theo companyId
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Branch GetBranchOfCompany(long companyId);

        void LoadBranch();

        List<BranchData> AllBranch();
        bool AddBranch(BranchData branch);
        bool UpdateBranch(BranchData branch);
        bool DeleteBranch(String id);
        BranchData GetBranch(String id);

        bool Update(string username, AccountInfo acc);

        AccountInfo GetUserInformation(string username);

        void LoadSystemPair();
        List<SystemPair> AllSystemPair();
        bool AddSystemPair(SystemPair SystemPair);
        bool UpdateSystemPair(SystemPair SystemPair);
        bool DeleteSystemPair(String id);
        SystemPair GetSystemPair(String id);

    }
}