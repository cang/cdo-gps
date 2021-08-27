#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : ILogicUtil.cs
// Time Create : 9:41 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using Datacenter.DataTranport;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles
{
    /// <summary>
    ///     chưa các thư viện cần thiết cho 1 logic chạy
    /// </summary>
    public interface ILogicUtil
    {
        /// <summary>
        ///     quản lý thông tin địa chỉ
        /// </summary>
        ILocationQuery LocationQuery { get; }

        /// <summary>
        ///     quản lý thông tin dữ liệu sql
        /// </summary>
        IQueryRoute DataContext { get; }

        /// <summary>
        ///     quản lý thông tin dữ liệu cache
        /// </summary>
        IDataStore DataCache { get; }

        /// <summary>
        ///     log
        /// </summary>
        ILog Log { get; }

        /// <summary>
        ///     id của máy chủ mẹ
        /// </summary>
        int MotherSqlId { get; }

        /// <summary>
        ///     truyền thông tin lên tổng cục
        /// </summary>
        IBgtTranport BgtTranport { get; }

        CacheLogManager CacheLog { get; }

        /// <summary>
        /// 0 : bình thường 1: nén, 2 : ..
        /// </summary>
        byte PacketType { get; }
    }
}