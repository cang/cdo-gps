using System.Collections.Generic;
using StarSg.Core;

namespace StarSg.Utils.Models.DatacenterResponse.System
{
    /// <summary>
    /// thông tin log
    /// </summary>
    public class SysLogResponse:BaseResponse
    {
        /// <summary>
        /// thông tin log
        /// </summary>
        public IList<string> Datas { get; set; }
        /// <summary>
        /// Index của log
        /// </summary>
        public long Index { get; set; }

    }
}