#region header
// /*********************************************************************************************/
// Project :Authentication
// FileName : LoginReq.cs
// Time Create : 3:53 PM 14/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

namespace Route.Api.Auth.Models.Req
{
    public class LoginTranfer
    {
        public string Username { set; get; }
        public string Pwd { get; set; }
    }
}