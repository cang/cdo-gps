#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 2:13 PM 18/12/2016
// FILENAME: DeviceSpeedLogRequest.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

#endregion

namespace Route.Api.Models
{
    /// <summary>
    ///     thông tin tốc độ xe
    /// </summary>
    [DataContract]
    public class DeviceSpeedLogRequest : BaseResponse
    {
        public DeviceSpeedLogRequest()
        {
            Status = 1;
        }

        /// <summary>
        ///     thông tin tốc độ xe
        /// </summary>
        [DataMember]
        public List<DeviceSpeedLogTranfer> DeviceSpeedLogList { set; get; } = new List<DeviceSpeedLogTranfer>();
    }
}