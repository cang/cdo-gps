#region header
// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DeviceTraceLogic.cs
// Time Create : 11:16 AM 18/01/2017
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using DevicePacketModels;

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    [Sort(5)]
    public class DeviceTraceLogic:ILogic
    {
        #region Implementation of ILogic

        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            ////if (device.Temp.TraceMachineId > 0)
            //if(device.Temp.GetTrace(TraceType.Machine)!=null)
            //{
            //    ////todo: phục vụ cho báo cáo 10: 'Cập nhật xe chạy' sau mỗi 30 giây đối phó bộ 
            //    //if ((device.Status.BasicStatus.ClientSend-device.Temp.TimeHandleRun).TotalSeconds >= 30) // 5 minutes
            //    //{
            //    //    device.Temp.TimeHandleRun = DateTime.Now;
            //    //    uTils.LocationQuery.GetAddress(device.Status.BasicStatus.GpsInfo);//Todo : Note VBD
            //    //    var trace = new DeviceTraceLog
            //    //    {
            //    //        CompanyId = device.CompanyId,
            //    //        Serial = device.Serial,
            //    //        Indentity = device.Indentity,
            //    //        BeginLocation = device.Status.BasicStatus.GpsInfo,
            //    //        BeginTime = DateTime.Now,
            //    //        DbId = 0,
            //    //        DriverId = 0,
            //    //        GroupId = device.GroupId,
            //    //        Type = TraceType.Run5,
            //    //        Distance = 0,
            //    //        EndLocation = device.Status.BasicStatus.GpsInfo,
            //    //        DriverTime = DateTime.Now,
            //    //        EndTime = DateTime.Now,
            //    //        Note = "Xe chạy 5 phút"
            //    //    };
            //    //    uTils.DataContext.Insert(trace, 0);
            //    //    uTils.DataContext.Commit();
            //    //}
            //}
        }

        #endregion
    }
}