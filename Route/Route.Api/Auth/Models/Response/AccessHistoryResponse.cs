using System.Collections.Generic;
using Core.Models.Auth;
using StarSg.Core;

namespace Route.Api.Auth.Models.Response
{
    /// <summary>
    ///     danh sách lịch sử truy xuất
    /// </summary>
    public class AccessHistoryResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách lịch sử truy xuất
        /// </summary>
        public List<AccessHistoryTranfer> Data { get; set; } = new List<AccessHistoryTranfer>();
    }

    public class SetupDeviceHistoryResponse : BaseResponse
    {
        /// <summary>
        ///     danh sách lịch sử truy xuất
        /// </summary>
        public List<SetupDeviceTranfer> Data { get; set; } = new List<SetupDeviceTranfer>();
    }


}