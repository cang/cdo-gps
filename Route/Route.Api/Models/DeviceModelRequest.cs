#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 2:13 PM 18/12/2016
// FILENAME: DeviceModelRequest.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using Core.Models.Tranfer;
using StarSg.Core;

#endregion

namespace Route.Api.Models
{
    /// <summary>
    ///     request của device model
    /// </summary>
    [DataContract]
    public class DeviceModelRequest : BaseResponse
    {
        public DeviceModelRequest()
        {
            Status = 1;
        }

        /// <summary>
        ///     thông tin loại thiết bị
        /// </summary>
        [DataMember]
        public List<DeviceModelTranfer> DeviceModelList { set; get; } = new List<DeviceModelTranfer>();
    }
}