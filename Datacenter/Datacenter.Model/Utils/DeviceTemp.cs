#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Model
// TIME CREATE : 9:18 PM 20/06/2016
// FILENAME: DeviceTemp.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.Collections.Generic;
using System.Threading;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using System.Collections.Concurrent;
using System.IO;
using DevicePacketModels.Events;

#endregion

namespace Datacenter.Model.Utils
{
    public class DeviceTemp : ISerializerModal
    {
        public virtual long Serial { get; set; }

        public OverSpeedLog09   OverSpeedLog09 { get; set; }
        private readonly    ConcurrentDictionary<TraceType, DeviceTraceLog> _lasttrace = new ConcurrentDictionary<TraceType, DeviceTraceLog>();

        public virtual int OverTime4HCount { get; set; }

        /// <summary>
        /// Tổng khoảng cách khi xe bắt đầu chạy, dùng để kiểm tra xe có chạy hay không
        /// </summary>
        public virtual long TraceStartDistance { get; set; }

        public virtual int IdPoint { get; set; }
        public virtual DateTime PointBeginTime { get; set; }

        public virtual int IdArea { get; set; }
        public virtual DateTime AreaBeginTime { get; set; }

        public virtual DeviceModel Model { get; set; }

        /// <summary>
        ///     vân tốc tối đa của thiết bị
        /// </summary>
        public virtual int MaxSpeed { get; set; }

        /// <summary>
        ///     thời gian sync lần trước đó
        /// </summary>
        public virtual DateTime LastSyncDevice { get; set; }
        public virtual GpsLocation LastLocation { get; set; }
        public virtual int OldSpeed09 { get; set; }

        public Object GeneralReportLogLock = new Object();
        public virtual GeneralReportLog GeneralReportLog { get; set; }//version 1

        public Object GeneralGuestLogLock = new Object();
        public virtual GeneralGuestLog GeneralGuestLog { get; set; }//version 7


        //public virtual int GeneralReportCount { get; set; }

        public virtual int OpenDoorCount { get; set; }
        public virtual int OverSpeedCount { get; set; }
        public virtual int PauseCount { get; set; }
        public virtual int AirConditionCount { get; set; }//số lần mở máy lạnh
        public virtual int OffMachineCount { get; set; }

        ///// <summary>
        ///// thời gian xử lý sự kiện dừng 15 phút
        ///// </summary>
        //public DateTime TimeHandlePause { get; set; }
        //public DateTime TimeHandleRun { get; set; }
        //public GpsLocation OldLocation { get; set; }

        /// <summary>
        /// Lưu lại thông tin truyền lên bộ trong ngày
        /// </summary>
        public virtual DateTime BgtvLastTransfer { get; set; } = DateTimeFix.Min; //version 2
        public virtual int      BgtvCountTransfer { get; set; }//version 2
        public virtual int      BgtvCount { get; set; }//version 2

        //Version 8 remove Fuel fields
        ///// <summary>
        ///// Giá trị nhiên liệu trước đó
        ///// </summary>
        //public virtual DateTime FuelLastTime { get; set; }//version 4
        //public virtual int      FuelLastValue { get; set; }//version 4
        //public virtual bool     FuelHasIssue { get; set; }//version 4

        public FuelTest FuelTest { get; set; }//version 9

        ///// <summary>
        ///// Có cảm biến đón khách
        ///// </summary>
        //public virtual bool HasGuestSensor { get; set; }//version 6,remove at version 18

        /// <summary>
        /// Tình trạng có sự kiện dừng (theo sự kiện 108/109)
        /// </summary>
        public virtual bool HasStopEvent { get; set; }//no saving

        /// <summary>
        /// Thời điểm dừng lần cuối, nếu giá trị này là <= 2000-01-01 thì có nghĩa là xe đang chạy
        /// </summary>
        public DateTime TimePause { get; set; }//version 10


        /// <summary>
        /// Đã gửi tin nhắn báo thiết bị offline
        /// </summary>
        public virtual bool SentOnlineSms { get; set; }


        /// <summary>
        /// Thời gian mở máy tính bằng giây ( tính phòng hờ nếu như TimeWorkInDay không chính xác cho XCT)
        /// </summary>
        public virtual int MachineSeconds { get; set; } //version 17

        /// <summary>
        /// Ghi nhận số lần gói tin trùng trong ngày
        /// </summary>
        public virtual int SyncDuplicate { get; set; } //not save

        /// <summary>
        /// Ghi nhận số lần gói tin kết thúc sự kiện mà không có gói tin bắt đầu tương ứng
        /// </summary>
        public virtual int EventWithoutStart { get; set; } //not save

        /// <summary>
        /// Ghi nhận số lần gói tin bắt đầu sự kiện mà không có gói tin kết thúc tương ứng
        /// </summary>
        public virtual int EventWithoutEnd { get; set; } //not save

        /// <summary>
        /// Ghi nhận thời gian của Sự kiện Thông tin lái xe liên tục của tái xế P114EndOvertime nhằm mục đích loại trùng do lỗi phát từ thiết bị
        /// </summary>
        public virtual DateTime LastEndOverTime { get; set; } = DateTimeFix.Min;//not save

        /// <summary>
        /// Thông tin tọa độ gói sync lúc vừa qua 0 giờ ( dùng để xử lý cuốc quá 10 giờ...)
        /// </summary>
        public virtual GpsLocation MidnightLocation { get; set; } = new GpsLocation();//version 20 ,Chỉ lưu tọa độ, không lưu địa chỉ

        /// <summary>
        /// Thông tin cuốc liên tục cuối cùng trong ngày để xử lý kiểm tra quá 10h
        /// </summary>
        public virtual Daily10HTemp Last10hTrace { get; set; } //version 20

        /// <summary>
        /// Thời điểm cập nhật device status lần cuối, sử dụng để kiểm tra và ghi định kì (không ghi liên tục) đối với gói sync
        /// </summary>
        public virtual DateTime     DeviceStatusUpdateTime { get; set; } = DateTimeFix.Min;//version 21

        /// <summary>
        /// tb đang mất tín hiệu
        /// </summary>
        public virtual DeviceTraceLog DeviceLostGsmLog { get; set; }//version 22

        /// <summary>
        /// luu dum cho ben Device Status LastTotalKmUsingOnDay
        /// </summary>
        public virtual long LastTotalKmUsingOnDay { get; set; } //version 23


        public virtual void Reset()
        {
            OpenDoorCount = 0;
            OverSpeedCount = 0;
            PauseCount = 0;
            OffMachineCount = 0;
            AirConditionCount = 0;
            OverTime4HCount = 0;
            BgtvCount = 0;
            BgtvCountTransfer = 0;
            MachineSeconds = 0;
            SyncDuplicate = 0;
            EventWithoutStart = 0;
            EventWithoutEnd = 0;
        }

        /// <summary>
        /// Bắt đầu lưu lại sự kiện 
        /// </summary>
        /// <param name="obj"></param>
        public virtual bool BeginTrace(DeviceTraceLog obj)
        {
            return _lasttrace.TryAdd(obj.Type, obj);
        }

        /// <summary>
        /// Lấy sự kiện kiểm tra
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual DeviceTraceLog GetTrace(TraceType type)
        {
            DeviceTraceLog ret = null;
            if (_lasttrace.TryGetValue(type, out ret))
                return ret;
            return null;
        }

        /// <summary>
        /// kiểm tra sự kiện tồn tại không
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool ExistTrace(TraceType type)
        {
            return _lasttrace.ContainsKey(type);
        }

        /// <summary>
        /// Kết thúc và hủy sự kiện
        /// </summary>
        /// <param name="obj"></param>
        public virtual DeviceTraceLog EndTrace(TraceType type)
        {
            DeviceTraceLog ret = null;
            //if (!_lasttrace.ContainsKey(type)) return null;
            if (_lasttrace.TryRemove(type, out ret))
                return ret;
            return null;
        }

        /// <summary>
        /// Kết thúc và hủy sự kiện của đối tượng
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool EndTrace(DeviceTraceLog obj)
        {
            if (obj == null) return false;
            return EndTrace(obj.Type) != null;
        }

        public virtual ICollection<DeviceTraceLog> LastTraces
        {
            get
            {
                return _lasttrace.Values;
            }
        }

        public virtual void Deserializer(BinaryReader stream, int version)
        {
            Serial = stream.ReadInt64();

            int n = stream.ReadInt32();
            _lasttrace.Clear();
            for (int i = 0; i < n; i++)
            {
                DeviceTraceLog obj = new DeviceTraceLog();
                obj.Type = (TraceType)stream.ReadByte();
                obj.BeginTime = DateTime.FromBinary(stream.ReadInt64());
                obj.Distance = stream.ReadInt64();
                obj.DriverTime = DateTime.FromBinary(stream.ReadInt64());
                obj.BeginLocation = new GpsLocation();
                obj.BeginLocation.Lat = stream.ReadSingle();
                obj.BeginLocation.Lng = stream.ReadSingle();
                BeginTrace(obj);
            }

            if(stream.ReadBoolean())
            {
                OverSpeedLog09 = new OverSpeedLog09();
                OverSpeedLog09.Deserializer(stream, version);
            }

            if(version>=1)
            {
                if (stream.ReadBoolean())
                {
                    GeneralReportLog = new GeneralReportLog();
                    GeneralReportLog.Deserializer(stream, version);
                }
            }

            OverTime4HCount = stream.ReadInt32();
            TraceStartDistance = stream.ReadInt64();
            //MaxSpeed = stream.ReadInt32();
            //LastSyncDevice = DateTime.FromBinary(stream.ReadInt64());
            //LastLocation = new GpsLocation();
            //LastLocation.Lat = stream.ReadSingle();
            //LastLocation.Lng = stream.ReadSingle();
            //OldSpeed09 = stream.ReadInt32();
            //GeneralReportCount = stream.ReadInt32();
            OpenDoorCount = stream.ReadInt32();
            OverSpeedCount = stream.ReadInt32();
            PauseCount = stream.ReadInt32();
            AirConditionCount = stream.ReadInt32();
            OffMachineCount = stream.ReadInt32();

            if (version < 16)
            {
                stream.ReadInt64();// TimeHandlePause = DateTime.FromBinary(stream.ReadInt64());
                stream.ReadInt64();// TimeHandleRun = DateTime.FromBinary(stream.ReadInt64());
            }

            ////OldLocation = new GpsLocation();
            ////OldLocation.Lat = stream.ReadSingle();
            ////OldLocation.Lng = stream.ReadSingle();

            if(version>=2)
            {
                BgtvLastTransfer = DateTime.FromBinary(stream.ReadInt64());
                BgtvCountTransfer = stream.ReadInt32();
                BgtvCount = stream.ReadInt32();
            }

            if (version >= 3)
            {
                IdPoint = stream.ReadInt32();
                PointBeginTime = DateTime.FromBinary(stream.ReadInt64());
                IdArea = stream.ReadInt32();
                AreaBeginTime = DateTime.FromBinary(stream.ReadInt64());
            }

            if(version>=4 && version < 8)
            {
                //DateTime FuelLastTime = DateTime.FromBinary(stream.ReadInt64());
                stream.ReadInt64();
                //int FuelLastValue = 
                stream.ReadInt32();
                //bool FuelHasIssue = 
                stream.ReadBoolean();
            }

            if (version >= 6 && version < 18)
            {
                stream.ReadBoolean();
            }

            if(version >=7 )
            {
                if (stream.ReadBoolean())
                {
                    GeneralGuestLog = new GeneralGuestLog();
                    GeneralGuestLog.Deserializer(stream, version);
                }
            }

            if(version>=9)
            {
                if (stream.ReadBoolean())
                {
                    FuelTest = new FuelTest();
                    FuelTest.Deserializer(stream, version);
                }
            }

            if (version >= 10)
            {
                TimePause = DateTime.FromBinary(stream.ReadInt64());
            }

            if (version >= 14)
            {
                SentOnlineSms = stream.ReadBoolean();
            }

            if (version >= 17)
            {
                MachineSeconds = stream.ReadInt32();
            }

            if (version >= 20)
            {
                MidnightLocation.Lat = stream.ReadSingle();
                MidnightLocation.Lng = stream.ReadSingle();
                if (stream.ReadBoolean())
                {
                    Last10hTrace = new Daily10HTemp();
                    Last10hTrace.Deserializer(stream, version);
                }
            }

            if(version>=21)
                DeviceStatusUpdateTime = DateTime.FromBinary(stream.ReadInt64());

            if (version >= 22)
            {
                if (stream.ReadBoolean())
                {
                    DeviceLostGsmLog = new DeviceTraceLog();
                    DeviceLostGsmLog.Deserializer(stream, version);
                }
            }

            if (version >= 23)
            {
                LastTotalKmUsingOnDay = stream.ReadInt64();
            }

        }

        public virtual void Serializer(BinaryWriter stream)
        {
            stream.Write(Serial);

            stream.Write(_lasttrace.Count);
            foreach(var obj in _lasttrace.Values)
            {
                stream.Write((byte)obj.Type);
                obj.BeginTime.Fix(); stream.Write(obj.BeginTime.ToBinary());
                stream.Write(obj.Distance);
                stream.Write(obj.DriverTime.ToBinary());
                stream.Write(obj.BeginLocation?.Lat??0f);
                stream.Write(obj.BeginLocation?.Lng??0f);
            }

            if (OverSpeedLog09 == null)
                stream.Write(false);
            else
            {
                stream.Write(true);
                OverSpeedLog09.Serializer(stream);
            }

            //version 1
            if (GeneralReportLog == null)
                stream.Write(false);
            else
            {
                stream.Write(true);
                GeneralReportLog.Serializer(stream);
            }

            stream.Write(OverTime4HCount);
            stream.Write(TraceStartDistance);
            //stream.Write(MaxSpeed);
            //LastSyncDevice.Fix();stream.Write(LastSyncDevice.ToBinary());
            //stream.Write(LastLocation?.Lat??0f);
            //stream.Write(LastLocation?.Lng??0f);
            //stream.Write(OldSpeed09);
            //stream.Write(GeneralReportCount);
            stream.Write(OpenDoorCount);
            stream.Write(OverSpeedCount);
            stream.Write(PauseCount);
            stream.Write(AirConditionCount);
            stream.Write(OffMachineCount);

            //version 16
            //TimeHandlePause.Fix();stream.Write(TimeHandlePause.ToBinary());
            //TimeHandleRun.Fix();stream.Write(TimeHandleRun.ToBinary());
            ////stream.Write(OldLocation.Lat);
            ////stream.Write(OldLocation.Lng);

            //version 2
            stream.Write(BgtvLastTransfer.ToBinary());
            stream.Write(BgtvCountTransfer);
            stream.Write(BgtvCount);

            //version 3
            stream.Write(IdPoint);
            stream.Write(PointBeginTime.ToBinary());
            stream.Write(IdArea);
            stream.Write(AreaBeginTime.ToBinary());

            ////Version 4 - Version 8 remove following
            //stream.Write(FuelLastTime.ToBinary());
            //stream.Write(FuelLastValue);
            //stream.Write(FuelHasIssue);

            //Version 6, remove at Version 18
            //stream.Write(HasGuestSensor);

            //version 7
            if (GeneralGuestLog == null)
                stream.Write(false);
            else
            {
                stream.Write(true);
                GeneralGuestLog.Serializer(stream);
            }

            //version 9
            if (FuelTest == null)
                stream.Write(false);
            else
            {
                stream.Write(true);
                FuelTest.Serializer(stream);
            }

            //version 10
            stream.Write(TimePause.ToBinary());

            //version 14
            stream.Write(SentOnlineSms);

            //version 17
            stream.Write(MachineSeconds);

            //version 20
            stream.Write(MidnightLocation.Lat);
            stream.Write(MidnightLocation.Lng);
            if (Last10hTrace == null)
                stream.Write(false);
            else
            {
                stream.Write(true);
                Last10hTrace.Serializer(stream);
            }

            //version 21
            stream.Write(DeviceStatusUpdateTime.ToBinary());

            //version 22
            if (DeviceLostGsmLog == null)
                stream.Write(false);
            else
            {
                stream.Write(true);
                DeviceLostGsmLog.Serializer(stream);
            }

            //version 23
            stream.Write(LastTotalKmUsingOnDay);
        }


        public virtual bool HasMachineOn
        {
            get
            {
                //return null != GetTrace(TraceType.Machine);
                return ExistTrace(TraceType.Machine);
            }
        }

    }

}