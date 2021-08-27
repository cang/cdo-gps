using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Route.Api.Auth.Models.Req
{
    /// <summary>
    /// nhóm user
    /// </summary>
    public class GroupUserTranfer
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Functions { get; set; }
        public string Parent { get; set; }
    }

    /// <summary>
    /// level
    /// </summary>
    public class AccountLevelTransfer
    {
        public int    Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}