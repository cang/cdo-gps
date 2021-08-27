using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Geos
{
    public class GeoPolygon
    {
        public GeoLocation[] data;
        public double LatMin, LonMin;
        public double LatMax, LonMax;

        public GeoPolygon()
        {
        }

        public GeoPolygon(double[] latlonarray)
        {
            if (latlonarray.Length < 1) return;

            int len = latlonarray.Length;

            //remove end point
            if (latlonarray[0] == latlonarray[len - 2] && latlonarray[1] == latlonarray[len - 1]) len -= 2;

            data = new GeoLocation[len/2];
            for (int i = 0; i < len; i += 2)
            {
                data[i / 2].Latitude = latlonarray[i];
                data[i / 2].Longitude = latlonarray[i + 1];
            }

            RefreshBounds();
        }

        public GeoPolygon(List<double> latlonlist) : this(latlonlist.ToArray())
        {
        }

        public bool IsValid()
        {
            return data != null && data.Length > 0;
        }


        
        private void RefreshBounds()
        {
            if (!IsValid()) return;
            LatMin = double.MaxValue;
            LonMin = double.MaxValue;
            LatMax = double.MinValue;
            LonMax = double.MinValue;
            foreach (var item in data)
            {
                if (LatMin > item.Latitude) LatMin = item.Latitude;
                if (LonMin > item.Longitude) LonMin = item.Longitude;

                if (LatMax < item.Latitude) LatMax = item.Latitude;
                if (LonMax < item.Longitude) LonMax = item.Longitude;
            }
        }

        public bool Contains(double lat, double lon)
        {
            if (!IsValid()) return false;

            //check bound
            //Only for vietnam (no negative value)
            if (lon < LonMin) return false;
            if (lon > LonMax) return false;
            if (lat < LatMin) return false;
            if (lat > LatMax) return false;


            var lastPoint = data[data.Length - 1];
            var isInside = false;
            var x = lon;
            foreach (var point in data)
            {
                var x1 = lastPoint.Longitude;
                var x2 = point.Longitude;
                var dx = x2 - x1;

                if (Math.Abs(dx) > 180.0)
                {
                    // we have, most likely, just jumped the dateline (could do further validation to this effect if needed).  normalise the numbers.
                    if (x > 0)
                    {
                        while (x1 < 0)
                            x1 += 360;
                        while (x2 < 0)
                            x2 += 360;
                    }
                    else
                    {
                        while (x1 > 0)
                            x1 -= 360;
                        while (x2 > 0)
                            x2 -= 360;
                    }
                    dx = x2 - x1;
                }

                if ((x1 <= x && x2 > x) || (x1 >= x && x2 < x))
                {
                    var grad = (point.Latitude - lastPoint.Latitude) / dx;
                    var intersectAtLat = lastPoint.Latitude + ((x - x1) * grad);

                    if (intersectAtLat > lat)
                        isInside = !isInside;
                }
                lastPoint = point;
            }

            return isInside;
        }

        public bool Contains(GeoLocation location)
        {
            return Contains(location.Latitude, location.Longitude);
        }

        //public bool Contains(GeoLocation location)
        //{
        //    if (!IsValid()) return false;

        //    //check bound
        //    //Only for vietnam (no negative value)
        //    if (location.Longitude < LonMin) return false;
        //    if (location.Longitude > LonMax) return false;
        //    if (location.Latitude < LatMin) return false;
        //    if (location.Latitude > LatMax) return false;


        //    var lastPoint = data[data.Length - 1];
        //    var isInside = false;
        //    var x = location.Longitude;
        //    foreach (var point in data)
        //    {
        //        var x1 = lastPoint.Longitude;
        //        var x2 = point.Longitude;
        //        var dx = x2 - x1;

        //        if (Math.Abs(dx) > 180.0)
        //        {
        //            // we have, most likely, just jumped the dateline (could do further validation to this effect if needed).  normalise the numbers.
        //            if (x > 0)
        //            {
        //                while (x1 < 0)
        //                    x1 += 360;
        //                while (x2 < 0)
        //                    x2 += 360;
        //            }
        //            else
        //            {
        //                while (x1 > 0)
        //                    x1 -= 360;
        //                while (x2 > 0)
        //                    x2 -= 360;
        //            }
        //            dx = x2 - x1;
        //        }

        //        if ((x1 <= x && x2 > x) || (x1 >= x && x2 < x))
        //        {
        //            var grad = (point.Latitude - lastPoint.Latitude) / dx;
        //            var intersectAtLat = lastPoint.Latitude + ((x - x1) * grad);

        //            if (intersectAtLat > location.Latitude)
        //                isInside = !isInside;
        //        }
        //        lastPoint = point;
        //    }

        //    return isInside;
        //}



    }
}
