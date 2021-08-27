#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : SortAttribute.cs
// Time Create : 9:41 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles
{
    /// <summary>
    ///     sắp xếp thứ tự
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SortAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public SortAttribute(int index)
        {
            Index = index;
        }

        /// <summary>
        ///     vị trí
        /// </summary>
        public int Index { get; private set; }
    }
}