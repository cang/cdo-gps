//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace StarSg.Utils.Geos
//{
//    public class GeoLocationAddress
//    {
//        int limit;
//        ConcurrentQueue<LocationAddress> locs = new ConcurrentQueue<LocationAddress>();

//        public GeoLocationAddress() : this(0)
//        {
//        }
//        public GeoLocationAddress(int limit)
//        {
//            this.limit = limit;
//        }

//        /// <summary>
//        /// Find Address by lat,lng and in limit distance
//        /// </summary>
//        /// <param name="lat"></param>
//        /// <param name="lng"></param>
//        /// <param name="limitdistance"></param>
//        /// <returns>null if not found</returns>
//        public String Find(float lat,float lng,double limitdistance)
//        {
//            foreach (LocationAddress loc in locs)
//            {
//                if( loc.Distance(lat,lng) <= limitdistance)
//                {
//                    return loc.Address;
//                }
//            }
//            return null;
//        }

//        /// <summary>
//        /// Find Address by lat,lng
//        /// </summary>
//        /// <param name="lat"></param>
//        /// <param name="lng"></param>
//        /// <returns>null if not found</returns>
//        public String Find(float lat, float lng)
//        {
//            return Find(lat, lng,0);
//        }

//        public void Add(float lat, float lng, String address)
//        {
//            locs.Enqueue(new LocationAddress() {
//                Latitude = lat,
//                Longitude = lng,
//                Address = address
//            });

//            LocationAddress ret;
//            while(locs.Count>limit)
//            {
//                locs.TryDequeue(out ret);
//                ret = null;
//            }
//        }


//        public class LocationAddress : IComparable<LocationAddress>
//        {
//            public float Latitude;
//            public float Longitude;
//            public String Address;

//            public int CompareTo(LocationAddress other)
//            {
//                if (Latitude == other.Latitude && Longitude == other.Longitude) return 0;
//                if (Latitude < other.Latitude
//                    || (Latitude == other.Latitude && Longitude < other.Longitude))
//                    return -1;
//                return 1;
//            }

//            public double Distance(LocationAddress to)
//            {
//                return GeoUtil.Distance(Latitude, Longitude, to.Latitude, to.Longitude);
//            }

//            public double Distance(float lat,float lng)
//            {
//                return GeoUtil.Distance(Latitude, Longitude, lat, lng);
//            }

//        }

//    }


//}
