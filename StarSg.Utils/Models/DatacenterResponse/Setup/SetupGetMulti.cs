using System.Collections;
using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.Setup
{
    public class SetupGetMulti:BaseResponse
    {
        public  IList<SetupInfoTranfer> SetupInfos { set; get; }
    }
}