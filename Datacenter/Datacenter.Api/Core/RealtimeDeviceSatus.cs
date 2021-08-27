#region header
// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : RealtimeDeviceSatus.cs
// Time Create : 10:03 AM 01/10/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Log;
using Microsoft.AspNet.SignalR.Client;
using StarSg.Utils.Models.DatacenterResponse.Status;
using Datacenter.Api.Models;

namespace Datacenter.Api.Core
{
    /// <summary>
    /// cung cấp thông tin các thiết bị qua cho route
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class RealtimeDeviceSatus:IPartImportsSatisfiedNotification
    {
        [Import] private ILog _log;
        private HubConnection _hubConnection = null;
        private IHubProxy _stockTickerHubProxy;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public async void OnImportsSatisfied()
        {
            //InitHubConnection();
        }

        public async void InitHubConnection()
        {
            try
            {
                _log.Info("SYSTEM", $"ResponseDataConfig.RouteDomainUrl {ResponseDataConfig.RouteDomainUrl}");

                _hubConnection = new HubConnection(ResponseDataConfig.RouteDomainUrl);
                _stockTickerHubProxy = _hubConnection.CreateHubProxy("UpdateDeviceStatus");
                // await _hubConnection.Start();
                await Task.Factory.StartNew(ValidConnection);
            }
            catch (Exception e)
            {
                _log.Exception("SYSTEM", e, "InitHubConnection");
            }
        }

        private async void ValidConnection()
        {
            while (true)
            {
                try
                {
                    if (_hubConnection.State == ConnectionState.Disconnected)
                    {
                        _log.Warning("SYSTEM","Reconect websocket");
                        await _hubConnection.Start();
                    }
                }
                catch (Exception e)
                {
                    _log.Exception("SYSTEM", e, "Connect Error");
                    //Console.WriteLine(e);
                }
                await Task.Delay(10000);
            }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        public async void UpdateDeviceSatatus(StatusDeviceTranfer status)
        {
            if (_hubConnection == null) return;
            try
            {
                if (_hubConnection.State != ConnectionState.Connected) return;
                //todo: delay ???
                await _stockTickerHubProxy.Invoke<StatusDeviceTranfer>("Update", new List<StatusDeviceTranfer> { status });
            }
            catch (Exception e)
            {
                _log.Exception("SYSTEM", e, "UpdateDeviceSatatus");
            }
        } 


    }
}