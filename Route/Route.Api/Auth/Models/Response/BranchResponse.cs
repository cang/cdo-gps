using Route.Api.Auth.Models.Req;
using StarSg.Core;
using System.Collections.Generic;

namespace Route.Api.Auth.Models.Response
{
    public class BranchResponse : BaseResponse
    {
        public BranchTranfer Data { get; set; } = new BranchTranfer();
    }

    public class BranchResponses : BaseResponse
    {
        public List<BranchTranfer> Data { get; set; } = new List<BranchTranfer>();
    }

}