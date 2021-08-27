#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceChangeTranfer.cs
// Time Create : 4:45 PM 25/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace Core.Models.Tranfer
{
    public class DeviceChangeTranfer
    {
        /// <summary>
        ///     Số serial cũ cần thay
        /// </summary>
        public string OldSerial { get; set; }

        /// <summary>
        ///     Số serial mới
        /// </summary>
        public string NewSerial { get; set; }
    }
}