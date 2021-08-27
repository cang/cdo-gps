using System;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using System.Web.Services.Description;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Route.Core;
using StarSg.Core;
using StarSg.Utils.Models.Auth;
using System.ComponentModel.Composition;
using Route.Api.Core;
using Route.Api.Auth.Core;

namespace Route.Api.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AuthorizeClaimsAttribute : AuthorizeAttribute
    {
        #region Overrides of AuthorizeAttribute

        /// <summary>
        ///     Determines whether client is authorized to connect to <see cref="T:Microsoft.AspNet.SignalR.Hubs.IHub" />.
        /// </summary>
        /// <param name="hubDescriptor">Description of the hub client is attempting to connect to.</param>
        /// <param name="request">The (re)connect request from the client.</param>
        /// <returns>
        ///     true if the caller is authorized to connect to the hub; otherwise, false.
        /// </returns>
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            ////todo: chưa tìm được cách lấy ra _accountManager tạm thời cho qua
            //return true;

            // todo: hàm này tính toán sự hợp lệ của connection 
            // todo: lấy kiểm tra token có hợp lệ hay ko 
            try
            {
                if (request.QueryString["token"] == null) return false;
                if (request.QueryString["companyId"] == null) return false;
                var token = request.QueryString["token"];

                //    var api = new ForwardApi();
                //    var user = api.Get<BaseResponse>($"{Domain}:{Port}/Auth/CheckToken?token={token}");
                //    if (user.Status==0) return false;
                //    return true;

                //MefLoader.Container.GetExportedValue<UserPermissionManager>();//Luật add ???
                UserPermissionManager UserPermision = new UserPermissionManager(token, AccountManager.Instance);
                return UserPermision.Check();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            ////var deviceTable = MefLoader.Container.GetExportedValue<IDeviceRouteTable>();
            ////var allSerial = user.Level < (int)AccountLevel.CustomerMaster ? deviceTable.GetAll() : user.DeviceSerial;

            ////ClientManager.RegisterConnection(token, Clients.Caller, allSerial);
            //return base.AuthorizeHubConnection(hubDescriptor, request);
        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            return true;
            //return base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
        }

        #endregion
    }
}