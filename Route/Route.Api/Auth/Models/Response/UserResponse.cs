using Route.Api.Auth.Models.Req;
using StarSg.Core;
using System.Collections.Generic;

namespace Route.Api.Auth.Models.Response
{
    /// <summary>
    ///     danh sách user
    /// </summary>
    public class UserResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách user
        /// </summary>
        public List<AccountTranfer> AccountTranfers { get; set; } = new List<AccountTranfer>();
    }

    /// <summary>
    ///     Single User
    /// </summary>
    public class UsernameResponse : BaseResponse
    {
        /// <summary>
        ///     user
        /// </summary>
        public AccountTranfer User { get; set; } = new AccountTranfer();
    }

}