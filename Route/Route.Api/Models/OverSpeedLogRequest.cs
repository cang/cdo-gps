#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 2:13 PM 18/12/2016
// FILENAME: OverSpeedLogRequest.cs
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
using StarSg.Utils.Models.Tranfer.DeviceManager;

#endregion

namespace Route.Api.Models
{
    /// <summary>
    ///     thông tin chạy quá tốc độ
    /// </summary>
    [DataContract]
    public class OverSpeedLogRequest : BaseResponse
    {
        public OverSpeedLogRequest()
        {
            Status = 1;
        }

        /// <summary>
        ///     thông tin chạy quá tốc độ
        /// </summary>
        [DataMember]
        public List<OverSpeedLogTranfer> OverSpeedLogList { set; get; } = new List<OverSpeedLogTranfer>();
    }

    /// <summary>
    /// log mấy tín hiệu
    /// </summary>
    public class DeviceTraceLogRequest : BaseResponse
    {
        /// <summary>
        /// log mấy tín hiệu
        /// </summary>
        
        public List<DeviceTraceLogTranfer> DeviceTraceLogList { set; get; } = new List<DeviceTraceLogTranfer>();

        public DeviceTraceLogRequest()
        {
            Status = 1;
        }
    }
}