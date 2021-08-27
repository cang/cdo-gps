using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using StarSg.Utils.Models.DatacenterResponse.Status;

namespace Datacenter.TestSignalR
{
    class Program
    {
        static void Main(string[] args)
        {

            Task.Factory.StartNew(async () =>
            {
                var _hubConnection = new HubConnection("http://route.sgsi.vn/", new Dictionary<string, string>()
                {
                    {"token", "260a3255-af5a-41fa-80db-8f042f8599c8"},
                    {"companyId", "1"}
                });

                var _stockTickerHubProxy = _hubConnection.CreateHubProxy("DeviceStatusHub");
                _stockTickerHubProxy.On<StatusDeviceTranfer>("Update", m =>
                {
                    Console.WriteLine($"Update  {m.Serial}");
                });
                await _hubConnection.Start();

                var t = await _stockTickerHubProxy.Invoke<IList<StatusDeviceTranfer>>("GetAll");
                Console.WriteLine(t.Count());
            });
           

            Console.Read();
        }
    }
}
