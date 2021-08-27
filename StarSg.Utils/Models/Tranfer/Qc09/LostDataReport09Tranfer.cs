using System;
using Core.Models;

namespace StarSg.Utils.Models.Tranfer.Qc09
{
    /// <summary>
    /// mất data truyền data lên bộ
    /// </summary>
    public class LostDataReport09Tranfer
    {
        public long Serial { get; set; }
        public string Bs { get; set; }
        public long CompanyId { get; set; }
        public int Type { get; set; }

        public int KhongTruyenDataCount { get; set; }
        public int TuanSuatDataCount { get; set; }
        public int TruyenThieuDataCount { get; set; }

        public TimeSpan TimeTotal { get; set; }
        public DateTime TimeUpdate { get; set; }

        public string CompanyTool { get; set; }
        public string Note { get; set; }
    }
}
