#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 2:13 PM 18/12/2016
// FILENAME: DriverSessionLogRequest.cs
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
    ///     thông tin thời gian lái xe
    /// </summary>
    public class DriverSessionLogRequest : BaseResponse
    {
        public DriverSessionLogRequest()
        {
            Status = 1;
        }

        /// <summary>
        ///     thông tin thời gian lái xe
        /// </summary>
        public List<DriverSessionLogTranfer> DriverSessionLogList { set; get; } = new List<DriverSessionLogTranfer>();
    }
}