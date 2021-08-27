using Route.Api.Auth.Models.Req;
using System.Collections.Generic;

namespace Route.Api.Auth.Core
{
    /// <summary>
    /// định nghĩa các hàm phân quyền truy cập
    /// </summary>
    public interface IPermissionManager
    {
        /// <summary>
        /// tạo mới nhóm user
        /// </summary>
        /// <param name="groupUser"></param>
        /// <returns></returns>
        bool NewGroupUser(GroupUserTranfer groupUser);

        /// <summary>
        /// cập nhật nhóm user
        /// </summary>
        /// <param name="groupUser"></param>
        /// <returns></returns>
        bool UpdateGroupUser(GroupUserTranfer groupUser);

        /// <summary>
        /// xóa nhóm user
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        bool DeleteGroupUser(string groupUserId);

        /// <summary>
        /// tạo mới quyền truy cập
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool NewPermission(FunctionsTranfer permission);

        /// <summary>
        /// cập nhật quyền truy cập
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool UpdatePermission(FunctionsTranfer permission);

        /// <summary>
        /// xóa quyền truy cập
        /// </summary>
        /// <param name="permisionId"></param>
        /// <returns></returns>
        bool DeletePermission(string permisionId);

        /// <summary>
        /// lấy tất cả nhóm user
        /// </summary>
        /// <returns></returns>
        List<GroupUserTranfer> AllGroupUser();

        /// <summary>
        /// lấy tất cả bảng quyền truy cập
        /// </summary>
        /// <returns></returns>
        List<FunctionsTranfer> AllPermission();

        /// <summary>
        /// lấy tất cả quyền truy cập theo nhóm user
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        List<FunctionsTranfer> ALlPermissionByGroupUser(string groupUserId);
    }
}