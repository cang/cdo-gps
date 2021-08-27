using Route.Api.Auth;
using Route.Api.Auth.Core;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Req;
using Route.Api.Auth.Models.Response;
using StarSg.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Route.Api.Controllers
{
    /// <summary>
    ///     phân quyền truy cập
    /// </summary> 
    [ValidRequestFilter]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PermissionController : ValidController
    {
        /// <summary>
        ///     kiểm tra tài khoản admin
        /// </summary>
        /// <returns></returns>
        private bool IsAdminAccount()
        {
            return Account.Level < AccountLevel.CustomerMaster;
        }

        /// <summary>
        ///     thêm nhóm user
        /// </summary>
        /// <param name="groupUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(BaseResponse))]
        [Route("Permission/NewRole")]
        public IHttpActionResult NewGroupUser(GroupUserTranfer groupUser)
        {
            if (IsAdminAccount())
            {
                if (groupUser == null)
                {
                    Log.Warning("PermissionController", $"thêm nhóm user null");
                    return Ok(new BaseResponse { Status = 0 });
                }
                if (PermissionManager.NewGroupUser(groupUser))
                {
                    Log.Warning("PermissionController", $"thêm nhóm user: {groupUser.Name} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("PermissionController", $"thêm nhóm user: {groupUser.Name} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     cập nhật nhóm user
        /// </summary>
        /// <param name="groupUser"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(BaseResponse))]
        [Route("Permission/UpdateRole")]
        public IHttpActionResult UpdateGroupUser(GroupUserTranfer groupUser)
        {
            if (IsAdminAccount())
            {
                if (groupUser == null)
                {
                    Log.Warning("PermissionController", $"cập nhật nhóm user null");
                    return Ok(new BaseResponse { Status = 0 });
                }
                if (PermissionManager.UpdateGroupUser(groupUser))
                {
                    Log.Warning("PermissionController", $"cập nhật nhóm user: {groupUser.Name} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("PermissionController", $"cập nhật nhóm user: {groupUser.Name} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     xóa nhóm user
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [ResponseType(typeof(BaseResponse))]
        [Route("Permission/DeleteRole")]
        public IHttpActionResult DeleteGroupUser(string roleId)
        {
            if (IsAdminAccount())
            {
                if (PermissionManager.DeleteGroupUser(roleId))
                {
                    Log.Warning("PermissionController", $"xóa nhóm user: {roleId} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("PermissionController", $"xóa nhóm user: {roleId} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     tạo mới quyền truy cập
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(BaseResponse))]
        [Route("Permission/NewFunction")]
        public IHttpActionResult NewPermission(FunctionsTranfer permission)
        {
            if (IsAdminAccount())
            {
                if (permission == null)
                {
                    Log.Warning("PermissionController", $"tạo mới quyền truy cập null");
                    return Ok(new BaseResponse { Status = 0 });
                }
                if (PermissionManager.NewPermission(permission))
                {
                    Log.Warning("PermissionController", $"tạo mới quyền truy cập: {permission.Name} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("PermissionController", $"tạo mới quyền truy cập: {permission.Name} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     cập nhật quyền truy cập
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(BaseResponse))]
        [Route("Permission/UpdateFunction")]
        public IHttpActionResult UpdatePermission(FunctionsTranfer permission)
        {
            if (IsAdminAccount())
            {
                if (permission == null)
                {
                    Log.Warning("PermissionController", $"cập nhật quyền truy cập null");
                    return Ok(new BaseResponse { Status = 0 });
                }
                if (PermissionManager.UpdatePermission(permission))
                {
                    Log.Warning("PermissionController", $"cập nhật quyền truy cập: {permission.Name} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("PermissionController", $"cập nhật quyền truy cập: {permission.Name} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     xóa bỏ quyền truy cập
        /// </summary>
        /// <param name="functionId">Tên chức năng</param>
        /// <returns></returns>
        [HttpDelete]
        [ResponseType(typeof(BaseResponse))]
        [Route("Permission/DeleteFunction")]
        public IHttpActionResult DeletePermission(string functionId)
        {
            if (IsAdminAccount())
            {
                if (PermissionManager.DeletePermission(functionId))
                {
                    Log.Warning("PermissionController", $"xóa quyền truy cập: {functionId} thành công");
                    return Ok(new BaseResponse { Status = 1, Description = "OK" });
                }
            }
            Log.Warning("PermissionController", $"xóa quyền truy cập: {functionId} thất bại");
            return Ok(new BaseResponse { Status = 0 });
        }

        /// <summary>
        ///     lấy toàn bộ danh sách nhóm user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(GroupUserResponse))]
        [Route("Permission/AllRole")]
        public IHttpActionResult AllGroupUser()
        {
            // if (IsAdminAccount())
            {
                var result = new List<GroupUserTranfer>();

                if(Account.Level < AccountLevel.Administrator)
                    result = PermissionManager.AllGroupUser();
                else if (Account.Level < AccountLevel.CustomerMaster)
                {
                    var acs = AdministrationManager.GetAllUserByLevel(AccountLevel.Administrator);//todo: cần check lại chỗ này
                    result = PermissionManager.AllGroupUser().Where(m =>
                    {
                        if (acs.Any(accountTranfer => accountTranfer.RoleId == m.Parent))
                        {
                            return true;
                        }
                        return m.Parent == Account.GroupUserId;
                    }).ToList();
                }
                else
                    result = PermissionManager.AllGroupUser().Where(m => m.Parent == Account.GroupUserId).ToList();

                if (result != null)
                {
                    Log.Warning("PermissionController", $"lấy toàn bộ danh sách nhóm user thành công");
                    return Ok(new GroupUserResponse { Status = 1, GroupUserTranfers = result });
                }
            }
            Log.Warning("PermissionController", $"lấy toàn bộ danh sách nhóm user thất bại");
            return Ok(new GroupUserResponse { Status = 0 });
        }

        /// <summary>
        ///     lấy toàn bộ danh sách quyền truy cập
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(PermissionResponse))]
        [Route("Permission/AllFunction")]
        public IHttpActionResult AllPermission()
        {
            if (IsAdminAccount())
            {
                var result = PermissionManager.AllPermission();
                if (result != null)
                {
                    Log.Warning("PermissionController", $"lấy toàn bộ danh sách quyền truy cập thành công");
                    return Ok(new PermissionResponse { Status = 1, FunctionsTranfers = result });
                }
            }
            Log.Warning("PermissionController", $"lấy toàn bộ danh sách quyền truy cập thất bại");
            return Ok(new PermissionResponse { Status = 0 });
        }

        /// <summary>
        ///     lấy toàn bộ quyền truy cập theo nhóm user
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(PermissionResponse))]
        [Route("Permission/AllFunctionByRole")]
        public IHttpActionResult ALlPermissionByGroupUser(string roleId)
        {
            // if (IsAdminAccount())
            {
                var result = PermissionManager.ALlPermissionByGroupUser(roleId);
                if (result != null)
                {
                    Log.Warning("PermissionController", $"lấy toàn bộ danh sách quyền truy cập theo nhóm user: {roleId} thành công");
                    return Ok(new PermissionResponse { Status = 1, FunctionsTranfers = result });
                }
            }
            Log.Warning("PermissionController", $"lấy toàn bộ danh sách quyền truy cập theo nhóm user: {roleId} thất bại");
            return Ok(new PermissionResponse { Status = 0 });
        }

        /// <summary>
        ///     lấy toàn bộ danh sách Level
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(AccountLevelResponse))]
        [Route("Permission/AllLevel")]
        public IHttpActionResult AllLevels()
        {
            var values = Enum.GetValues(typeof(AccountLevel)).Cast<AccountLevel>().ToList();
            if (Account.Level > AccountLevel.Root)
                values = values.Where(m => m > Account.Level).ToList();

            if (values != null)
            {
                Log.Warning("PermissionController", $"lấy toàn bộ danh sách nhóm level thành công");
                List<AccountLevelTransfer> ret = new List<AccountLevelTransfer>(values.Count);
                foreach (var item in values)
                {
                    ret.Add(new AccountLevelTransfer()
                    {
                        Id = (int)item,
                        Name = Enum.GetName(typeof(AccountLevel), item)
                    });
                }
                return Ok(new AccountLevelResponse { Status = 1, AccountLevels = ret });
            }

            Log.Warning("PermissionController", $"lấy toàn bộ danh sách nhóm level thất bại");
            return Ok(new GroupUserResponse { Status = 0 });
        }

    }
}
