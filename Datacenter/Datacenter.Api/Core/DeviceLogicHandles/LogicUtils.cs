#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : LogicUtils.cs
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
    /// </summary>
    public class LogicUtils : ILogicUtil
    {
        #region Implementation of ILogicUtil

        /// <summary>
        ///     quản lý thông tin địa chỉ
        /// </summary>
        public ILocationQuery LocationQuery { get; set; }

        /// <summary>
        ///     quản lý thông tin dữ liệu sql
        /// </summary>
        public IQueryRoute DataContext { get; set; }

        /// <summary>
        ///     quản lý thông tin dữ liệu cache
        /// </summary>
        public IDataStore DataCache { get; set; }

        /// <summary>
        ///     log
        /// </summary>
        public ILog Log { get; set; }

        /// <summary>
        ///     id của máy chủ mẹ
        /// </summary>
        public int MotherSqlId { get; set; }

        /// <summary>
        ///     truyền thông tin lên tổng cục
        /// </summary>
        public IBgtTranport BgtTranport { get; set; }

        public CacheLogManager CacheLog { get; set; }

        /// <summary>
        /// 0 : bình thường 1: nén, 2 : ..
        /// </summary>
        public byte PacketType { get; set; }

        #endregion
    }
}