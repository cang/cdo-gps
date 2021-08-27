//#region header

//// /*********************************************************************************************/
//// Project :Datacenter.Api
//// FileName : Class1.cs
//// Time Create : 9:39 AM 25/02/2017
//// Author:  Cang Do (dovancang@gmail.com)
//// /********************************************************************************************/

//#endregion

//#region include

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;

//#endregion

//namespace Datacenter.Api.Core
//{

////http://api.vietbando.com/Help_V2/Default.html#definedfunction/TransportTypeSpeed
////0: không xác định.
////1: Xe ô tô khách.
////2: Xe ô tô tải.
////3: Xe mô tô.
////4: Xe buýt.
////5: Xe rơ moóc-container.
////6: Xe chuyên dùng.
////7: Xe gắn máy.
////8: Xe kéo.
////9: Xe ô tô khách hơn 9 chỗ.
////10: Xe ô tô khách hơn 30 chỗ.
////11: Xe ô tô tải trên 3,5 tấn.
////12: Xe ô tô khách giường nằm 2 tầng.
////13: Xe máy chuyên dụng. Là các xe máy thi công, xe máy nông nghiệp, lâm nghiệp và các loại xe đặc chủng khác sử dụng vào mục đích quốc phòng, an ninh có tham gia giao thông đường bộ.


//    /// <summary>
//    ///     0: không xác định.
//    ///     1: Xe ô tô khách.
//    ///     2: Xe ô tô tải.
//    ///     3: Xe mô tô.
//    ///     4: Xe buýt.
//    ///     5: Xe rơ moóc-container.
//    ///     6: Xe chuyên dùng.
//    ///     7: Xe gắn máy.
//    ///     8: Xe kéo.
//    ///     9: Xe ô tô khách hơn 9 chỗ.
//    ///     10: Xe ô tô khách hơn 30 chỗ.
//    ///     11: Xe ô tô tải trên 3,5 tấn.
//    /// </summary>
//    public enum TransportType
//    {
//        None,
//        OtoKhach,
//        Ototai,
//        Moto,
//        Buyt,
//        Romooc,
//        ChuyenDung,
//        Ganmay,
//        OtoKhach9Cho,
//        OtoKhach30Cho,
//        OtoTai3Tan
//    }


//    [DataContract]
//    public class Coord
//    {
//        [DataMember(Name = "x")]
//        public double X { get; set; }

//        [DataMember(Name = "y")]
//        public double Y { get; set; }
//    }

//    [DataContract]
//    public class CoordTracking
//    {
//        [DataMember(Name = "Latitude")]
//        public double Latitude { get; set; }

//        [DataMember(Name = "Longitude")]
//        public double Longitude { get; set; }
//    }

//    [DataContract]
//    public class VbdBatchGeoCode
//    {
//        [DataMember(Name = "Coors")]
//        public IList<Coord> Coord { get; set; }

//        [DataMember(Name = "Radius")]
//        public int Radius { get; set; }
//    }

//    [DataContract]
//    public class VbdTrackingRequest
//    {
//        [DataMember]
//        public CoordTracking CurPoint { get; set; }

//        [DataMember]
//        public CoordTracking PrePoint { get; set; }

//        [DataMember]
//        public int Radius { get; set; }

//        [DataMember]
//        public int TransportType { get; set; }
//    }

//    [DataContract]
//    public class VbdTrackingReponse
//    {
//        [DataMember]
//        public VbdError Error { get; set; }

//        [DataMember]
//        public bool IsSuccess { get; set; }

//        [DataMember]
//        public DateTime ReponseTime { get; set; }

//        [DataMember]
//        public VbdTrackingValue Value { get; set; }
//    }

//    [DataContract]
//    public class VbdTrackingValue
//    {
//        [DataMember]
//        public string DistrictId { get; set; }

//        [DataMember]
//        public string DistrictName { get; set; }

//        [DataMember]
//        public int MaxSpeed { get; set; }

//        [DataMember]
//        public int MinSpeed { get; set; }

//        [DataMember]
//        public string ProvinceId { get; set; }

//        [DataMember]
//        public string ProvinceName { get; set; }

//        [DataMember]
//        public double SnapX { get; set; }

//        [DataMember]
//        public double SnapY { get; set; }

//        [DataMember]
//        public string Street { get; set; }

//        [DataMember]
//        public string WardId { get; set; }

//        [DataMember]
//        public string WardName { get; set; }
//    }

//    public class VbdError
//    {
//        public string ExceptionType { get; set; }
//        public string Message { get; set; }
//    }

//    [DataContract]
//    public class VbdGeoCode
//    {
//        [DataMember(Name = "X")]
//        public double X { get; set; }

//        [DataMember(Name = "Y")]
//        public double Y { get; set; }

//        [DataMember(Name = "Radius")]
//        public int Radius { get; set; }
//    }

//    [DataContract]
//    public class VbdResponseBatchRevertGeoCode
//    {
//        [DataMember]
//        public string[] District { get; set; }

//        [DataMember]
//        public string[] DistrictID { get; set; }

//        [DataMember]
//        public string[] Province { get; set; }

//        [DataMember]
//        public string[] ProvinceID { get; set; }

//        [DataMember]
//        public string[] Street { get; set; }

//        [DataMember]
//        public string[] Ward { get; set; }

//        [DataMember]
//        public string[] WardID { get; set; }

//        [DataMember]
//        public string[] Number { get; set; }

//        #region Overrides of Object

//        /// <summary>
//        ///     Returns a string that represents the current object.
//        /// </summary>
//        /// <returns>
//        ///     A string that represents the current object.
//        /// </returns>
//        public override string ToString()
//        {
//            if (string.IsNullOrEmpty(Province[0])) return "";
//            return $"{Number[0]} {Street[0]} ,{Ward[0]} ,{District[0]}, {Province[0]} ";
//        }

//        public IEnumerable<string> GetAddress()
//        {
//            for (var i = 0; i < Province.Length; i++)
//            {
//                yield return $"{Number[0]} {Street[0]} ,{Ward[0]} ,{District[0]}, {Province[0]} ";
//            }
//        }

//        #endregion
//    }

//    [DataContract]
//    public class VbdResponseRevertGeoCode
//    {
//        [DataMember]
//        public string District { get; set; }

//        [DataMember]
//        public string DistrictID { get; set; }

//        [DataMember]
//        public string Province { get; set; }

//        [DataMember]
//        public string ProvinceID { get; set; }

//        [DataMember]
//        public string Street { get; set; }

//        [DataMember]
//        public string Ward { get; set; }

//        [DataMember]
//        public string WardID { get; set; }

//        [DataMember]
//        public string Number { get; set; }

//        #region Overrides of Object

//        /// <summary>
//        ///     Returns a string that represents the current object.
//        /// </summary>
//        /// <returns>
//        ///     A string that represents the current object.
//        /// </returns>
//        public override string ToString()
//        {
//            if (string.IsNullOrEmpty(Province)) return "";
//            return $"{Number} {Street} ,{Ward} ,{District}, {Province} ";
//        }

//        #endregion
//    }


//    public static class VbdServices
//    {
//        public static string GeoCodeHost { get; set; } = "";
//        public static string RegisterKey { get; set; } = "";


//        ///// <summary>
//        /////     chuyển đổi tọa độ sang địa chỉ
//        ///// </summary>
//        ///// <param name="lat"></param>
//        ///// <param name="lng"></param>
//        ///// <returns></returns>
//        //public static string RevertGeoCode(double lat, double lng)
//        //{
//        //    try
//        //    {
//        //        //var wr = WebRequest.Create("http://developers.vietbando.com/V2/Service/PartnerPortalservice.svc/rest/ReverseGeoCodingWithFullAddress");
//        //        var wr = WebRequest.Create($"{GeoCodeHost}ReverseGeoCodingWithFullAddress");

//        //        wr.Method = "POST";
//        //        wr.Headers.Add("RegisterKey", RegisterKey);
//        //        wr.ContentType = "application/json";
//        //        var serializer = new DataContractJsonSerializer(typeof(VbdGeoCode));
//        //        serializer.WriteObject(wr.GetRequestStream(),
//        //            new VbdGeoCode {Radius = 10, Y = lat, X = lng});

//        //        var response = wr.GetResponse();
//        //        serializer = new DataContractJsonSerializer(typeof(VbdResponseRevertGeoCode));
//        //        var result = (VbdResponseRevertGeoCode) serializer.ReadObject(response.GetResponseStream());
//        //        response.Dispose();

//        //        return result.ToString();
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine($"Lôi RevertGeoCode {lat},{lng} ");
//        //        throw;
//        //    }
//        //    return null;
//        //}

//        ///// <summary>
//        /////     chuyển đổi tọa độ sang địa chỉ số lượng lớn
//        ///// </summary>
//        ///// <param name="datas"></param>
//        ///// <returns></returns>
//        //public static IEnumerable<string> RevertBatchGeoCode(IList<Tuple<double, double>> datas)
//        //{
//        //    try
//        //    {
//        //        //var wr = WebRequest.Create("http://developers.vietbando.com/V2/Service/PartnerPortalservice.svc/rest/BatchReverseGeoCodingWithFullAddress");
//        //        var wr = WebRequest.Create($"{GeoCodeHost}BatchReverseGeoCodingWithFullAddress");
//        //        wr.Method = "POST";
//        //        wr.Headers.Add("RegisterKey", RegisterKey);
//        //        wr.ContentType = "application/json";
//        //        var serializer = new DataContractJsonSerializer(typeof(VbdBatchGeoCode));
//        //        serializer.WriteObject(wr.GetRequestStream(),
//        //            new VbdBatchGeoCode
//        //            {
//        //                Radius = 10,
//        //                Coord = datas.Select(m =>
//        //                        new Coord {Y = m.Item1, X = m.Item2}).ToList()
//        //            });

//        //        var response = wr.GetResponse();
//        //        serializer = new DataContractJsonSerializer(typeof(VbdResponseBatchRevertGeoCode));

//        //        var result = (VbdResponseBatchRevertGeoCode) serializer.ReadObject(response.GetResponseStream());
//        //        response.Dispose();

//        //        return result?.GetAddress();
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw;
//        //    }
//        //    return null;
//        //}

//        public static int GetSpeedOfStreet(double lat, double lng, double lat1, double lng1)
//        {
//            try
//            {
//                //var wr = WebRequest.Create("http://developers.vietbando.com/V2/Service/PartnerPortalservice.svc/rest/GetTrackingInformation");
//                var wr = WebRequest.Create($"{GeoCodeHost}GetTrackingInformation");

//                wr.Method = "POST";
//                wr.Headers.Add("RegisterKey", RegisterKey);
//                wr.ContentType = "application/json";
//                var serializer = new DataContractJsonSerializer(typeof(VbdTrackingRequest));
//                serializer.WriteObject(wr.GetRequestStream(),
//                    new VbdTrackingRequest()
//                    {
//                        CurPoint = new CoordTracking() {Longitude = lng, Latitude = lat},
//                        PrePoint = new CoordTracking() {Longitude = lng1, Latitude = lat1},
//                        Radius = 50,
//                        TransportType = (int) TransportType.OtoKhach9Cho
//                    });

//                var response = wr.GetResponse();
//                serializer = new DataContractJsonSerializer(typeof(VbdTrackingReponse));

//                var result = (VbdTrackingReponse) serializer.ReadObject(response.GetResponseStream());
//                response.Dispose();

//                return result?.IsSuccess == true ? result.Value.MaxSpeed : -1;
//            }
//            catch
//            {
//                throw;
//            }

//            return -1;
//        }



//    }
//}