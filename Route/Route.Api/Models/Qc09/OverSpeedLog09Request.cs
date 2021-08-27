#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 1:46 PM 18/12/2016
// FILENAME: OverSpeedLog09Request.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer.Qc09;

#endregion

namespace Route.Api.Models.Qc09
{
    /// <summary>
    ///     quá tốc độ theo thông tư 09
    /// </summary>
    public class OverSpeedLog09Request : BaseResponse
    {
        public OverSpeedLog09Request()
        {
            Status = 1;
        }
        
        public IList<OverSpeedLog09Tranfer> OverSpeed09LogList { set; get; } = new List<OverSpeedLog09Tranfer>();
    }
}