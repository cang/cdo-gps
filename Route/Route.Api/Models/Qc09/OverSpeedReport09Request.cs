#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 1:46 PM 18/12/2016
// FILENAME: OverSpeedReport09Request.cs
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
    public class OverSpeedReport09Request : BaseResponse
    {
        public OverSpeedReport09Request()
        {
            Status = 1;
        }
        
        public IList<OverSpeedReport09Tranfer> OverSpeedReport09List { set; get; } =
            new List<OverSpeedReport09Tranfer>();
    }
}