#region header

// /*********************************************************************************************/
// Project :Models
// FileName : DateTimeFix.cs
// Time Create : 9:40 AM 17/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;

namespace Datacenter.Model.Utils
{
    public static class DateTimeFix
    {
        public static readonly DateTime Min = new DateTime(2000, 1, 1);
        public static readonly DateTime Max = new DateTime(3000, 1, 1);

        /// <summary>
        ///     config lấy theo ngày
        /// </summary>
        public static readonly int CurrentDayCompare = 0;

        public static DateTime Fix(this DateTime time)
        {
            if (time <= Min || time >= Max)
            {
                return new DateTime(2010, 1, 1);
            }
            return time;
        }
        public static bool IsValidDatetime(this DateTime time)
        {
            if (time <= Min || time >= Max)
            {
                return false;
            }
            return true;
        }
        public static bool IsValidLostData(this DateTime time)
        {
            try
            {
                if (time.Year >= DateTime.Now.Year - 2 && time.Year <= DateTime.Now.Year + 2)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }
        /// <summary>
        ///     check ngày truyền vào nhỏ hơn ngày hiện tại bao nhiêu
        ///     nếu nhỏ hơn số ngày dayNumber thì return true và ngược lại
        /// </summary>
        /// <param name="timeCheck"></param>
        /// <param name="dayNumber"></param>
        /// <returns></returns>
        public static bool CheckCurrentDate(this DateTime timeCheck, int dayNumber)
        {
            //return true;
            //todo chạy khi tính năng zip hoạt động
            var subDate = DateTime.Now.AddDays(-dayNumber);
            if (timeCheck.Date >= new DateTime(subDate.Year, subDate.Month, subDate.Day, 0, 0, 0))
            {
                return true;
            }
            return false;
        }
    }
}