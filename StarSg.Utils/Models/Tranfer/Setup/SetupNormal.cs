using System.Collections.Generic;

namespace StarSg.Utils.Models.Tranfer.Setup
{
    public class SetupNormal:BaseSetup
    {
        public short TimeSync { get; set; }
        public short OverTimeInSession { get; set; }
        public short OverTimeInDay { get; set; }
        public byte OverSpeed { get; set; }
        public IList<string> PhoneSystemControl { get; set; } = new List<string>();
        public string Bs { get; set; }
    }
}