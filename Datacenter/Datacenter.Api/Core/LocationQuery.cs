#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : LocationQuery.cs
// Time Create : 10:56 AM 13/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Datacenter.Model.Components;
using DataCenter.Core;
using Log;
using StarSg.Core;
using System.Collections.Generic;
using StarSg.Utils.Geos;
using StarSg.Utils.Models.DatacenterResponse.Geo;
using Datacenter.Api.Models;
using System.Diagnostics;

#endregion

namespace Datacenter.Api.Core
{
   
    /// <summary>
    ///     quản lý thông tin truy vấn địa chỉ
    /// </summary>
    [Export(typeof (ILocationQuery))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LocationQuery : ILocationQuery
    {
        //private readonly string _apiName = "WhatHere";

        [Import] private Loader _loader;
        [Import] private ILog _log;

        /// <summary>
        ///     lấy thông tin địa chỉ
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public string GetAddress(float lat, float lng)
        {
            //invalid
            if (lat == 0f && lng == 0f) return "";
            if (lat < -90f || lat > 90f || lng < -180f || lng > 180f) return "";

            String ret = null;

            if (ResponseDataConfig.GeoServerUrl == null) ResponseDataConfig.GeoServerUrl  = _loader.Config.GeoCode.GeoServerUrl;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Lấy từ GEO Server
            for(int i = 0; i < 2; i++)
            {
                try
                {
                    //Tìm địa chỉ trong phạm vi ...
                    GeoAddressResponse foundaddress;
                    if (i == 0)
                        foundaddress = new ForwardApi().Get<GeoAddressResponse>($"{ResponseDataConfig.GeoServerUrl}/geocode/{lat},{lng}?distance={_loader.Config.GeoCode.AddressDistance}"
                            ,20000);
                    else
                        foundaddress = new ForwardApi().Get<GeoAddressResponse>($"{ResponseDataConfig.GeoServerUrl}/geocode/{lat},{lng}?distance=100");

                    if (foundaddress != null && foundaddress.Status > 0)
                        ret = foundaddress.Data.address;
                    break;
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    _log.Exception("LocationQuery", e, $"GEOSERVER {lat},{lng} {stopwatch.ElapsedMilliseconds}");
                    if (e.Message != null && e.Message.Contains("timed out"))
                    {
                        stopwatch.Start();
                        continue;
                    }
                }
            }

            if (!String.IsNullOrEmpty(ret))
            {
                _log.Debug("GEO", $"GEOSERVER {lat},{lng}");
                return Ensure255(ExcludeVietnam(ret));
            }

            return "";
        }

        /// <summary>
        ///     lấy thông tin địa chỉ
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public string GetAddress(GpsLocation point)
        {
            point.Address = GetAddress(point.Lat, point.Lng);
            return point.Address;
        }

        static string VNSTRING = ", Việt Nam";
        static int VNLENGH = VNSTRING.Length;

        /// <summary>
        /// Loại bõ quốc gia Vietnam trong địa chỉ 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private string ExcludeVietnam(string address)
        {
            if (String.IsNullOrWhiteSpace(address)) return address;
            if (address.EndsWith(", Vietnam"))
                return address.Substring(0,address.Length - 9);
            if (address.EndsWith(VNSTRING))
                return address.Substring(0, address.Length - VNLENGH);
            return address;
        }

        /// <summary>
        /// Chỉ giữ lại 255 kí tự cuối cùng
        /// </summary>
        private string Ensure255(string address)
        {
            if (String.IsNullOrWhiteSpace(address)) return address;
            return WithMaxLength(address,255);
        }

        private string WithMaxLength(string value, int maxLength)
        {
            if (value==null) return value;
            if (value.Length < maxLength) return value;
            return value.Substring(value.Length - maxLength, maxLength);
        }
    }

}