using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Geos
{
    public struct GeoLocation
    {
        public double Latitude;
        public double Longitude;
        public double Distance(GeoLocation to)
        {
            return GeoUtil.Distance(Latitude, Longitude, to.Latitude, to.Longitude);
        }
    }

}
