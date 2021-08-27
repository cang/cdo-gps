#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Api
// TIME CREATE : 1:03 PM 18/12/2016
// FILENAME: Interface1.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using System.Collections.Generic;

namespace Datacenter.Api.Core
{
    /// <summary>
    /// chia request nếu số lượng thiết bị nhiều
    /// </summary>
    public interface ISplitRequest
    {
        /// <summary>
        /// Chia ra các request nhỏ tới datacenter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="logicWhere"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IList<T> Split<T, T1>(IList<T1> logicWhere, Func<IList<T1>, IList<T>> callback);
        /// <summary>
        /// Chia ra các request nhỏ tới datacenter
        /// Dữ liệu có thể bị xáo trộn do chay multithread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="logicWhere"></param>
        /// <param name="callback"></param>
        /// <param name="multiTask"></param>
        /// <returns></returns>
        IList<T> Split<T, T1>(IList<T1> logicWhere, Func<IList<T1>, IList<T>> callback, bool multiTask);
    }
}