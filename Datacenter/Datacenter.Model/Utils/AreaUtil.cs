#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : AreaUtil.cs
// Time Create : 8:09 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;

namespace Datacenter.Model.Utils
{
    public static class AreaUtil
    {
        /// <summary>
        ///     từ danh sách điểm tọa độ chuyển sang dạng data lưu vào trong database
        ///     Trường tọa độ ta sẽ lưu thành 1 chuỗi dạng lat;lng|lat;lng….khi lưu vào dữ liệu. Sau khi đọc ra ta mới
        ///     phân tích thành 1 mảng đối tượng Points.
        /// </summary>
        /// <param name="pointlList">danh sách điểm tọa độ</param>
        /// <returns></returns>
        public static string PointListToString(this IList<GpsLocation> pointlList)
        {
            try
            {
                var forward = string.Join("|", pointlList.Select(p => p.Lat + ";" + p.Lng));
                return forward;
            }
            catch (Exception)
            {
                // có lỗi khi xử lý thành data
            }
            return "";
        }

        /// <summary>
        ///     từ database chuyển thành danh sách điểm tọa độ
        /// </summary>
        /// <param name="pointlListData">data chứa trong database</param>
        /// <returns></returns>
        public static List<GpsLocation> StringToPointList(this string pointlListData)
        {
            var points = new List<GpsLocation>();
            try
            {
                var strpointarray = pointlListData.Split('|');
                points.AddRange(strpointarray.Select(item => item.Split(';')).Select(newitem => new GpsLocation
                {
                    Lat = float.Parse(newitem[0]),
                    Lng = float.Parse(newitem[1])
                }));
            }
            catch (Exception)
            {
                // có lỗi thì parser data thành list point
            }
            return points;
        }
        public static bool Contain(this Area area, GpsLocation p)
        {
            var polygon = area?.Points.StringToPointList();
            if (polygon == null || polygon.Count < 3)
                return false;

            double minX = polygon[0].Lat;
            double maxX = polygon[0].Lat;
            double minY = polygon[0].Lng;
            double maxY = polygon[0].Lng;
            for (var i = 1; i < polygon.Count; i++)
            {
                var q = polygon[i];
                minX = Math.Min(q.Lat, minX);
                maxX = Math.Max(q.Lat, maxX);
                minY = Math.Min(q.Lng, minY);
                maxY = Math.Max(q.Lng, maxY);
            }

            if (p.Lat < minX || p.Lat > maxX || p.Lng < minY
                || p.Lng > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            var inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if (polygon[i].Lng > p.Lng != polygon[j].Lng > p.Lng
                    && p.Lat
                    < (polygon[j].Lat - polygon[i].Lat) * (p.Lng - polygon[i].Lng)
                    / (polygon[j].Lng - polygon[i].Lng) + polygon[i].Lat)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}