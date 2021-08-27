#region header
// /*********************************************************************************************/
// Project :DataCenter.Core
// FileName : ILocationQuery.cs
// Time Create : 9:57 AM 20/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using Datacenter.Model.Components;

namespace DataCenter.Core
{
    /// <summary>
    /// quản lý thông tin địa chỉ
    /// </summary>
    public interface ILocationQuery
    {
        /// <summary>
        /// truy vấn địa chỉ
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        string GetAddress(float lat, float lng);


        /// <summary>
        /// truy vấn địa chỉ
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        string GetAddress(GpsLocation point);
    }
}