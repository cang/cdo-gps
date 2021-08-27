#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : HistoryDeviceLogic.cs
// Time Create : 11:06 AM 28/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using DevicePacketModels;
using FluentNHibernate.Utils;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    /// 
    /// </summary>
    [Sort(5)]
    public class HistoryDeviceLogic : ILogic
    {
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            //var dr = uTils.DataCache.GetQueryContext<Driver>().GetByKey(device.Status.DriverStatus.DriverId);
            var log = new DeviceLog
            {
                DbId = company.DbId,
                CompanyId = company.Id,
                //DriverId = dr?.Id ?? 0,
                GroupId = device.GroupId,
                Indentity = device.Indentity,
                Serial = device.Serial,
                DeviceStatus =new DeviceStatusInfo()
                {
                    AirMachine = device.Status.BasicStatus.AirMachine,
                    Angle = device.Status.BasicStatus.Angle,
                    ClientSend = device.Status.BasicStatus.ClientSend,
                    Door = device.Status.BasicStatus.Door,
                    Fuel = device.Status.BasicStatus.Fuel,
                    GpsInfo = new GpsLocation()
                    {
                        Lat = device.Status.BasicStatus.GpsInfo?.Lat??0,
                        Lng = device.Status.BasicStatus.GpsInfo?.Lng??0,
                        Address = device.Status.BasicStatus.GpsInfo?.Address
                    },
                    GpsStatus = device.Status.BasicStatus.GpsStatus,
                    GsmSignal = device.Status.BasicStatus.GsmSignal,
                    Machine = device.Status.BasicStatus.Machine,
                    Power = device.Status.BasicStatus.Power,
                    ServerRecv = device.Status.BasicStatus.ServerRecv,
                    SpeedTrace = device.Status.BasicStatus.SpeedTrace,
                    Sos = device.Status.BasicStatus.Sos,
                    Speed = device.Status.BasicStatus.Speed,
                    UseFuel = device.Status.BasicStatus.UseFuel,
                    UseRfid = device.Status.BasicStatus.UseRfid,
                    UseTemperature = device.Status.BasicStatus.UseTemperature,
                    TotalCurrentGpsDistance = device.Status.BasicStatus.TotalCurrentGpsDistance,
                    TotalGpsDistance = device.Status.BasicStatus.TotalGpsDistance,
                    Temperature = device.Status.BasicStatus.Temperature
                }, //device.Status.BasicStatus.DeepClone(),
                DriverStatus = new DriverStatusInfo()
                {
                    DriverId = device.Status.DriverStatus.DriverId,
                    Gplx = "",
                    Name = "",
                    OverSpeedCount = device.Status.DriverStatus.OverSpeedCount,
                    TimeBeginWorkInSession = device.Status.DriverStatus.TimeBeginWorkInSession,
                    TimeWorkInDay = device.Status.DriverStatus.TimeWorkInDay
                },//device.Status.DriverStatus.DeepClone(),
                Id = 0
            };

            uTils.CacheLog.PushHistoryDevice(log);
        }
    }
}