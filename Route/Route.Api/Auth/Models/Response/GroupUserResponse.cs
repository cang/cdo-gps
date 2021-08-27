using System.Collections.Generic;
using Route.Api.Auth.Models.Req;
using StarSg.Core;

namespace Route.Api.Auth.Models.Response
{
    /// <summary>
    ///     danh sách nhóm user
    /// </summary>
    public class GroupUserResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách nhóm user
        /// </summary>
        public List<GroupUserTranfer> GroupUserTranfers { get; set; } = new List<GroupUserTranfer>();
    }

    /// <summary>
    ///     danh sách level
    /// </summary>
    public class AccountLevelResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách level
        /// </summary>
        public List<AccountLevelTransfer> AccountLevels { get; set; } = new List<AccountLevelTransfer>();
    }

}