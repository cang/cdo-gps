#region header

// /*********************************************************************************************/
// Project :DevicePacketModels
// FileName : DateTimeConvert.cs
// Time Create : 8:47 AM 04/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace DevicePacketModels.Utils
{
    public class DateTimeConvert
    {
        public static DateTime GetTimeByUnixTime(long tick)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(tick).ToLocalTime();
            return dtDateTime;
        }

        public static long GetTimeByUnixTime(DateTime time)
        {
            return (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;
        }

    }
}