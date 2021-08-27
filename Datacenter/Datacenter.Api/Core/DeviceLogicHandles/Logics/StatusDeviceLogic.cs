#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : StatusDeviceLogic.cs
// Time Create : 2:58 PM 23/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Linq;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using DevicePacketModels;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    /// </summary>
    [Sort(1)]
    public class StatusDeviceLogic : ILogic
    {
        private readonly AngleConverter _angleConverter = new AngleConverter();

        #region Implementation of ILogic

        /// <summary>
        ///     xử lý các thông tin
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="uTils"></param>
        /// <param name="device"></param>
        /// <param name="company"></param>
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            if (device.Status.BasicStatus.GpsInfo == null)
                device.Status.BasicStatus.GpsInfo = new GpsLocation();
            // cập nhật và tính góc theo tọa độ GPS
            var old = device.Status.BasicStatus.GpsInfo;

            //device.Temp.OldLocation = old;
            device.Status.BasicStatus.GpsStatus = packet.GpsStatus;
            if (device.Status.BasicStatus.GpsStatus)
                device.Status.BasicStatus.GpsInfo = new GpsLocation
                {
                    Lat = packet.GpsInfo.Lat,
                    Lng = packet.GpsInfo.Lng
                };
            if (old != null && old != device.Status.BasicStatus.GpsInfo)
                device.Status.BasicStatus.Angle = CalculateBearing(old, device.Status.BasicStatus.GpsInfo);

            // cập nhật các trạng thái IO
            //device.Status.BasicStatus.AirMachine = packet.StatusIo.AirMachine;
            //device.Status.BasicStatus.Door = packet.StatusIo.Door;
            device.Status.BasicStatus.AirMachine = device.InvertAir?(!packet.StatusIo.AirMachine): packet.StatusIo.AirMachine;
            device.Status.BasicStatus.Door = device.InvertDoor?(!packet.StatusIo.Door): packet.StatusIo.Door;

            device.Status.BasicStatus.Machine = packet.StatusIo.Key;
            device.Status.BasicStatus.Sos = packet.StatusIo.Sos;
            device.Status.BasicStatus.UseFuel = packet.StatusIo.UseFuel;
            device.Status.BasicStatus.UseRfid = packet.StatusIo.UseRfid;
            device.Status.BasicStatus.UseTemperature = packet.StatusIo.UseTemperature;

            // Nếu qua ngày mới thì lưu lại tọa độ gói syn vừa qua 0 giờ vào biến tạm (sử dụng cho cuốc quá 10h) 
            if(packet.Time.Date > device.Status.BasicStatus.ClientSend.Date)
                device.Temp.MidnightLocation = device.Status.BasicStatus.GpsInfo;

            //// thời gian
            //device.Status.BasicStatus.ClientSend = packet.Time;
            //device.Status.BasicStatus.ServerRecv = DateTime.Now;
            //device.Temp.SentOnlineSms = false;//khi nhận lại tín hiệu thì reset lại biến này

            // tín hiệu GSM
            device.Status.BasicStatus.GsmSignal = packet.GsmSignal;
            // nguồn
            device.Status.BasicStatus.Power = packet.Power;
            device.Status.BasicStatus.Temperature = packet.Temperature;

            //chỉ cập nhật khi có tín hiệu gps VA thoi gian goi thiet bi phai luon lon hon thoi gian truoc do
            if (device.Status.BasicStatus.GpsStatus && packet.Time > device.Status.BasicStatus.ClientSend)
            {
                if(packet.TotalCurrentGpsDistance>0)
                    device.Status.BasicStatus.TotalCurrentGpsDistance = packet.TotalCurrentGpsDistance;
                if(packet.TotalGpsDistance>0)
                    device.Status.BasicStatus.TotalGpsDistance = packet.TotalGpsDistance;
            }

            //sua loi sau khi doi thiet bi 
            //if(device.Status.BasicStatus.TotalGpsDistance < device.Status.LastTotalKmUsingOnDay)
            if (
                device.Status.BasicStatus.TotalGpsDistance < 5000 // < 5km --> thiet bi moi lap
                && device.Status.BasicStatus.TotalGpsDistance < device.Status.LastTotalKmUsingOnDay
                )
            {
                device.Temp.LastTotalKmUsingOnDay
                                         = device.Status.LastTotalKmUsingOnDay
                                         = device.Status.BasicStatus.TotalGpsDistance;
            }

            // thời gian
            device.Status.BasicStatus.ClientSend = packet.Time;
            device.Status.BasicStatus.ServerRecv = DateTime.Now;
            device.Temp.SentOnlineSms = false;//khi nhận lại tín hiệu thì reset lại biến này

            device.Status.BasicStatus.SpeedTrace = packet.SpeedLogs.Aggregate("",
                (current, speedLog) => current + $"{speedLog},");

            device.Status.BasicStatus.Speed = packet.GpsInfo.Speed;

            //TimeWorkInDay bị sai trong 1 số trường hợp như xe công trình (đứng 1 chỗ)
            device.Status.DriverStatus.TimeWorkInDay = packet.TimeWorkInDay;

            //Nếu là xe công trình thì thời gian chạy trong ngày sẽ được tính theo cuốc xe đi
            if(device.DeviceType==DeviceType.ConstructionVehicle || device.DeviceType == DeviceType.Dynamo)
            {
                //tổng giá trị các cuốc trước đó
                int totalseconds = device.Temp.MachineSeconds;
                //cuốc hiện hành
                var trace = device.Temp.GetTrace(Model.Log.TraceType.Machine);
                if(trace!=null && device.Status.BasicStatus.Machine)//chỉ xét khi máy vẫn còn bật chìa khóa
                {
                    //tính ra giá trị cuốc hiện hành đến thời điểm gói sync cuối cùng
                    int currentseconds= (int)Math.Round((device.Status.BasicStatus.ClientSend - trace.BeginTime).TotalSeconds);

                    //if (currentseconds > 86500) //86400
                    //    uTils.Log.Error("StatusDeviceLogic", $"Thời gian mở máy cuối cùng quá 1 ngày {device.Serial} mở máy {trace.BeginTime}");
                    //if (totalseconds + currentseconds > 86500) //86400
                    //    uTils.Log.Error("StatusDeviceLogic", $"Thời gian mở máy quá 1 ngày {device.Serial} tổng sự kiện {device.Temp.MachineSeconds} ");

                    //else 
                    if (currentseconds > 0)
                        totalseconds += currentseconds;
                }

                //quy ra phút
                totalseconds /= 60;

                //Lấy giá trị max
                //device.Status.DriverStatus.TimeWorkInDay = Math.Max(device.Status.DriverStatus.TimeWorkInDay, totalseconds);
                device.Status.DriverStatus.TimeWorkInDay = totalseconds;

            }

            device.Status.DriverStatus.TimeWork = packet.TimeWork;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        private double CalculateBearing(GpsLocation position1, GpsLocation position2)
        {
            var lat1 = _angleConverter.ConvertDegreesToRadians(position1.Lat);
            var lat2 = _angleConverter.ConvertDegreesToRadians(position2.Lat);
            var long1 = _angleConverter.ConvertDegreesToRadians(position2.Lng);
            var long2 = _angleConverter.ConvertDegreesToRadians(position1.Lng);
            var dLon = long1 - long2;

            var y = Math.Sin(dLon)*Math.Cos(lat2);
            var x = Math.Cos(lat1)*Math.Sin(lat2) - Math.Sin(lat1)*Math.Cos(lat2)*Math.Cos(dLon);
            var brng = Math.Atan2(y, x);

            return (_angleConverter.ConvertRadiansToDegrees(brng) + 360)%360;
        }

        /// <summary>
        /// </summary>
        public class AngleConverter
        {
            /// <summary>
            /// </summary>
            /// <param name="angle"></param>
            /// <returns></returns>
            public double ConvertDegreesToRadians(double angle)
            {
                return Math.PI*angle/180.0;
            }

            /// <summary>
            /// </summary>
            /// <param name="angle"></param>
            /// <returns></returns>
            public double ConvertRadiansToDegrees(double angle)
            {
                return 180.0*angle/Math.PI;
            }
        }
    }
}