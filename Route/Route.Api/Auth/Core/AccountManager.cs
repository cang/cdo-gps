#region header

// /*********************************************************************************************/
// Project :Authentication
// FileName : AccountManager.cs
// Time Create : 3:31 PM 20/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Log;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Req;
using DaoDatabase;

namespace Route.Api.Auth.Core
{
    /// <summary>
    ///     xử lý quản lý account
    /// </summary>
    [Export(typeof (IAccountManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AccountManager : IAccountManager, IPartImportsSatisfiedNotification
    {
        private readonly ConcurrentDictionary<string, Tuple<Models.Entity.Account, long>> _allAccount =
            new ConcurrentDictionary<string, Tuple<Models.Entity.Account, long>>();

        public static readonly int UserLogMax = 1000;

        /// <summary>
        ///     10 phút chạy lại timer
        /// </summary>
        public static readonly int TimerUserLog = 10;

        [Import] private DelayActionManager _delayAction;

        [Import] private ILog _log;

        [Import] private Loader _loader;

        private readonly List<UserLog> _userLogCache = new List<UserLog>();

        private readonly ConcurrentDictionary<long, Branch> _allBranchByCompany = new ConcurrentDictionary<long, Branch>();
        private readonly ConcurrentDictionary<string, BranchData> _allBranch = new ConcurrentDictionary<string, BranchData>();

        private readonly ConcurrentDictionary<string, SystemPair> _allSystemPair = new ConcurrentDictionary<string, SystemPair>();

        private Timer _timer;

        /// <summary>
        ///     thêm user log
        /// </summary>
        /// <param name="userLog"></param>
        public void AddUserLogCache(UserLog userLog)
        {
            _userLogCache.Add(userLog);
        }

        /// <summary>
        ///     lấy user log
        /// </summary>
        /// <returns></returns>
        public List<UserLog> GetUserLogs()
        {
            return _userLogCache;
        }

        public void CreateStaticToken()
        {
            _allAccount.TryAdd(Guid.Empty.ToString(),
                new Tuple<Models.Entity.Account, long>(new Models.Entity.Account()
                {
                    CompanyIds = new List<Company>(),
                    CreateDate = DateTime.Now,
                    DeviceIds = new List<Device>(),
                    GroupUserId = "Root",
                    IsBlock = false,
                    Level = AccountLevel.Root,
                    Pwd = StarSg.Utils.Utils.Crypto.GetSH1("Qwerty@123456"),
                    Role = new Role() {Name = "Root", Functions = new List<RoleFun>()},
                    Username = "root",
                    DisplayName = "Root"
                }, 0));
        }

        private readonly object _lockCreateGuid = new object();

        #region Implementation of IAccountManager

        /// <summary>
        ///     kiểm tra token của tài khoản
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidToken(string token)
        {
            Tuple<Models.Entity.Account, long> tmp;
            if (_allAccount.TryGetValue(token, out tmp))
            {
                _delayAction.ResetTimeOut(tmp.Item2);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     lấy tất cả các user online
        /// </summary>
        /// <returns></returns>
        public List<AccountTranfer> GetUserOnline()
        {
            return _allAccount.Values.Select(m => m.Item1).Select(k => new AccountTranfer
            {
                Username = k.Username,
                //Pwd = k.Pwd,
                CompanyIds = k.CompanyIds.Select(m => m.CompanyId).ToList(),
                RoleId = k.GroupUserId,
                Level = (int) k.Level,
                DeviceIds = k.DeviceIds.Select(m => m.Serial).ToList(),
                IsBlock = k.IsBlock,
                DisplayName = k.DisplayName,
                Phone = k.Phone
            }).ToList();
        }

        /// <summary>
        ///     lấy thông tin user theo token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Models.Entity.Account GetUser(string token)
        {
            Tuple<Models.Entity.Account, long> tmp;
            if (_allAccount.TryGetValue(token, out tmp))
            {
                _delayAction.ResetTimeOut(tmp.Item2);
                return tmp.Item1;
            }
            return null;
        }

        /// <summary>
        ///     tạo mới 1 token theo tên tài khoản đăng nhập
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="pwd">mật khẩu</param>
        /// <returns></returns>
        public string Login(string username, string pwd)
        {
            try
            {
                pwd = StarSg.Utils.Utils.Crypto.GetSH1(pwd);
                var context = _loader.GetContext();
                var account = context.GetWhere<Models.Entity.Account>(m => m.Username == username && m.Pwd == pwd).FirstOrDefault();
                if (account == null)
                {
                    // không tồn tại tài khoản hoặc sai pass
                    _log.Warning("AccountManager", $"login tài khoản null");
                }
                else
                {
                    account.Role = context.Get<Role>(account.GroupUserId);
                    context.Dispose();
                    if (account.Role == null) return string.Empty; // nếu tài khoản chưa có groupUser thì ko cho login
                    var token = "";
                    lock (_lockCreateGuid)
                    {
                        token = Guid.NewGuid().ToString();
                        Thread.Sleep(1);
                    }
                    if (_allAccount.ContainsKey(token))
                    {
                        // tạo token trùng
                        return string.Empty;
                    }
                    _allAccount.TryAdd(token, new Tuple<Models.Entity.Account, long>(account, _delayAction.AddAction(() =>
                    {
                        // sau khi hết thời gian 10 phút mà ko có thao tác lấy dữ liệu nào từ user thi gỡ ra
                        Logout(token);
                    }, new TimeSpan(0, 120, 0))));
                    return token;
                }
                _log.Warning("AccountManager", $"login tài khoản thất bại");
                context.Dispose();
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "đăng nhập lỗi");
            }
            return "";
        }

        /// <summary>
        /// Giới hạn tối đa của id công ty
        /// </summary>
        public const long COMPANYLIMIT = 100000000L;

        /// <summary>
        /// Lấy danh sách user quản lý nhóm
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public IList<String> GetUsersOfGroup(long companyid,long groupid)
        {
            try
            {
                groupid = companyid * COMPANYLIMIT + groupid;
                using (var context = _loader.GetContext())
                {
                    ////Cần nâng cấp chỗ này
                    //List<String> ret = new List<string>(1);
                    //var accountList = context.GetAll<string, Account>(m => m.Username);
                    //foreach (var Username in accountList.Keys)
                    //{
                    //    if (accountList[Username].CompanyIds.Any(m => m.CompanyId == groupid))
                    //        ret.Add(Username);
                    //}
                    //return ret;
                    List<String> ret = new List<string>();
                    context.CustomHandle<NHibernate.ISession>(m => {
                        try
                        {
                            NHibernate.ISQLQuery query = m.CreateSQLQuery($"SELECT Account FROM Company where CompanyId={groupid}");
                            var rets = query.DynamicList();
                            if (rets != null)
                            {
                                for (int i = 0; i < rets.Count; i++)
                                    ret.Add(rets[i].Account);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("AccountManager", ex, "SELECT Account FROM Company where CompanyId");
                        }

                    });
                    return ret;
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "Lấy danh sách user quản lý nhóm");
            }
            return new List<String>(0);
        }

        /// <summary>
        /// Lấy user trực tiếp sử dụng thiết bị
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public IList<String> GetUsersOfSerial(long serial)
        {
            try
            {
                using (var context = _loader.GetContext())
                {
                    //return context.GetWhere<Account>(m => m.DeviceIds.FirstOrDefault(d => d.Serial == serial)!=null).Select(r=>r.Username).ToList();
                    List<String> ret = new List<string>();
                    context.CustomHandle<NHibernate.ISession>(m => {
                        try
                        {
                            NHibernate.ISQLQuery query = m.CreateSQLQuery($"SELECT Account FROM Device where Serial={serial}");
                            var rets = query.DynamicList();
                            if(rets!=null)
                            {
                                for (int i = 0; i < rets.Count; i++)
                                    ret.Add(rets[i].Account);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("AccountManager", ex, "SELECT Account FROM Device where Serial");
                        }

                    });
                    return ret;
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "Lấy user trực tiếp sử dụng thiết bị");
            }
            return new List<String>(0);
        }

        /// <summary>
        ///     bắt buộc tài khoản đăng xuất
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ForceLogout(string username)
        {
            var tmp = _allAccount.ToArray().Where(m => m.Value.Item1.Username == username);
            foreach (var keyValuePair in tmp)
            {
                ForceLogoutWithToken(keyValuePair.Key);
            }
            return true;
        }

        /// <summary>
        ///     bắt buộc tài khoản có token này đăng xuất
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ForceLogoutWithToken(string token)
        {
            Tuple<Models.Entity.Account, long> tmp;
            if (_allAccount.TryRemove(token, out tmp))
            {
                _delayAction.RemoveAction(tmp.Item2);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     user logout
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Logout(string token)
        {
            return ForceLogoutWithToken(token);
        }

        /// <summary>
        ///     thay đổi mật khẩu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool ChangePassword(string username, string pwd)
        {
            var context = _loader.GetContext();
            var account = context.GetWhere<Models.Entity.Account>(m => m.Username == username).FirstOrDefault();
            if (account == null)
            {
                return false;
            }
            account.Pwd = StarSg.Utils.Utils.Crypto.GetSH1(pwd);
            context.Update(account);
            context.Commit();
            context.Dispose();
            return true;
        }

        /// <summary>
        ///     cập nhật thông tin user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Update(string username, AccountInfo info)
        {
            if (info == null)
            {
                return false;
            }

            var context = _loader.GetContext();
            var account = context.GetWhere<Models.Entity.Account>(m => m.Username == username).FirstOrDefault();
            if (account == null)
            {
                return false;
            }

            account.DisplayName = info.DisplayName;
            account.Phone = info.Phone;

            context.Update(account);
            context.Commit();
            context.Dispose();
            return true;
        }


        /// <summary>
        ///     insert data userlog vào database và xóa cache
        /// </summary>
        private void RunTimerUserLog()
        {
            if (_userLogCache == null || _userLogCache.Count <= 0) return;
            var context = _loader.GetContext();
            context.InsertAll(_userLogCache);
            _userLogCache.Clear();
        }

        /// <summary>
        /// Cập nhật branchCode cho companyId
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        public bool EnsureBranch(long companyId, String branchCode)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    var oldBranch = context.Get<Branch>(companyId);

                    //update
                    if(oldBranch!=null)
                    {
                        oldBranch.BranchCode = branchCode;
                        context.Update<Branch>(oldBranch);
                    }
                    //add
                    else
                    {
                        context.Insert<Branch>(new Branch()
                        {
                            BranchCode = branchCode,
                            CompanyId = companyId
                        });
                    }

                    context.Commit();
                }

                //cập nhật trên memory
                Branch ret;
                if (_allBranchByCompany.TryGetValue(companyId, out ret))
                    _allBranchByCompany.TryUpdate(companyId, new Branch() { CompanyId = companyId, BranchCode = branchCode }, ret);
                else
                    _allBranchByCompany.TryAdd(companyId, new Branch() { CompanyId = companyId, BranchCode = branchCode });

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "EnsureBranch");
            }
            return false;
        }

        /// <summary>
        /// Lấy branchCode theo companyId
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Branch GetBranchOfCompany(long companyId)
        {
            //Nếu companyId là companyId + groupID thì đổi lại
            if (companyId > AccountTranfer.COMPANYLIMIT)
                companyId = companyId / AccountTranfer.COMPANYLIMIT;

            Branch ret;
            if (_allBranchByCompany.TryGetValue(companyId, out ret))
                return ret;

            //try
            //{
            //    using (var context = _loader.GetContext())
            //    {
            //        return context.Get<Branch>(companyId);
            //    }
            //}
            //catch (Exception e)
            //{
            //    _log.Exception("AccountManager", e, "GetBranch");
            //}

            return null;
        }

        /// <summary>
        /// Load all branche vô memory
        /// </summary>
        public void LoadBranch()
        {
            try
            {
                using (var context = _loader.GetContext())
                {
                    var rets = context.GetAll<long, Branch>(m => m.CompanyId);
                    foreach (var item in rets)
                    {
                        _allBranchByCompany.TryAdd(item.Key, item.Value);
                    }

                    var retdatas = context.GetAll<string, BranchData>(m => m.Id);
                    foreach (var item in retdatas)
                    {
                        _allBranch.TryAdd(item.Key, item.Value);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "LoadBranch");
            }
        }

        public BranchData GetBranch(string id)
        {
            BranchData ret;
            if (_allBranch.TryGetValue(id, out ret))
                return ret;
            return null;
        }

        public List<BranchData> AllBranch()
        {
            var result = new List<BranchData>();
            var tmp = _allBranch.GetEnumerator();
            while (tmp.MoveNext())
            {
                result.Add(tmp.Current.Value);
            }
            return result;
        }

        public bool AddBranch(BranchData branch)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    context.Insert<BranchData>(branch);
                    context.Commit();
                }

                //cập nhật trên memory
                _allBranch.TryAdd(branch.Id, branch);

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "AddBranch");
            }
            return false;
        }

        public bool UpdateBranch(BranchData branch)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    context.Update<BranchData>(branch);
                    context.Commit();
                }

                //cập nhật trên memory
                BranchData ret;
                if (_allBranch.TryGetValue(branch.Id, out ret))
                    _allBranch.TryUpdate(branch.Id, branch, ret);
                //else
                //   _allBranch.TryAdd(branch.Id, branch);

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "UpdateBranch");
            }
            return false;
        }

        public bool DeleteBranch(String id)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    context.DeleteWhere<BranchData>(m => m.Id == id);
                    context.Commit();
                }

                //cập nhật trên memory
                BranchData ret;
                _allBranch.TryRemove(id,out ret);

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "UpdateBranch");
            }
            return false;
        }






        public void LoadSystemPair()
        {
            try
            {
                using (var context = _loader.GetContext())
                {
                    var retdatas = context.GetAll<string, SystemPair>(m => m.Id);
                    foreach (var item in retdatas)
                    {
                        _allSystemPair.TryAdd(item.Key, item.Value);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "LoadSystemPair");
            }
        }

        public SystemPair GetSystemPair(string id)
        {
            SystemPair ret;
            if (_allSystemPair.TryGetValue(id, out ret))
                return ret;
            return null;
        }

        public List<SystemPair> AllSystemPair()
        {
            var result = new List<SystemPair>();
            var tmp = _allSystemPair.GetEnumerator();
            while (tmp.MoveNext())
            {
                result.Add(tmp.Current.Value);
            }
            return result;
        }

        public bool AddSystemPair(SystemPair obj)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    context.Insert<SystemPair>(obj);
                    context.Commit();
                }

                //cập nhật trên memory
                _allSystemPair.TryAdd(obj.Id, obj);

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "AddSystemPair");
            }
            return false;
        }

        public bool UpdateSystemPair(SystemPair obj)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    context.Update<SystemPair>(obj);
                    context.Commit();
                }

                //cập nhật trên memory
                SystemPair ret;
                if (_allSystemPair.TryGetValue(obj.Id, out ret))
                    _allSystemPair.TryUpdate(obj.Id, obj, ret);
                //else
                //   _allSystemPair.TryAdd(branch.Id, branch);

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "UpdateSystemPair");
            }
            return false;
        }

        public bool DeleteSystemPair(String id)
        {
            try
            {
                //Cập nhật db
                using (var context = _loader.GetContext())
                {
                    context.DeleteWhere<SystemPair>(m => m.Id == id);
                    context.Commit();
                }

                //cập nhật trên memory
                SystemPair ret;
                _allSystemPair.TryRemove(id, out ret);

                return true;
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "DeleteSystemPair");
            }
            return false;
        }


        #endregion

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            if (_timer == null)
            {
                _timer = new Timer(_ => RunTimerUserLog(), null, new TimeSpan(), TimeSpan.FromMinutes(TimerUserLog));
            }
            Instance = this;
        }

        /// <summary>
        /// Lấy thông tin user từ username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AccountInfo GetUserInformation(string username)
        {
            try
            {

                using (var context = _loader.GetContext())
                {
                    var account = context.GetWhere<Models.Entity.Account>(m => m.Username == username).FirstOrDefault();
                    if (account != null)
                    {
                        return new AccountInfo()
                        {
                             Phone = account.Phone,
                             DisplayName = account.DisplayName
                        };
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "Lấy Models.Entity.Account từ username");
            }
            return null;
        }

        /// <summary>
        /// Keep to use in Hub
        /// </summary>
        public static AccountManager Instance;

        #endregion
    }

}