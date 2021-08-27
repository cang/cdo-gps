using System.Collections.Generic;
using System.Linq;

namespace Route.Api.Auth.Models.Req
{
    /// <summary>
    /// chi nhánh
    /// </summary>
    public class BranchTranfer
    {
        /// <summary>
        /// Mã Chi Nhánh lấy làm hostname luôn
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tên Chi Nhánh
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Logo của chi nhánh
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Số dt support
        /// </summary>
        /// 
        public string SupportPhoneNumber { get; set; }

        /// <summary>
        /// Số dt report
        /// </summary>
        public string ReportPhoneNumber { get; set; }

        /// <summary>
        /// Link web site
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// Dùng để lưu dữ liệu động bất kì ( vd json ... , hoặc text format)
        /// </summary>
        public string Reserve { get; set; }


        /// <summary>
        /// Header
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Footer
        /// </summary>
        public string Footer { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// LongName
        /// </summary>
        public string LongName { get; set; }

    }
}