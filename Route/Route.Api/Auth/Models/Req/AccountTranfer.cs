using System.Collections.Generic;
using System.Linq;

namespace Route.Api.Auth.Models.Req
{
    /// <summary>
    /// tài khoản
    /// </summary>
    public class AccountTranfer : AccountInfo
    {
        public const long COMPANYLIMIT = 100000000L;
        public string Username { get; set; }
        public string Pwd { get; set; }
        public bool IsBlock { get; set; }
        public string RoleId { get; set; }
        public List<long> CompanyIds { get; set; }
        public List<long> DeviceIds { get; set; }
        public int Level { get; set; }

        //public string DisplayName { get; set; }
        //public string Phone { get; set; }

        public bool ContainsCompanyIds(long companyId)
        {
            if (companyId > COMPANYLIMIT) companyId = companyId / COMPANYLIMIT;
            return (CompanyIds?.Any(uid => (uid > COMPANYLIMIT ? uid / COMPANYLIMIT : uid) == companyId) ?? false);
        }
    }

    public class AccountInfo
    {
        public string DisplayName { get; set; }
        public string Phone { get; set; }
    }
}