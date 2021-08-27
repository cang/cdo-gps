#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 1:46 PM 18/12/2016
// FILENAME: DriverSession10HRequest.cs
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

namespace Route.UserRoute.Models.Qc09
{
    public class DriverSession10HRequest : BaseResponse
    {
        public DriverSession10HRequest()
        {
            Status = 1;
        }
        
        public IList<DriverSession10HTranfer> DriverSession10HList { set; get; } = new List<DriverSession10HTranfer>();
    }
}