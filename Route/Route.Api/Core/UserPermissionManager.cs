using System.Collections.Generic;
using System.Linq;
using StarSg.Core;
using StarSg.Utils.Models.Auth;
using StarSg.Utils.Models.DatacenterResponse.DeviceGroup;
using Route.Api.Auth.Models.Entity;

namespace Route.Api.Core
{
    /// <summary>
    ///     quản lý thông tin truy cập của người sử dụng
    /// </summary>
    public class UserPermissionManager : IUserPermisionManager
    {
        /// <summary>
        /// Giới hạn tối đa của id công ty
        /// </summary>
        public const long COMPANYLIMIT = 100000000L;

        private readonly UserInfoTranfer _user;

        /// <summary>
        /// UserPermissionManager
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        public UserPermissionManager(string token, Auth.Core.IAccountManager account)
        {
            if (account==null || !account.ValidToken(token))
            {
                _user = new UserInfoTranfer { IsValid = false, Level = (int)AccountLevel.Customer };
                //var apiAuth = new ForwardApi();
                //_user = apiAuth.Get<UserInfoTranfer>($"{domain}:{port}/Auth/GetUserInfo?token={token}");
            }
            else
            {
                var user = account.GetUser(token);
                _user = new UserInfoTranfer
                {
                    UserName = user.Username,
                    CompanyId = user.CompanyIds?.Select(m => m.CompanyId).ToList(),
                    Level = (int)user.Level,
                    DeviceSerial = user.DeviceIds?.Select(m => m.Serial).ToList(),
                    IsValid = true,
                    GroupUserId = user.GroupUserId
                };
            }
        }

        #region Implementation of IUserPermisionManager

        /// <summary>
        /// Lấy công ty đâu tiên mà user quản lý, 0 là k có cty nào
        /// </summary>
        /// <returns></returns>
        public long GetFirstCompany()
        {
            return _user?.CompanyId.FirstOrDefault() ?? 0;
        }

        /// <summary>
        /// Lấy danh sách công ty mà user quản lý
        /// </summary>
        /// <returns></returns>
        public IList<long> GetCompany()
        {
            return _user?.CompanyId ?? new List<long>();
        }

        /// <summary>
        /// Lấy danh sách thiết bị
        /// </summary>
        /// <returns></returns>
        public IList<long> GetDevice()
        {
            return _user?.DeviceSerial ?? new List<long>();
        }

        /// <summary>
        /// kiểm tra xem công ty này có được quản lý hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainCompanyId(long id)
        {
            if (_user.Level < (int)AccountLevel.CustomerMaster) return true;
            //return _user?.CompanyId?.Contains(id) ?? false;
            return _user?.CompanyId?.Any(uid => (uid > COMPANYLIMIT ? uid / COMPANYLIMIT : uid) == id) ?? false;
        }

        /// <summary>
        /// lấy id cty nằm trong user : 0 có nghĩa là không có cty nào được quản lý
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public long GetUserCompanyId(long id)
        {
            if (_user.Level < (int)AccountLevel.CustomerMaster) return id;
            return _user?.CompanyId?.FirstOrDefault(uid => (uid > COMPANYLIMIT ? uid / COMPANYLIMIT : uid) == id) ?? 0;
        }

        /// <summary>
        /// lấy id nhóm nằm trong user : 0 có nghĩa là không có nhóm nào được quản lý, -1 có nghĩa là tất cả các nhóm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public long GetUserGroupId(long id)
        {
            if (_user.Level < (int)AccountLevel.CustomerMaster) return -1;
            long ret = GetUserCompanyId(id);
            if (ret == id) return -1;
            return ret % COMPANYLIMIT;
        }

        /// <summary>
        /// Kiểm tra và lọc bõ những nhóm không thuộc quyền của user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groups"></param>
        public void EnsureGroup(long id, DeviceGroupGetAll groups)
        {
            if (groups == null) return;
            if (_user.Level < (int)AccountLevel.CustomerMaster) return;

            long groupId = GetUserGroupId(id);
            if (groupId == -1) return;
            if (groupId <= 0)
            {
                groups.Groups.Clear();
                return;
            }
            groups.Groups = groups.Groups.Where(m => m.Id == groupId).ToList();
        }

        /// <summary>
        /// kiểm tra xem thiết bị có được quản lý hay không
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public bool ContainDeviceSerial(long serial)
        {
            if (_user.Level < (int)AccountLevel.Customer ) return true;
            return _user?.DeviceSerial?.Contains(serial) ?? false;
        }

        /// <summary>
        /// Lấy cấp bậc tài khoản
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            return _user?.Level ?? 0;
        }

        /// <summary>
        /// Kiểm tra  xem user có hợp lệ hay không
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            return _user?.IsValid ?? false;
        }

        #endregion


        /// <summary>
        /// User Information
        /// </summary>
        public UserInfoTranfer User
        {
            get
            {
                return _user;
            }
        }

    }
}