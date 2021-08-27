#region header

// /*********************************************************************************************/
// Project :Route.Api
// FileName : DeviceStatusHub.cs
// Time Create : 10:17 AM 30/09/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.DynamicData;
using Microsoft.AspNet.SignalR;
using Route.Core;
using StarSg.Core;
using StarSg.Utils.Models.Auth;
using StarSg.Utils.Models.DatacenterResponse.Status;
using Route.Api.Core;
using System.ComponentModel.Composition;
using Route.Api.Auth.Core;
using System;
using Route.Api.Auth.Models.Entity;
using Route.Api.Auth.Core.ConfigFile;


#endregion

namespace Route.Api.Hubs
{
    [AuthorizeClaimsAttribute]
    public class DeviceStatusHub : Hub<IWClient>
    {
        public static HubClientConnectionManager<IWClient> ClientManager { get; } =
            new HubClientConnectionManager<IWClient>();

        /// <summary>
        ///     lấy thông tin toàn bộ trạng thái của thiết bị
        /// </summary>
        /// <returns></returns>
        public IList<StatusDeviceTranfer> GetAll()
        {
            var companyId = long.Parse(Context.QueryString["companyId"]);
            var groupId = Context.QueryString["groupId"];
            var token = Context.QueryString["token"];

            var api = new ForwardApi();
            UserPermissionManager UserPermision = new UserPermissionManager(token, AccountManager.Instance);

            //level 0 1 2 được coi tới 93 ngày, level 3 đc coi 31 ngày
            int expireDay = UserPermision.GetLevel() >= (int)AccountLevel.Customer ? 0 : 93;


            #region unknown devices
            if (companyId <= 0 && UserPermision.GetLevel() < (int)AccountLevel.CustomerMaster)
            {
                api.AddHeader("token", token);
                var resutlt =
                    api.Get<StatusDeviceGet>(
                        $"{AuthConfig.RouteDomainUrl}/api/Status/GetDeviceStatusByCompanyId?companyId={companyId}&expireDay={expireDay}");
                if (resutlt.Status != 1)
                    return new List<StatusDeviceTranfer>();
                return resutlt.Datas;

            }
            #endregion unknown devices

            var companyRoute = MefLoader.Container.GetExportedValue<ICompanyRouteTable>();
            var center = companyRoute.GetDataCenter(companyId);
            if (center == null) return new List<StatusDeviceTranfer>();

            #region dành cho khách lẻ
            if (UserPermision.GetLevel() >= (int)AccountLevel.Customer )
            {
                IList<long> serials = UserPermision?.User?.DeviceSerial ?? null;
                if (serials != null && serials.Count > 0)
                {
                    var resutlt = api.Get<StatusDeviceGet>($"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusBySerials?serials={String.Join(",", serials)}&expireDay={expireDay}");
                    if (resutlt.Status != 1)
                        return new List<StatusDeviceTranfer>();
                    return resutlt.Datas;
                }

                //lọc cho quản lý nhóm (không sử dụng tham số groupId truyền vô)
                long group = UserPermision.GetUserGroupId(companyId);
                if (group == 0) return new List<StatusDeviceTranfer>();
                else if (group > 0) groupId = group.ToString();
            }
            #endregion dành cho khách lẻ

            //CHƯA CẦN THIẾT LỌC CHỖ NÀY
            ////admin của đại lý
            //if (UserPermision.GetLevel() == (int)AccountLevel.CustomerMaster)
            //    if(group>-1) return new StatusDeviceGet { Description = "Người dùng này không quản lý cty nào cả" };

            if (string.IsNullOrWhiteSpace(groupId))
            {
                var resutlt =
                    api.Get<StatusDeviceGet>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByCompanyId?companyId={companyId}&expireDay={expireDay}");
                if (resutlt.Status != 1)
                    return new List<StatusDeviceTranfer>();
                return resutlt.Datas;
            }
            else
            {
                var resutlt =
                    api.Get<StatusDeviceGet>(
                        $"{center.Ip}:{center.Port}/api/Status/GetDeviceStatusByGroupId?companyId={companyId}&groupId={long.Parse(groupId)}&expireDay={expireDay}");
                if (resutlt.Status != 1)
                    return new List<StatusDeviceTranfer>();
                return resutlt.Datas;

            }

        }

        #region Overrides of HubBase

        /// <summary>
        ///     Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnConnected()
        {
            var token = Context.QueryString["token"];
            UserPermissionManager userPermision = new UserPermissionManager(token, AccountManager.Instance);

            //var deviceTable = MefLoader.Container.GetExportedValue<IDeviceRouteTable>();
            var allSerial = new List<long>();


            if(userPermision.GetLevel() < (int)AccountLevel.Customer)
                allSerial = GetAll().Select(m => m.Serial).ToList();
            else
            {
                //đây là khách lẻ
                if (userPermision.User.DeviceSerial != null && userPermision.User.DeviceSerial.Count > 0)
                    allSerial = userPermision.User.DeviceSerial.ToList();

                // đây là đội, chỉ lấy danh sách device của đội
                else
                    allSerial = GetAll().Select(m => m.Serial).ToList();
            }

            ClientManager.RegisterConnection(token, Clients.Caller,Context.ConnectionId, allSerial);

            return base.OnConnected();
        }


        /// <summary>
        ///     Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">
        ///     true, if stop was called on the client closing the connection gracefully;
        ///     false, if the connection has been lost for longer than the
        ///     <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
        ///     Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            var token = Context.QueryString["token"];
            ClientManager.UnregisterConnection(token, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        #endregion
    }
}