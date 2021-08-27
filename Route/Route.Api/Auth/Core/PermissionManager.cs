using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Log;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Req;

namespace Route.Api.Auth.Core
{
    /// <summary>
    ///     quản lý quyền truy cập
    /// </summary>
    [Export(typeof(IPermissionManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class PermissionManager : IPermissionManager, IPartImportsSatisfiedNotification
    {
        [Import]
        private ILog _log;

        [Import]
        private Loader _loader;

        /// <summary>
        ///     tạo mới nhóm user
        /// </summary>
        /// <param name="groupUser"></param>
        /// <returns></returns>
        public bool NewGroupUser(GroupUserTranfer groupUser)
        {
            var error = 0;
            try
            {
//check null đầu vào
                if (groupUser == null)
                {
                    return false;
                }
                //thêm nhóm user vào database
                var context = _loader.GetContext();
                if (context.Get<Functions>(groupUser.Name) != null)
                {
                    context.Dispose();
                    return false;
                }
                var fun = new List<Functions>();
                foreach (var function in groupUser.Functions)
                {
                    var tmp = context.Get<Functions>(function);
                    if (tmp != null) fun.Add(tmp);
                }
                var gr= new Role
                {
                    Name = groupUser.Name,
                    Description = groupUser.Description,
                    Parent=groupUser.Parent
                };


                gr.Functions = fun.Select(m => new RoleFun()
                {
                    Id = 0,
                    Fun = m,
                    Role = gr
                }).ToList();
                //foreach (var function in gr.Functions)
                //{
                //    context.Insert(function);
                //}
                //context.Commit();

                context.Insert(gr);
                context.Commit();
                context.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _log.Exception("CreateRole", e, "tạo nhóm tài khoản");
                return false;
            }
        }

        /// <summary>
        ///     cập nhật nhóm user
        /// </summary>
        /// <param name="groupUser"></param>
        /// <returns></returns>
        public bool UpdateGroupUser(GroupUserTranfer groupUser)
        {
            //check null đầu vào
            try
            {
                if (groupUser == null)
                {
                    return false;
                }
                var context = _loader.GetContext();
                //lấy nhóm user từ database
                var oldGroupUser = context.GetWhere<Role>(m => m.Name == groupUser.Name).FirstOrDefault();
                if (oldGroupUser == null)
                {
                    return false;
                }
                //update các trường data
                var fun = new List<Functions>();
                foreach (var function in groupUser.Functions.Where(m => oldGroupUser.Functions.FirstOrDefault(x => x.Fun.Name == m) == null))
                {
                    var tmp = context.Get<Functions>(function);
                    if (tmp != null) fun.Add(tmp);
                }
                var remove = oldGroupUser.Functions.Where(m => !groupUser.Functions.Contains(m.Fun.Name)).ToList();
                foreach (var function in remove)
                {
                    context.Delete(function);
                    oldGroupUser.Functions.Remove(function);
                }
                context.Commit();

                foreach (var function in fun)
                {
                    var tmp = new RoleFun
                    {
                        Id = 0,
                        Fun = function,
                        Role = oldGroupUser
                    };
                    oldGroupUser.Functions.Add(tmp);
                    //context.Insert(tmp);
                }
                //context.Commit();

            
                oldGroupUser.Name = groupUser.Name;
                oldGroupUser.Description = groupUser.Description;
                //oldGroupUser.Parent = groupUser.Parent;
                context.Update(oldGroupUser);
                context.Commit();
                context.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _log.Exception("UpdateGroupUser", e, "cập nhật role lỗi");
                return false;
            }
        }

        /// <summary>
        ///     xóa nhóm user
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        public bool DeleteGroupUser(string groupUserId)
        {
            var error = 0;
            try
            {
                var context = _loader.GetContext();
                var oldGroupUser = context.GetWhere<Role>(m => m.Name == groupUserId).FirstOrDefault();
                if (oldGroupUser == null)
                {
                    return false;
                }
                //foreach (var function in oldGroupUser.Functions)
                //{
                //    context.Delete(function);
                //}
                //context.Commit();
                error = 1;
                context.Delete(oldGroupUser);
                context.Commit();

                context.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _log.Exception("DeleteRole", e, $"Xóa role lỗi : {error}");
                return false;
            }
        }

        /// <summary>
        ///     thêm mới phân quyền
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool NewPermission(FunctionsTranfer permission)
        {
            //check null đầu vào
            if (permission == null)
            {
                return false;
            }
            var context = _loader.GetContext();
            //thêm phân quyền mới vào database
            if (context.Get<Functions>(permission.Name) != null)
            {
                context.Dispose();
                return false;
            }
            context.Insert(new Functions
            {
                Description = permission.Description,
                Name = permission.Name
            });
            context.Commit();
            context.Dispose();
            return true;
        }

        /// <summary>
        ///     cập nhật phân quyền
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool UpdatePermission(FunctionsTranfer permission)
        {
            //check null đầu vào
            if (permission == null)
            {
                return false;
            }
            var context = _loader.GetContext();
            //lấy lại permission cũ từ database
            var permissionOld = context.GetWhere<Functions>(m => m.Name == permission.Name).FirstOrDefault();
            if (permissionOld == null)
            {
                return false;
            }
            // cập nhật lại các biến
            permissionOld.Description = permission.Description;
            permissionOld.Name = permission.Name;
            context.Update(permissionOld);
            context.Commit();
            context.Dispose();
            return true;
        }

        /// <summary>
        ///     xóa phân quyền theo id
        /// </summary>
        /// <param name="permisionId"></param>
        /// <returns></returns>
        public bool DeletePermission(string permisionId)
        {
            var index = 0;
            try
            {
                var context = _loader.GetContext();
                //lấy lại permission cũ từ database
                var permissionOld = context.GetWhere<Functions>(m => m.Name == permisionId).FirstOrDefault();
                if (permissionOld == null)
                {
                    return false;
                }

                //foreach (var r in context.GetWhere<RoleFun>(m => m.Fun.Name == permissionOld.Name))
                //{
                //    //r.Role = null;
                //    //r.Fun = null;
                //    context.Delete(r);
                //}
                index = 1;
                //xóa data khỏi database

                context.Commit();
                index = 2;
                context.Delete(permissionOld);
                context.Commit();
                index = 3;
                context.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _log.Exception("PERMISSION",e,$"delete permission error: {index}");
                return false;
            }
        }

        /// <summary>
        ///     lấy tất cả các nhóm user
        /// </summary>
        /// <returns></returns>
        public List<GroupUserTranfer> AllGroupUser()
        {
            var context = _loader.GetContext();
            var result =
                context.GetAll<string, Role>(m => m.Name).Select(c => c.Value).Select(k => new GroupUserTranfer
                {
                    Description = k.Description,
                    Name = k.Name,
                    Functions = k.Functions.Select(m => m.Fun.Name).ToList(),
                    Parent = k.Parent
                }).ToList();
            context.Dispose();
            return result;
        }

        /// <summary>
        ///     lấy tất cả các phân quyền
        /// </summary>
        /// <returns></returns>
        public List<FunctionsTranfer> AllPermission()
        {
            var context = _loader.GetContext();
            var result = context.GetAll<string, Functions>(m => m.Name).Select(c => c.Value).Select(k => new FunctionsTranfer
            {
                Name = k.Name,
                Description = k.Description
            }).ToList();
            context.Dispose();
            return result;
        }

        /// <summary>
        ///     lấy danh sách phân quyền theo nhóm user
        /// </summary>
        /// <param name="groupUserId"></param>
        /// <returns></returns>
        public List<FunctionsTranfer> ALlPermissionByGroupUser(string groupUserId)
        {
            var result = new List<FunctionsTranfer>();
            var context = _loader.GetContext();
            //lấy nhóm user
            var groupUser = context.GetWhere<Role>(m => m.Name == groupUserId).FirstOrDefault();
            if (groupUser?.Functions != null)
            {
                result.AddRange(groupUser.Functions.Select(m => new FunctionsTranfer
                {
                    Description = m.Fun.Description,
                    Name = m.Fun.Name
                }));
            }
            context.Dispose();
            return result;
        }

        public void OnImportsSatisfied()
        {
        }
    }
}