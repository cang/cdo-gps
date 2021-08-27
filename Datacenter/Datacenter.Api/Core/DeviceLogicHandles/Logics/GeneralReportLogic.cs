#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Api
// TIME CREATE : 10:14 PM 18/12/2016
// FILENAME: GeneralReportLogic.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using System.Linq;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using DevicePacketModels;
using StarSg.Utils.Models.Tranfer;
using Datacenter.Model.Components;

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    /// Kiểm tra và cập nhật GeneralReportLog theo gói sync vô biến tạm device.Temp.GeneralReportLog
    /// </summary>
    [Sort(6)]
    public class GeneralReportLogic:ILogic
    {
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            try
            {
                if (device.Temp.DeviceLostGsmLog != null
                    && uTils.PacketType ==0)
                    HandleWakeupLostDevice(packet, uTils, device, company);

                long km = 0;
                if (device.Status != null && device.Status.BasicStatus!=null)
                    km = device.Status.BasicStatus.TotalGpsDistance - device.Status.LastTotalKmUsingOnDay;//km = device.Status.BasicStatus.TotalGpsDistance - device.Status.TotalKmBeginDate;

                //Nếu đang có sự kiện xảy ra thì Giá trị dầu đầu tính cho báo cáo tổng hợp là giá trị trước khi xảy ra sự kiện
                float fuel0 = device.Status.BasicStatus.Fuel;
                if (device.Temp.FuelTest != null && device.Temp.FuelTest.FuelOfBeginingEvent >= 0 
                    && device.DeviceType != DeviceType.Dynamo) //khong xet MAY PHAT DIEN
                    fuel0 = device.Temp.FuelTest.FuelOfBeginingEvent;

                lock (device.Temp.GeneralReportLogLock)
                {
                    //tạo mới cho gói tin đầu ngày
                    if (device.Temp.GeneralReportLog == null)
                    {

                        device.Temp.GeneralReportLog = new GeneralReportLog
                        {
                            UpdateTime = DateTime.Now.Date,
                            GuidId = device.Indentity,
                            CompanyId = device.CompanyId,
                            Id = 0,
                            DbId = company.DbId,
                            BeginDateFuel = fuel0,
                            RemainFuel = fuel0,
                            OverTimeInday = 0,
                            GroupId = device.GroupId
                            //,
                            //UseGuest = device.Temp?.HasGuestSensor??false,
                            //GuestTimeInday =0,
                            //KmGuestOnDay = 0
                        };
                    }

                    //kiểm tra đúng ngày cập nhật là hôm nay, cập nhật data mới
                    if (device.Temp.GeneralReportLog.UpdateTime == DateTime.Now.Date)
                    {
                        if (device.Temp.GeneralReportLog.BeginDateFuel == 0 && fuel0 > 0)
                            device.Temp.GeneralReportLog.BeginDateFuel = fuel0;

                        device.Temp.GeneralReportLog.KmOnDay = km > 0 ? km : 0;

                        device.Temp.GeneralReportLog.OpenDoorCount = device.Temp.OpenDoorCount;
                        device.Temp.GeneralReportLog.PauseCount = device.Temp.PauseCount;
                        device.Temp.GeneralReportLog.OnAirMachineCount = device.Temp.AirConditionCount;

                        device.Temp.GeneralReportLog.InvalidSpeedCount = device.Temp.OverSpeedCount;

                        //lỗi packet.TimeWorkInDay = 0 từ thiết bị
                        //if (packet.TimeWorkInDay > 0)
                        //device.Temp.GeneralReportLog.OverTimeInday = packet.TimeWorkInDay;
                        if (device.Status.DriverStatus.TimeWorkInDay>0)
                            device.Temp.GeneralReportLog.OverTimeInday = device.Status.DriverStatus.TimeWorkInDay;

                        device.Temp.GeneralReportLog.InvalidOverTimeCount = device.Temp.OverTime4HCount;

                        //device.Temp.GeneralReportLog.OverTimeIndayCount = packet.TimeWorkInDay / 600;
                        device.Temp.GeneralReportLog.OverTimeIndayCount = device.Temp.GeneralReportLog.OverTimeInday / 600;

                        device.Temp.GeneralReportLog.RemainFuel = fuel0;
                    }
                }
            }
            catch (Exception e)
            {
                uTils.Log.Exception("GeneralReportLogic", e, "Lỗi hàm x data");
            }

            #region Xử lý báo cáo tổng hợp cho đón trả khách
            //if (device.Temp.HasGuestSensor)
            if (device.DeviceType== DeviceType.TaxiVehicle)
            {
                try
                {
                    lock (device.Temp.GeneralGuestLogLock)
                    {
                        //tạo mới cho gói tin đầu ngày
                        if (device.Temp.GeneralGuestLog == null)
                            device.Temp.GeneralGuestLog = new GeneralGuestLog
                            {
                                UpdateTime = DateTime.Now.Date.AddHours(5),
                                GuidId = device.Indentity,
                                CompanyId = device.CompanyId,
                                Id = 0,
                                DbId = company.DbId,
                                GuestTimeInday = 0,
                                KmGuestOnDay = 0,
                                NoGuestTimeInday = 0,
                                KmNoGuestOnDay = 0,
                                GroupId = device.GroupId
                            };
                    }
                }
                catch (Exception e)
                {
                    uTils.Log.Exception("GeneralReportLogic", e, "Lỗi hàm đón trả khách");
                }
            }
            #endregion Xử lý báo cáo tổng hợp cho đón trả khách

        }

        private void HandleWakeupLostDevice(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            try
            {
                if ((device.Status.BasicStatus.ServerRecv - device.Temp.DeviceLostGsmLog.BeginTime).TotalMinutes < 60) return;

                DeviceTraceLog trace = device.Temp.DeviceLostGsmLog;
                trace.DbId = company.DbId;
                trace.EndLocation = device.Status.BasicStatus.GpsInfo;
                trace.EndTime = device.Status.BasicStatus.ServerRecv;
                //tính khoảng cách theo thông tin thiết bị gửi lên
                trace.Distance = device.Status.BasicStatus.TotalGpsDistance - trace.Distance;
                if (trace.Distance < 0) trace.Distance = 0;

                //Lấy địa chỉ begin location
                if(trace.BeginLocation!=null) uTils.LocationQuery.GetAddress(trace.BeginLocation);
                //Nếu như vị trí kết thúc gần với vị trí ban đầu thì lấy địa chỉ ban đầu
                if (trace.BeginLocation != null && trace.EndLocation != null
                    && StarSg.Utils.Geos.GeoUtil.Distance(trace.BeginLocation.Lat, trace.BeginLocation.Lng
                        , trace.EndLocation.Lat, trace.EndLocation.Lng) < 100)
                    trace.EndLocation.Address = trace.BeginLocation.Address;
                else if(trace.EndLocation!=null)
                    uTils.LocationQuery.GetAddress(trace.EndLocation);

                uTils.CacheLog.PushTraceLog(trace);

                device.Temp.DeviceLostGsmLog = null;

                //uTils.Log.Debug("PACKET", $"HandleWakeupLostDevice : {device.Serial}");
            }
            catch (Exception e)
            {
                uTils.Log.Exception("GeneralReportLogic", e, $"HandleWakeupLostDevice {device.Serial}");
            }
        }


    }
}