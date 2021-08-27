#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : BaseDeviceController.cs
// Time Create : 10:56 AM 13/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http.Dependencies;
using System.Web.Http.Description;
using Datacenter.Model.Entity;

#endregion

namespace Datacenter.Api.Controllers
{
    public class BaseDeviceController : BaseController
    {
        public Device Device { get; set; }
        public Company Company { get; set; }
        public Driver Driver { get; set; }
        public DeviceGroup Group { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override bool ValidAccess(IDependencyResolver dependency, HttpRequestHeaders header)
        {
            IEnumerable<string> tmp;
            if (!header.TryGetValues("serial", out tmp) || !tmp.Any()) return false;
            var serial = long.Parse(tmp.First());
            Device = Cache.GetQueryContext<Device>().GetByKey(serial);
            if (Device == null) return false;
            Company = Cache.GetCompanyById(Device.CompanyId);
            Group = Cache.GetQueryContext<DeviceGroup>().GetByKey(Device.GroupId);
            Driver = Cache.GetQueryContext<Driver>().GetByKey(Device.Status?.DriverStatus.DriverId ?? 0L);
            return base.ValidAccess(dependency, header);
        }
    }
}