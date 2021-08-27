using System.Collections;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class EnterpriseDeviceAllReportGet:BaseResponse
    {
         public List<EnterpriseDeviceAllReportTranfer> Datas { get; set; }
    }

    public class EnterpriseDeviceFuelReportGet : BaseResponse
    {
        public List<EnterpriseDeviceFuelReportTranfer> Datas { get; set; }
    }

    public class EnterpriseDeviceGuestReportGet : BaseResponse
    {
        public List<EnterpriseDeviceGuestReportTranfer> Datas { get; set; }
    }
}