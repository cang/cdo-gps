using Datacenter.Model.Entity;
using StarSg.Utils.Geos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCenter.Core
{
    public class RouteGpsLogic
    {
        static readonly char[] SPLITTOKENS = new char[] { ',', ' ' };

        public RouteGps Data;

        //runtime data
        protected GeoPolygon Polygon;
        public byte[] MaxSpeeds;
        public byte[] MinSpeeds;

        public RouteGpsLogic(RouteGps data)
        {
            this.Data = data;
            Preprocess();
        }


        /// <summary>
        /// Kiểm tra điểm có nằm trong đường này không
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public bool Contains(float lat, float lon)
        {
            if (Polygon == null) return false;
            return Polygon.Contains(lat, lon);
        }


        /// <summary>
        /// Lấy vận tốc tối đa của đường này theo loại xe
        /// </summary>
        /// <param name="transportType">loại xe</param>
        /// <returns>255 nếu không có</returns>
        public byte GetMaxSpeed(int transportType)
        {
            if (MaxSpeeds != null && MaxSpeeds.Length >transportType)
                return MaxSpeeds[(int)transportType];
            return byte.MaxValue;
        }

        /// <summary>
        /// Lấy vận tốc tối thiểu của đường này theo loại xe
        /// </summary>
        /// <param name="transportType"></param>
        /// <returns>0 nếu không có</returns>
        public byte GetMinSpeed(int transportType)
        {
            if (MinSpeeds != null && MinSpeeds.Length > transportType)
                return MinSpeeds[(int)transportType];
            return 0;
        }

        /// <summary>
        /// Xử lý dữ liệu từ modal
        /// </summary>
        private void Preprocess()
        {
            if (String.IsNullOrWhiteSpace(Data.DataPath)) return;
            if (String.IsNullOrWhiteSpace(Data.MaxSpeed)) return;
            if (String.IsNullOrWhiteSpace(Data.MinSpeed)) return;

            String[] sDataPath = Data.DataPath.Split(SPLITTOKENS, StringSplitOptions.RemoveEmptyEntries);
            double[] fDataPath = new double[sDataPath.Length];
            for (int i = sDataPath.Length - 1; i >= 0; i--)
                fDataPath[i] = double.Parse(sDataPath[i]);
            Polygon = new GeoPolygon(fDataPath);

            String[] sMaxSpeed = Data.MaxSpeed.Split(SPLITTOKENS, StringSplitOptions.RemoveEmptyEntries);
            MaxSpeeds = new byte[sMaxSpeed.Length];
            for (int i = sMaxSpeed.Length - 1; i >= 0; i--)
                MaxSpeeds[i] = byte.Parse(sMaxSpeed[i]);

            String[] sMinSpeed = Data.MinSpeed.Split(SPLITTOKENS, StringSplitOptions.RemoveEmptyEntries);
            MinSpeeds = new byte[sMinSpeed.Length];
            for (int i = sMinSpeed.Length - 1; i >= 0; i--)
                MinSpeeds[i] = byte.Parse(sMinSpeed[i]);
        }

    }
}