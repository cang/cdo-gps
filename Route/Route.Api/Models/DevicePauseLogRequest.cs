#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 2:13 PM 18/12/2016
// FILENAME: DevicePauseLogRequest.cs
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
    public class DevicePauseLogRequest : BaseResponse
    {
        public DevicePauseLogRequest()
        {
            Status = 1;
        }

        /// <summary>
        ///     thông tin dừng đỗ xe
        /// </summary>
        [DataMember]
        public List<DevicePauseLogTranfer> DevicePauseLogList { set; get; } = new List<DevicePauseLogTranfer>();
    }
}