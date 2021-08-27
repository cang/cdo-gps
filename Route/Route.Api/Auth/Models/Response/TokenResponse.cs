using Route.Api.Auth.Models.Response;
using StarSg.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authentication.Models.Response
{
    public class TokenResponse : BaseResponse
    {
        public string Token { get; set; }
    }
}