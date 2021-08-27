#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 1:46 PM 18/12/2016
// FILENAME: GeneralReport09Request.cs
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
    public class GeneralReport09Request : BaseResponse
    {
        public GeneralReport09Request()
        {
            Status = 1;
        }

        [DataMember]
        public IList<GeneralReport09Log> GeneralReport09List { set; get; } = new List<GeneralReport09Log>();
    }
}