#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: StarSg.Core
// TIME CREATE : 10:02 PM 25/10/2016
// FILENAME: IPlan.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using Quartz;

namespace StarSg.PlanJob
{
    public interface IPlan
    {
        /// <summary>
        /// cài đặt 1 công việc vào thời gian dc chỉ định
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="time"></param>
        /// <returns></returns>
        bool CreatePlan<T>(TimeSpan time) where T : IJob;

        /// <summary>
        /// thêm các đối tượng sử dụng cho các job khi phát sinh
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool PutObjectToContext(string key, object obj);
    }
}