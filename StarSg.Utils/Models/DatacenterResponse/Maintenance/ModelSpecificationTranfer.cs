#region header
// /*********************************************************************************************/
// Project :StarSg.Utils
// FileName : ModelSpecificationTranfer.cs
// Time Create : 8:27 AM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion
namespace StarSg.Utils.Models.DatacenterResponse.Maintenance
{
    public class ModelSpecificationTranfer
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     Số km đảo lốp
        /// </summary>
        public virtual long KmDaoLop { get; set; }

        /// <summary>
        ///     Số km thay vỏ
        /// </summary>
        public virtual long KmThayVo { get; set; }

        /// <summary>
        ///     số km thay lọc nhớt
        /// </summary>
        public virtual long KmThayNhot { get; set; }

        /// <summary>
        ///     số km thay lọc dầu
        /// </summary>
        public virtual long KmThayLocDau { get; set; }

        /// <summary>
        ///     số km thay lọc gió
        /// </summary>
        public virtual long KmThayLocGio { get; set; }

        /// <summary>
        ///     số km thay lọc nhớt
        /// </summary>
        public virtual long KmThayLocNhot { get; set; }

    }
}