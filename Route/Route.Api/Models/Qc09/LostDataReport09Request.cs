#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 1:46 PM 18/12/2016
// FILENAME: LostDataReport09Request.cs
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
    ///     requet thông kê vi phạm truyền data lên bộ
    /// </summary>
    public class LostDataReport09Request : BaseResponse
    {
        public LostDataReport09Request()
        {
            Status = 1;
        }
        
        public List<LostDataReport09Tranfer> LostDataReport09List { set; get; } = new List<LostDataReport09Tranfer>();
    }
}