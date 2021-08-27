#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : RoleResponse.cs
// Time Create : 10:23 AM 23/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using Route.Api.Auth.Models.Entity;
using StarSg.Core;
using System.Collections.Generic;

namespace Route.Api.Auth.Models.Response
{
    public class RoleResponse:BaseResponse
    {
        public int Level { set; get; }
        public IList<Functions> Funcs { get; set; } = new List<Functions>();
        public string Role { get; set; }
        public string Username { get; set; }
    }
}