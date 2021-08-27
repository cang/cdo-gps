using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Core.Models.Auth;
using Log;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Models.Req;
using DaoDatabase;
using NHibernate.Criterion;

namespace Route.Api.Auth.Core
{
    /// <summary>
    ///     quản trị tài khoản
    /// </summary>
    [Export(typeof (IAdministrationManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AdministrationManager : IAdministrationManager, IPartImportsSatisfiedNotification
    {
        [Import] private IAccountManager _accountManager;
        [Import] private Loader _loader;
        [Import] private ILog _log;
        /// <summary>
        ///     khóa tài khoản theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool LockUser(string username)
        {
            var context = _loader.GetContext();
            var account = context.GetWhere<Account>(m => m.Username == username).FirstOrDefault();
            if (account == null)
            {
                // không tồn tại tài khoản hoặc sai pass
                return false;
            }
            //force logout username
            _accountManager.ForceLogout(username);
            //lock tài khoản
            account.IsBlock = true;
            context.Update(account);
            context.Commit();
            context.Dispose();
            return true;
        }

        /// <summary>
        ///     thêm log và database
        /// </summary>
        /// <param name="userLogTranfer"></param>
        /// <returns></returns>
        public bool AddUserLog(UserLogTranfer userLogTranfer)
        {
            if (userLogTranfer == null)
            {
                //giá trị null
                return false;
            }
            var oldUser = _accountManager.GetUser(userLogTranfer.Token);
            _accountManager.AddUserLogCache(new UserLog
            {
                CreateDate = DateTime.Now,
                Description = userLogTranfer.Description,
                TypeLog = userLogTranfer.TypeLog,
                Username = oldUser.Username,
                TimeHandle = userLogTranfer.TimeHandle
            });
            return true;
        }

        /// <summary>
        /// lấy user log
        /// </summary>
        /// <param name="username"></param>
        /// <param name="begineDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public List<UserLogTranfer> GetUserLog(string username, DateTime begineDateTime, DateTime endDateTime)
        {
            var context = _loader.GetContext();
            var result =
                context.GetWhere<UserLog>(
                    m => m.Username == username && m.CreateDate >= begineDateTime && m.CreateDate <= endDateTime)
                    .Select(k => new UserLogTranfer
                    {
                        Id = k.Id,
                        Description = k.Description,
                        Token = k.Username,
                        CreateDate = k.CreateDate,
                        TypeLog = k.TypeLog,
                        TimeHandle = k.TimeHandle
                    }).ToList();
            context.Dispose();
            return result;
        }

        private List<AccountTranfer> GetAccountTranferFromDynamic(IList<dynamic> list)
        {
            Dictionary<string, AccountTranfer> dic = new Dictionary<string, AccountTranfer>();
            if (list != null)
            {
                foreach (var obj in list)
                {
                    AccountTranfer acc = null;
                    if (!dic.TryGetValue(obj.Username, out acc))
                    {
                        acc = new AccountTranfer();
                        acc.CompanyIds = new List<long>();
                        acc.DeviceIds = new List<long>(0);
                        acc.Username = obj.Username;
                        acc.RoleId = obj.GroupUserId;
                        acc.IsBlock = obj.IsBlock;
                        acc.DisplayName = obj.DisplayName;
                        acc.Phone = obj.Phone;

                        AccountLevel tmplevel = AccountLevel.ReadonlyGuest;
                        if (Enum.TryParse<AccountLevel>(obj.Level, out tmplevel))
                            acc.Level = (int)tmplevel;

                        dic.Add(obj.Username, acc);
                    }

                    object comobj = obj.CompanyId;
                    if (!Object.ReferenceEquals(null, comobj))
                    {
                        long ret = -1;
                        if (comobj.GetType() == typeof(long)) ret = (long)comobj;
                        else long.TryParse(comobj.ToString(), out ret);
                        if (ret >= 0 && !acc.CompanyIds.Contains(ret)) acc.CompanyIds.Add(ret);
                    }


                    object devobj = obj.Serial;
                    if (!Object.ReferenceEquals(null, devobj))
                    {
                        long ret = -1;
                        if (devobj.GetType() == typeof(long)) ret = (long)devobj;
                        else long.TryParse(devobj.ToString(), out ret);
                        if (ret >= 0 && !acc.DeviceIds.Contains(ret)) acc.DeviceIds.Add(ret);
                    }
                }
            }
            return dic.Values.ToList();
        }

        ///// <summary>
        /////     lấy các user thuộc công ty
        ///// </summary>
        ///// <param name="companyId"></param>
        ///// <returns></returns>
        //public List<AccountTranfer> GetUserByCompany(long companyId)
        //{
        //    var accountList = GetAllUser();
        //    //lọc lại kết quả các account thuộc company truyền vào
        //    var result = new List<AccountTranfer>();
        //    foreach (var data in accountList)
        //    {
        //        //if (data.CompanyIds != null && data.CompanyIds.Contains(companyId))
        //        if (data.ContainsCompanyIds(companyId))
        //        {
        //            result.Add(data);
        //        }
        //    }
        //    return result;
        //}
   

        /// <summary>
        ///     lấy các user thuộc công ty
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<AccountTranfer> GetUserByCompany(long companyId)
        {
            if (companyId > AccountTranfer.COMPANYLIMIT) companyId = companyId / AccountTranfer.COMPANYLIMIT;

            List<AccountTranfer> ret = new List<AccountTranfer>(0);
            try
            {
                using (var context = _loader.GetContext())
                {
                    context.CustomHandle<NHibernate.ISession>(m => {
                        try
                        {
                            NHibernate.ISQLQuery query = m.CreateSQLQuery($@"SELECT acc.* , com.CompanyId, dev.Serial 
                                                                                FROM Account  as acc
                                                                                left outer join Company as com on acc.Username = com.Account
                                                                                left outer join Device as dev on acc.Username = dev.Account
                                                                                WHERE com.CompanyId= {companyId} OR com.CompanyId/{AccountTranfer.COMPANYLIMIT} = {companyId}
                                                                                ");

                            //NHibernate.ISQLQuery query = m.CreateSQLQuery($@"SELECT acc.* , com.CompanyId
                            //                                                    FROM Account as acc
                            //                                                    left outer join Company as com on acc.Username = com.Account
                            //                                                    WHERE com.CompanyId= {companyId} OR com.CompanyId/{AccountTranfer.COMPANYLIMIT} = {companyId}
                            //                                                    ");

                            ret = GetAccountTranferFromDynamic(query.DynamicList());
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("AccountManager", ex, "SELECT acc.* , com.CompanyId, dev.Serial");
                        }

                    });
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "GetUserByCompany");
            }
            return ret;
        }


        ///// <summary>
        /////     lấy tất cả user
        ///// </summary>
        ///// <returns></returns>
        //public List<AccountTranfer> GetAllUser()
        //{
        //    var context = _loader.GetContext();
        //    //lấy tất cả account
        //    var accountList =
        //        context.GetAll<string, Account>(m => m.Username).Select(c => c.Value).Select(k => new AccountTranfer
        //        {
        //            Username = k.Username,
        //            //Pwd = k.Pwd,
        //            CompanyIds = k.CompanyIds.Select(m => m.CompanyId).ToList(),
        //            RoleId = k.GroupUserId,
        //            Level = (int) k.Level,
        //            DeviceIds = k.DeviceIds.Select(m => m.Serial).ToList(),
        //            IsBlock = k.IsBlock
        //        }).ToList();
        //    context.Dispose();
        //    return accountList;
        //}

      
        /// <summary>
        ///     lấy tất cả user
        /// </summary>
        /// <returns></returns>
        public List<AccountTranfer> GetAllUser()
        {
            List<AccountTranfer> ret = new List<AccountTranfer>(0);
            try
            {
                using (var context = _loader.GetContext())
                {
                    context.CustomHandle<NHibernate.ISession>(m => {
                        try
                        {
                            NHibernate.ISQLQuery query = m.CreateSQLQuery(@"SELECT acc.* , com.CompanyId, dev.Serial 
                                                                                FROM Account as acc
                                                                                left outer join Company as com on acc.Username = com.Account
                                                                                left outer join Device as dev on acc.Username = dev.Account
                                                                                ");
                            //NHibernate.ISQLQuery query = m.CreateSQLQuery(@"SELECT acc.* , com.CompanyId
                            //                                                    FROM Account as acc
                            //                                                    left outer join Company as com on acc.Username = com.Account
                            //                                                    ");
                            ret = GetAccountTranferFromDynamic(query.DynamicList());
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("AccountManager", ex, "SELECT acc.* , com.CompanyId, dev.Serial");
                        }

                    });
                }
            }
            catch (Exception e)
            {
                _log.Exception("AccountManager", e, "GetAllUser");
            }
            return ret;
        }


        /// <summary>
        /// lấy danh sách user theo cấp bâch
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public List<AccountTranfer> GetAllUserByLevel(AccountLevel level)
        {
            var context = _loader.GetContext();
            //lấy tất cả account
            var accountList =
                context.GetWhere<Account>(m => m.Level==level).Select(k => new AccountTranfer
                {
                    Username = k.Username,
                    //Pwd = k.Pwd,
                    CompanyIds = k.CompanyIds.Select(m => m.CompanyId).ToList(),
                    RoleId = k.GroupUserId,
                    Level = (int)k.Level,
                    DeviceIds = k.DeviceIds.Select(m => m.Serial).ToList(),
                    IsBlock = k.IsBlock,
                    DisplayName = k.DisplayName,
                    Phone = k.Phone
                }).ToList();
            context.Dispose();
            return accountList;
        }
        /// <summary>
        /// Bõ qua hàm này do sử dụng GroupUserId
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<AccountTranfer> GetAllUser(string parent)
        {
            return new List<AccountTranfer>(0);

            //var context = _loader.GetContext();
            //var tmpGroup=new Dictionary<string,Role>();
            ////lấy tất cả account
            //var accountList =
            //    context.GetAll<string, Account>(m => m.Username).Where(m =>
            //    {
            //        if (m.Value.GroupUserId == null) return false;
            //        Role gr;
            //        if (tmpGroup.TryGetValue(m.Value.GroupUserId, out gr))
            //        {

            //        }
            //        else
            //        {
            //            gr = context.Get<Role>(m.Value.GroupUserId);
            //            if (gr == null) return false;
            //            tmpGroup.Add(gr.Name, gr);
            //        }
            //        return gr.Parent == parent;
            //    }).Select(c => c.Value).Select(k => new AccountTranfer
            //    {
            //        Username = k.Username,
            //        //Pwd = k.Pwd,
            //        CompanyIds = k.CompanyIds.Select(m => m.CompanyId).ToList(),
            //        RoleId = k.GroupUserId,
            //        Level = (int)k.Level,
            //        DeviceIds = k.DeviceIds.Select(m => m.Serial).ToList(),
            //        IsBlock = k.IsBlock
            //    }).ToList();
            //context.Dispose();
            //return accountList;
        }


        /// <summary>
        ///     tạo mới user
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool CreateUser(AccountTranfer ac)
        {
            try
            {
//check null account 
                if (ac == null)
                {
                    return false;
                }
                
                var context = _loader.GetContext();
                //kiểm tra user đã tồn tại chưa
                if (context.Get<Account>(ac.Username) != null)
                {
                    return false;
                }

                ////không xài role
                //if (string.IsNullOrEmpty(ac.RoleId)||context.Get<Role>(ac.RoleId) == null) return false;

                //thêm account vào database
                var account = new Account
                {
                    Username = ac.Username,
                    CreateDate = DateTime.Now,
                    Pwd = StarSg.Utils.Utils.Crypto.GetSH1(ac.Pwd),
                    GroupUserId = ac?.RoleId,
                    IsBlock = false,
                    Level = (AccountLevel) ac.Level,
                    DisplayName = ac.DisplayName,
                    Phone = ac.Phone
                };
                account.CompanyIds =
                    ac.CompanyIds?.Select(m => new Company {CompanyId = m, Id = 0, Account = account}).ToList()??new List<Company>();
                //foreach (var companyId in account.CompanyIds)
                //{
                //    context.Insert(companyId);
                //}
                //context.Commit();
                account.DeviceIds =
                    ac.DeviceIds?.Select(m => new Device {Serial = m, Id = 0, Account = account}).ToList()??new List<Device>();
                
                //foreach (var deviceId in account.DeviceIds)
                //{
                //    context.Insert(deviceId);
                //}
               // context.Commit();
                context.Insert(account);
                context.Commit();
                context.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _log.Exception("CreateUser", e, "tạo tài khoản lỗi");
                return false;
            }
        }

        /// <summary>
        ///     lấy user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AccountTranfer GetUser(string username)
        {
            try
            {
                //check null account 
                if (String.IsNullOrWhiteSpace(username))
                {
                    return null;
                }
                using (var context = _loader.GetContext())
                {
                    //lấy tất cả account
                    var acc = context.Get<Account>(username);
                    var ret = new AccountTranfer
                    {
                        Username = acc.Username,
                        //Pwd = k.Pwd,
                        CompanyIds = acc.CompanyIds.Select(m => m.CompanyId).ToList(),
                        RoleId = acc.GroupUserId,
                        DeviceIds = acc.DeviceIds.Select(m => m.Serial).ToList(),
                        Level = (int)acc.Level,
                        IsBlock = acc.IsBlock,
                        DisplayName = acc.DisplayName,
                        Phone = acc.Phone
                    };
                    return ret;
                }
            }
            catch (Exception e)
            {
                _log.Exception("GetUser", e, "lỗi lấy tài khoản");
                return null;
            }
        }

        /// <summary>
        ///     cập nhật lại user
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool UpdateUser(AccountTranfer account)
        {
            try
            {
                //check null account 
                if (account == null)
                {
                    return false;
                }
                var context = _loader.GetContext();
                var oldAccount = context.Get<Account>(account.Username);
                if (oldAccount == null)
                {
                    //accout này không tồn tại trong database
                    return false;
                }
                if(account.CompanyIds==null) account.CompanyIds=new List<long>();
                if(account.DeviceIds==null) account.DeviceIds=new List<long>();
                //check có sự thay đổi danh sách công ty
                var removeCompanyId = oldAccount.CompanyIds.Where(m => !account.CompanyIds.Contains(m.CompanyId)).ToList();
                var addCompanyId =
                    account.CompanyIds.Where(m => oldAccount.CompanyIds.FirstOrDefault(z => z.CompanyId == m) == null).ToList();
                foreach (var companyId in removeCompanyId)
                {
                    context.Delete(companyId);
                    oldAccount.CompanyIds.Remove(companyId);
                }
                foreach (var company in addCompanyId)
                {
                    var tmp = new Company {Account = oldAccount, CompanyId = company};
                    context.Insert(tmp);
                    oldAccount.CompanyIds.Add(tmp);
                }

                //if (!CompareListCompany(oldAccount.CompanyIds.ToList(), account.CompanyIds))
                //{
                //    oldAccount.CompanyIds =
                //        account.CompanyIds.Select(m => new Company { Account = oldAccount, CompanyId = m }).ToList();
                //}
                //check có sự thay đổi danh sách device
                //if (!CompareListDevice(oldAccount.DeviceIds.ToList(), account.DeviceIds))
                //{
                //    oldAccount.DeviceIds =
                //        account.DeviceIds.Select(m => new Device { Account = oldAccount, Serial = m ,Id = 0}).ToList();
                //}

                var removeDevice= oldAccount.DeviceIds.Where(m => !account.DeviceIds.Contains(m.Serial)).ToList();
                var addDevice = account.DeviceIds.Where(m => oldAccount.DeviceIds.FirstOrDefault(z => z.Serial == m) == null).ToList();
                foreach (var device in removeDevice)
                {
                    context.Delete(device);
                    oldAccount.DeviceIds.Remove(device);
                }
                foreach (var device in addDevice)
                {
                    var tmp = new Device {Account = oldAccount, Serial = device};
                    context.Insert(tmp);
                    oldAccount.DeviceIds.Add(tmp);
                }
                context.Commit();
                oldAccount.GroupUserId = account?.RoleId;
                oldAccount.IsBlock = account.IsBlock;
                oldAccount.Level = (AccountLevel) account.Level;
                oldAccount.DisplayName = account.DisplayName;
                oldAccount.Phone = account.Phone;

                //empty mean not be updated - should be empty from update user
                if (!String.IsNullOrWhiteSpace(account.Pwd))
                    oldAccount.Pwd = StarSg.Utils.Utils.Crypto.GetSH1(account.Pwd);

                context.Update(oldAccount);
                context.Commit();
                context.Dispose();
                return true;
            }
            catch (Exception e)
            {
                _log.Exception("UpdateUser",e,"lỗi cập nhật tài khoản");
                return false;
            }
        }



        ///// <summary>
        ///// kiểm tra danh sách công ty trong database có khác với request
        ///// </summary>
        ///// <param name="database"></param>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //private bool CompareListCompany(List<Company> database, List<long> request)
        //{
        //    if (database.Count != request.Count)
        //    {
        //        return false;
        //    }
        //    return database.All(company => request.Contains(company.CompanyId));
        //}

        ///// <summary>
        /////  kiểm tra danh sách device trong database có khác với request
        ///// </summary>
        ///// <param name="database"></param>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //private bool CompareListDevice(List<Device> database, List<long> request)
        //{
        //    if (database.Count != request.Count)
        //    {
        //        return false;
        //    }
        //    return database.All(device => request.Contains(device.Serial));
        //}

        /// <summary>
        ///     xóa user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool DeleteUser(string username)
        {
            var context = _loader.GetContext();
            var oldAccount = context.GetWhere<Account>(m => m.Username == username).FirstOrDefault();
            if (oldAccount == null)
            {
                return false;
            }
            context.Delete(oldAccount);
            context.Commit();
            context.Dispose();
            return true;
        }

        public void OnImportsSatisfied()
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="method">tùy chọn</param>
        /// <returns></returns>
        public List<AccessHistoryTranfer> GetAccessHistory(string username, DateTime begin, DateTime end, string method = "")
        {
            try
            {
                if(String.IsNullOrWhiteSpace(method))
                    using (var db = _loader.GetContext())
                    {
                        return db.GetWhere<AccessHistory>(m =>
                       m.Username == username
                       && m.AtTime >= begin
                       && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                       {
                           Id = m.Id,
                           AtTime = m.AtTime,
                           CompanyId = m.CompanyId,
                           Content = m.Content,
                           GroupId = m.GroupId,
                           Method = m.Method,
                           Note = m.Note,
                           Serial = m.Serial,
                           Username = m.Username
                       }).ToList();
                    }
                else
                    using (var db = _loader.GetContext())
                    {
                        return db.GetWhere<AccessHistory>(m =>
                       m.Username == username
                       && m.Method == method
                       && m.AtTime >= begin
                       && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                       {
                           Id = m.Id,
                           AtTime = m.AtTime,
                           CompanyId = m.CompanyId,
                           Content = m.Content,
                           GroupId = m.GroupId,
                           Method = m.Method,
                           Note = m.Note,
                           Serial = m.Serial,
                           Username = m.Username
                       }).ToList();
                    }
            }
            catch (Exception e)
            {
                _log.Exception("GetAccessHistory", e, "Lỗi lấy lịch sử truy xuất");
            }
            return new List<AccessHistoryTranfer>(0);
        }

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
        public List<AccessHistoryTranfer> GetAccessHistory(long companyId, long groupId, DateTime begin, DateTime end, string method,long serial)
        {
            try
            {
                if(serial>0)
                {
                    if (String.IsNullOrWhiteSpace(method))
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<AccessHistory>(m =>
                           m.Serial == serial
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               Content = m.Content,
                               GroupId = m.GroupId,
                               Method = m.Method,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<AccessHistory>(m =>
                           m.Serial == serial
                           && m.Method == method
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               Content = m.Content,
                               GroupId = m.GroupId,
                               Method = m.Method,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
                else if(groupId <= 0)
                {
                    if (String.IsNullOrWhiteSpace(method))
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<AccessHistory>(m =>
                           m.CompanyId == companyId
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               Content = m.Content,
                               GroupId = m.GroupId,
                               Method = m.Method,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<AccessHistory>(m =>
                           m.CompanyId == companyId
                           && m.Method == method
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               Content = m.Content,
                               GroupId = m.GroupId,
                               Method = m.Method,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(method))
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<AccessHistory>(m =>
                           m.CompanyId == companyId
                           && m.GroupId == groupId
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               Content = m.Content,
                               GroupId = m.GroupId,
                               Method = m.Method,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<AccessHistory>(m =>
                           m.CompanyId == companyId
                           && m.GroupId == groupId
                           && m.Method == method
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new AccessHistoryTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               Content = m.Content,
                               GroupId = m.GroupId,
                               Method = m.Method,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
            }
            catch (Exception e)
            {
                _log.Exception("GetAccessHistory", e, "Lỗi lấy lịch sử truy xuất");
            }
            return new List<AccessHistoryTranfer>(0);
        }

        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo serial
        /// </summary>
        /// <param name="serial">serial</param>
        /// <returns></returns>
        public List<AccessHistoryTranfer> GetAccessHistoryBySerial(long serial)
        {
            try
            {
                using (var db = _loader.GetContext())
                {
                    return db.GetWhere<AccessHistory>(m =>
                    m.Serial == serial).Select(m => new AccessHistoryTranfer()
                    {
                        Id = m.Id,
                        AtTime = m.AtTime,
                        CompanyId = m.CompanyId,
                        Content = m.Content,
                        GroupId = m.GroupId,
                        Method = m.Method,
                        Note = m.Note,
                        Serial = m.Serial,
                        Username = m.Username
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                _log.Exception("GetAccessHistoryBySerial", e, "Lỗi lấy lịch sử truy xuất");
            }
            return new List<AccessHistoryTranfer>(0);
        }


        /// <summary>
        /// lấy danh sách lịch sử truy xuất theo giá trị bên trong nội dung
        /// </summary>
        /// <param name="content">Giá trị nội dung</param>
        public List<AccessHistoryTranfer> GetAccessHistoryByContent(String content)
        {
            if(String.IsNullOrWhiteSpace(content)) return new List<AccessHistoryTranfer>(0);
            try
            {
                List<AccessHistoryTranfer> ret = new List<AccessHistoryTranfer>();
                using (var db = _loader.GetContext())
                {
                    db.CustomHandle<NHibernate.ISession>(m => {
                        try
                        {
                            NHibernate.ISQLQuery query = m.CreateSQLQuery($"SELECT * FROM AccessHistory WHERE Content like '%{content}%'");
                            IList<dynamic> list = query.DynamicList();
                            if (list != null)
                            {
                                foreach (var obj in list)
                                {
                                    AccessHistoryTranfer his = new AccessHistoryTranfer();
                                    his.Id = obj.Id;
                                    his.AtTime = obj.AtTime;
                                    his.CompanyId = obj.CompanyId;
                                    his.GroupId = obj.GroupId;
                                    his.Method = obj.Method;

                                    if (!Object.ReferenceEquals(null, obj.Note))
                                        his.Note = obj.Note;

                                    if (!Object.ReferenceEquals(null, obj.Content))
                                        his.Content = obj.Content;

                                    his.Serial = obj.Serial;
                                    his.Username = obj.Username;

                                    ret.Add(his);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("GetAccessHistoryByContent", ex, $"SELECT * FROM AccessHistory WHERE Content like '%{content}%'");
                        }

                    });
                    return ret;
                }
            }
            catch (Exception e)
            {
                _log.Exception("GetAccessHistoryByContent", e, "Lỗi lấy lịch sử truy xuất");
            }
            return new List<AccessHistoryTranfer>(0);
        }


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
        public List<SetupDeviceTranfer> GetSetupDeviceHistory(DateTime begin, DateTime end, short opcode=0, long companyId=0, long groupId=0, long serial=0)
        {
            try
            {
                if(serial==0 && groupId==0 && companyId==0)
                {
                    if (opcode < 1)
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.opcode == opcode
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
                else
                if (serial > 0)
                {
                    if (opcode<1)
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.Serial == serial
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.Serial == serial
                           && m.opcode == opcode
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
                else if (groupId <= 0)
                {
                    if (opcode<1)
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.CompanyId == companyId
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.CompanyId == companyId
                           && m.opcode == opcode
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
                else
                {
                    if (opcode < 1)
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.CompanyId == companyId
                           && m.GroupId == groupId
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                    else
                        using (var db = _loader.GetContext())
                        {
                            return db.GetWhere<SetupDeviceHistory>(m =>
                           m.CompanyId == companyId
                           && m.GroupId == groupId
                           && m.opcode == opcode
                           && m.AtTime >= begin
                           && m.AtTime <= end).Select(m => new SetupDeviceTranfer()
                           {
                               Id = m.Id,
                               AtTime = m.AtTime,
                               CompanyId = m.CompanyId,
                               opcode = m.opcode,
                               GroupId = m.GroupId,
                               Retry = m.Retry,
                               Note = m.Note,
                               Serial = m.Serial,
                               Username = m.Username
                           }).ToList();
                        }
                }
            }
            catch (Exception e)
            {
                _log.Exception("GetSetupDeviceHistory", e, "Lỗi lấy lịch sử truy xuất");
            }
            return new List<SetupDeviceTranfer>(0);
        }

    }
}