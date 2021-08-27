using Core.Models.Tranfer;

namespace StarSg.Utils.Models.Tranfer
{
    public class CompanyTranfer
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GpsPoint Location { get; set; }
        public string ShortName { get; set; }
        public int DbId { get; set; }
        public string BranchCode { get; set; }

        /// <summary>
        /// 0 mac dinh, 1 : xe dien
        /// </summary>
        public byte Type { get; set; }
    }
}