using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Models.DatacenterResponse.Geo
{
    public class GeoAddress
    {
        public string address { set; get; }
        public string number { set; get; }
        public string street { set; get; }
        public string ward { set; get; }
        public string district { set; get; }
        public string city { set; get; }
        public string province { set; get; }
        public int postalcode { set; get; }
        public string country { set; get; }
    }
}
