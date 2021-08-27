#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.Api
// TIME CREATE : 4:48 PM 30/10/2016
// FILENAME: VbdController.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using StarSg.Core;
using StarSg.Utils.Models.DatacenterResponse.Geo;
using Route.Api.Core;
using System.ComponentModel.Composition;
using Route.Api.Auth.Core.ConfigFile;
using System.Collections.Generic;

namespace Route.Api.Controllers
{
    /// <summary>
    /// API truy xuất thông tin vị trí địa lý
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeoController : BaseController
    {
        /// <summary>
        /// Lấy địa chỉ tọa độ
        /// </summary>
        /// <param name="lat">latitude </param>
        /// <param name="lon">longitude </param>
        /// <param name="distance">Địa chỉ trong phạm vi theo mét</param>
        /// <param name="read">chỉ đọc trong geo server nếu không có thì trả về (mặc định = false)</param>
        /// <returns></returns>
        [HttpGet]
        public GeoAddressResponse Get(float lat, float lon, float distance = 0, bool read = false)
        {
            //Lấy từ GEO Server
            try
            {
                //Tìm địa chỉ trong phạm vi distance m
                return ForwardApi.Get<GeoAddressResponse>($"{AuthConfig.GeoServerUrl}/geocode/{lat},{lon}?distance={distance}&read={read}");
            }
            catch (Exception e)
            {
                return new GeoAddressResponse() { Description = e.Message };
            }
        }

        /// <summary>
        /// Thêm địa chỉ theo lat,lon
        /// </summary>
        /// <param name="lat">latitude </param>
        /// <param name="lon">longitude </param>
        /// <param name="address">Địa chỉ cần cập nhật</param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse Add(float lat, float lon, GeoAddress address)
        {
            try
            {
                return ForwardApi.Post<BaseResponse>($"{AuthConfig.GeoServerUrl}/geocode/{lat},{lon}", address);
            }
            catch (Exception e)
            {
                return new BaseResponse() { Description = e.Message };
            }
        }

        /// <summary>
        /// Cập nhật địa chỉ theo lat,lon, nếu không có thì thêm vô
        /// </summary>
        /// <param name="lat">latitude </param>
        /// <param name="lon">longitude </param>
        /// <param name="address">Địa chỉ cần cập nhật</param>
        /// <returns></returns>
        [HttpPut]
        public BaseResponse Update(float lat, float lon, GeoAddress address)
        {
            try
            {
                return ForwardApi.Put<BaseResponse>($"{AuthConfig.GeoServerUrl}/geocode/{lat},{lon}", address);
            }
            catch (Exception e)
            {
                return new BaseResponse() { Description = e.Message };
            }
        }

        /// <summary>
        /// Xóa địa chỉ theo lat,lon và khoảng cách
        /// </summary>
        /// <param name="lat">latitude </param>
        /// <param name="lon">longitude </param>
        /// <param name="distance">Xóa tất cả các địa chỉ trong khoảng cách này (đơn vị mét)</param>
        /// <returns></returns>
        [HttpDelete]
        public BaseResponse Delete(float lat, float lon, float distance = 0)
        {
            try
            {
                return ForwardApi.Del<BaseResponse>($"{AuthConfig.GeoServerUrl}/geocode/{lat},{lon}", distance);
            }
            catch (Exception e)
            {
                return new BaseResponse() { Description = e.Message };
            }
        }


        /// <summary>
        /// GeocodeQuery
        /// </summary>
        /// <param name="distance">khoảng cách sai số</param>
        /// <param name="points">Mảng tọa độ</param>
        /// <returns></returns>
        [HttpPost]
        public GeoQueryResponse GeocodeQuery(float distance, List<GeoPostPoint> points)
        {
            try
            {
                return ForwardApi.Post<GeoQueryResponse>($"{AuthConfig.GeoServerUrl}/geocodequery?distance={distance}", points);
            }
            catch (Exception e)
            {
                return new GeoQueryResponse() { Description = e.Message };
            }
        }

        public class GeoPostPoint
        {
            public float lat { get; set; }
            public float lon { get; set; }
        }

        public class GeoQueryResponse : BaseResponse
        {
            public List<String> Data { get; set; } = new List<string>();
        }

    }
}