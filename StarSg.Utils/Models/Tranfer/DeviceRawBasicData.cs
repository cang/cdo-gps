#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceRawBasicData.cs
// Time Create : 5:24 PM 22/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;

namespace Core.Models.Tranfer
{
    /// <summary>
    ///     Chứa thông tin cơ bản của gói tin cập nhật từ thiết bị
    /// </summary>
    public class DeviceRawBasicData
    {
        /// <summary>
        ///     các gói tin có trong packet
        /// </summary>
        public IList<string> Datas { get; set; } = new List<string>();

        /// <summary>
        ///     Thông tin kèm theo
        /// </summary>
        public string Cookie { get; set; }
    }
}