using Route.Api.Auth.Models.Req;
using StarSg.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Route.Api.Auth.Models.Response
{
    public class SystemPairResponse : BaseResponse
    {
        public SystemPairTransfer Data { get; set; }
    }

    public class SystemPairResponses : BaseResponse
    {
        public List<SystemPairTransfer> Data { get; set; } = new List<SystemPairTransfer>();
    }
}