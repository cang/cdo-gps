using System.Collections.Generic;
using Route.Api.Auth.Models.Req;
using StarSg.Core;

namespace Route.Api.Auth.Models.Response
{
    /// <summary>
    ///     danh sách quyền truy cập
    /// </summary>
    public class PermissionResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách quyền truy cập
        /// </summary>
        public List<FunctionsTranfer> FunctionsTranfers { get; set; } = new List<FunctionsTranfer>();
    }
}