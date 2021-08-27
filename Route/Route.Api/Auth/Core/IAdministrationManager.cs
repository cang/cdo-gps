using System;
using System.Collections.Generic;
using Core.Models.Auth;
using Route.Api.Auth.Models.Req;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Auth.Core
{
    /// <summary>
    /// quản trị 
    /// </summary>
    public interface IAdministrationManager
    {
        /// <summary>
        ///     khóa tài khoản
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool LockUser(string username);

        /// <summary>
        ///     thêm user log vào database
        /// </summary>
        /// <param name="userLogTranfer"></param>
        /// <returns></returns>
        bool AddUserLog(UserLogTranfer userLogTranfer);

        /// <summary>
        ///     lấy danh sách log ứng theo username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="begineDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        List<UserLogTranfer> GetUserLog(string username, DateTime begineDateTime, DateTime endDateTime);

        /// <summary>
        ///     lấy toàn bộ danh sách tài khoản theo công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        List<AccountTranfer> GetUserByCompany(long companyId);

        /// <summary>
        ///     lấy toàn bộ danh sách tài khoản
        /// </summary>
        /// <returns></returns>
        List<AccountTranfer> GetAllUser();

        /// <summary>
        /// lấy danh sách user theo cấp bâch
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        List<AccountTranfer> GetAllUserByLevel(AccountLevel level);

        List<AccountTranfer> GetAllUser(string parent);
        /// <summary>
        ///     tạo mới tài khoản
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        bool CreateUser(AccountTranfer ac);

        /// <summary>
        /// lấy user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        AccountTranfer GetUser(string username);

        /// <summary>
        ///     cập nhật tài khoản
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool UpdateUser(AccountTranfer account);

        /// <summary>
        ///     xóa tài khoản
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool DeleteUser(string username);

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="method">tùy chọn</param>
        /// <returns></returns>
        List<AccessHistoryTranfer> GetAccessHistory(string username, DateTime begin, DateTime end,String method);


        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo cty (cty= 0 là lấy theo serial), nhóm (nhóm = 0 là lấy tất cả) hoặc theo serial nếu có nhập
        /// </summary>
        /// <param name="companyId">0 lấy theo serial nếu có nhập serial</param>
        /// <param name="groupId">0 lấy tất cả theo cty</param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="method">phương thức thay đổi : rỗng thì bõ qua</param>
        /// <param name="serial">serial : 0 thì bõ qua</param>
        /// <returns></returns>
        List<AccessHistoryTranfer> GetAccessHistory(long companyId, long groupId, DateTime begin, DateTime end, string method, long serial);

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo serial
        /// </summary>
        /// <param name="serial">serial</param>
        /// <returns></returns>
        List<AccessHistoryTranfer> GetAccessHistoryBySerial(long serial);

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo giá trị bên trong nội dung
        /// </summary>
        /// <param name="content">Giá trị nội dung</param>
        /// <returns></returns>
        List<AccessHistoryTranfer> GetAccessHistoryByContent(String content);


        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo cty (cty= 0 là lấy theo serial), nhóm (nhóm = 0 là lấy tất cả) hoặc theo serial nếu có nhập
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="opcode">phương thức thay đổi : rỗng thì bõ qua</param>
        /// <param name="companyId">0 lấy theo serial nếu có nhập serial</param>
        /// <param name="groupId">0 lấy tất cả theo cty</param>
        /// <param name="serial">serial : 0 thì bõ qua</param>
        /// <returns></returns>
        List<SetupDeviceTranfer> GetSetupDeviceHistory(DateTime begin, DateTime end, short opcode, long companyId, long groupId, long serial);

    }
}