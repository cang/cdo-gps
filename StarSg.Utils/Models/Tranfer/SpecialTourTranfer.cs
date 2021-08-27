#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceTranfer.cs
// Time Create : 9:39 AM 24/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer
{
    /// <summary>
    ///     Thông tin cuốc ngoại tỉnh/đặc biệt
    /// </summary>
    public class SpecialTourTranfer
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Mã số xe
        /// </summary>
        public virtual long Serial { get; set; }

        /// <summary>
        /// Ngày dự kiến : không xét giờ hoặc lúc 0 giờ
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        ///     Đi ngoài/lần
        /// </summary>
        public virtual String HowTimes { get; set; }

        /// <summary>
        ///     Đi tỉnh/địa điểm
        /// </summary>
        public virtual String Address { get; set; }

        /// <summary>
        ///     KM dự kiến
        /// </summary>
        public virtual int KmOnPlan { get; set; }

        /// <summary>
        ///     Ghi chú
        /// </summary>
        public virtual String Note { get; set; }

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }

        /// <summary>
        ///     Biển số, chỉ sử dụng cho report
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        ///     Id công ty , chỉ sử dụng cho report
        /// </summary>
        public long CompanyId { get; set; }

        /// <summary>
        ///     Id đội xe, chỉ sử dụng cho report
        /// </summary>
        public long GroupId { get; set; }

    }

}