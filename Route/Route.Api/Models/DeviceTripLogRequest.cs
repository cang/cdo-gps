#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 2:13 PM 18/12/2016
// FILENAME: DeviceTripLogRequest.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.Qc31;

#endregion

namespace Route.Api.Models
{
    /// <summary>
    ///     thông tin dừng đỗ xe
    /// </summary>
    [DataContract]
    public class DeviceTripLogRequest : BaseResponse
    {
        public DeviceTripLogRequest()
        {
            Status = 1;
        }

        /// <summary>
        ///     thông tin dừng đỗ xe
        /// </summary>
        [DataMember]
        public List<DeviceTripLogTranfer> DeviceTripLogList { set; get; } = new List<DeviceTripLogTranfer>();
    }
}