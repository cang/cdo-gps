#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : BaseHandle.cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using CorePacket;
using DevicePacketModels;
using DevicePacketModels.Events;
using DevicePacketModels.Setups;
using Log;
using Route.Core;
using ServerCore;
using StarSg.Core;
using System.Collections.Generic;

#endregion

namespace Route.DeviceServer.Handles
{
    public class BaseHandle
    {
        [Import] protected IClientCachePacket CachePacket;

        [Import]
        protected IDeviceRouteTable DevieceRoute { get; set; }

        [Import]
        protected ILog Log { get; set; }


        protected DataCenterInfo GetDatacenter(long serial)
        {
            return DevieceRoute.GetDataCenter(serial);
        }

        protected bool ForwardEvent(HttpMethod method, long serial, string url, object data = null)
        {
            var fw = new ForwardApi();
            var datacenter = GetDatacenter(serial);
            fw.AddHeader("serial", serial.ToString());
            if (datacenter == null) return false;
            var result = false;
            try
            {
                switch (method)
                {
                    case HttpMethod.GET:
                        result = fw.Get<bool>($"{url}", data);
                        break;
                    case HttpMethod.POST:
                        result = fw.Post<bool>($"{url}", data);
                        break;
                    case HttpMethod.PUT:
                        result = fw.Put<bool>($"{url}", data);
                        break;
                    case HttpMethod.DELETE:
                        result = fw.Del<bool>($"{url}", data);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, $"Chuyển dữ liệu qua center lỗi serial {serial} , url : {url} ");
            }
            return result;
        }

        protected bool ForwardSysn(long serial, P01SyncPacket data)
        {
            try
            {
                var fw = new ForwardApi();
                fw.AddHeader("serial", data.Serial.ToString());
                var datacenter = GetDatacenter(serial);
                if (datacenter == null) return false;
                var url = $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/Sync";
                Log.Debug("PACKET", $"URL request sync : {url}");
                var result = fw.Post<byte[]>(url, data);

                if (result == null || result.Length == 0) return true;
                CachePacket.Push(serial, result);
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception("PACKET", ex, $"forward sync packet fail {data.Serial}");
                throw;
            }
        }

        protected bool ForwardSysn301(long serial, P01SyncPacket data,bool last = false)
        {
            try
            {
                var fw = new ForwardApi();
                fw.AddHeader("serial", data.Serial.ToString());
                var datacenter = GetDatacenter(serial);
                if (datacenter == null) return false;
                String url;
                if(last) url = $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/Sync301last";
                else url = $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/Sync301";
                Log.Debug("PACKET", $"URL request sync : {url}");
                //var result = 
                    fw.Post<byte[]>(url, data);
                //if (result == null || result.Length == 0) return true;
                //CachePacket.Push(serial, result);
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception("PACKET", ex, $"forward sync301 packet fail {data.Serial}");
                throw;
            }
        }

        protected bool ForwardSysn10(long serial, P10SyncPacket data)
        {
            try
            {
                var fw = new ForwardApi();
                fw.AddHeader("serial", data.Serial.ToString());
                var datacenter = GetDatacenter(serial);
                if (datacenter == null) return false;
                var url = $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/Sync10";
                Log.Debug("PACKET", $"URL request sync : {url}");
                var result = fw.Post<byte[]>(url, data);

                if (result == null || result.Length == 0) return true;
                CachePacket.Push(serial, result);
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception("PACKET", ex, $"forward sync packet fail {data.Serial}");
                throw;
            }
        }

        protected bool ForwardEndOverTime(long serial, P114EndOvertime p)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/EndOverTime", p);
        }
        protected bool ForwardOffMachine(long serial, P101OffMachine packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/OffMachine", packet);
        }
        protected bool ForwardOnMachine(long serial, P100OnMachine packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/OnMachine", packet);
        }
        protected bool ForwardOnAirCondition(long serial, P102OnAirCondition packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/OnAirCondition", packet);
        }

        protected bool ForwardOffAirCondition(long serial, P103OffAirCondition packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/OffAirCondition", packet);
        }

        protected bool ForwardOpenDoor(long serial, P104OpenDoor packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/OpenDoor", packet);
        }

        protected bool ForwardCloseDoor(long serial, P105CloseDoor packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/CloseDoor", packet);
        }


        protected bool ForwardBeginOverSpeed(long serial, P106BeginOverSpeed packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/BeginOverSpeed", packet);
        }


        protected bool ForwardEndOverSpeed(long serial, P107EndOverSpeed packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/EndOverSpeed", packet);
        }

        protected bool ForwardBeginStop(long serial, P108BeginStop packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/BeginStop", packet);
        }

        protected bool ForwardEndStop(long serial, P109EndStop packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/EndStop", packet);
        }


        protected bool ForwardDeviceReset(long serial, P110DeviceReset packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/DeviceReset", packet);
        }

        protected bool ForwardChangeDriver(long serial, P111ChangeDriver packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/ChangeDriver", packet);
        }

        protected bool ForwardResetDriverTimeWork(long serial, P112ResetDriverTimeWork packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/ResetTimeWork", packet);
        }

        protected bool ForwardChangeSim(long serial, P113ChangeSim packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/ChangeSim", packet);
        }

        protected bool ForwardDeviceInfo(long serial, P205DeviceInfo packet)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/DeviceInfo", packet);
        }

        protected bool ForwardBeginGuest(long serial, P115BeginGuest p)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/BeginGuest", p);
        }

        protected bool ForwardEndGuest(long serial, P116EndGuest p)
        {
            var datacenter = GetDatacenter(serial);
            if (datacenter == null) return false;
            return ForwardEvent(HttpMethod.POST, serial,
                $"{datacenter.Ip}:{datacenter.Port}/api/DevicePacketHandle/EndGuest", p);
        }

    }
}