#region header

// /*********************************************************************************************/
// Project :Core
// FileName : DeviceTranfer.cs
// Time Create : 9:39 AM 24/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using Core.Models.Tranfer.CheckZone;

namespace StarSg.Utils.Models.Tranfer.CheckZone
{
    /// <summary>
    ///     vùng check
    /// </summary>
    public class CheckZoneTranfer
    {
        /// <summary>
        /// id vùng
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// tên vùng
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ghi chú , mô tả
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// id công ty
        /// </summary>
        public virtual long CompanyId { get; set; }

        /// <summary>
        /// Id nhóm
        /// </summary>
        public virtual long GroupId { get; set; }

        /// <summary>
        /// vân tốc tối da trong vùng
        /// </summary>
        public virtual int MaxSpeed { get; set; }

        /// <summary>
        /// số lượng thiết bị tối đa
        /// </summary>
        public virtual int MaxDevice { get; set; }

        /// <summary>
        /// danh sách các điểm tạo nên vùng >= 3 điểm
        /// </summary>
        public virtual List<Point> Points { get; set; }

        /// <summary>
        /// Địa chỉ Vùng
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// Loại khu vực
        /// </summary>
        public virtual string Type { get; set; }

    }
}