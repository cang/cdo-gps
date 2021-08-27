using System.Collections.Generic;
using Core.Models.Auth;
using StarSg.Core;

namespace Route.Api.Auth.Models.Response
{
    /// <summary>
    ///     danh sách log của user
    /// </summary>
    public class UserLogResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách log của user
        /// </summary>
        public List<UserLogTranfer> UserLogTranfers { get; set; } = new List<UserLogTranfer>();
    }
}