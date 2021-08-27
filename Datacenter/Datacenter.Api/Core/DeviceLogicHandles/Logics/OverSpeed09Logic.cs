#region header
// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Datacenter.Api
// TIME CREATE : 2:38 PM 18/12/2016
// FILENAME: OverSpeed09Logic.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************
#endregion

using System;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Utils;
using DevicePacketModels;
using StarSg.Utils.Models.Tranfer;

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    /// 
    /// </summary>
    [Sort(4)]
    public class OverSpeed09Logic : ILogic
    {
        public const int DEFAULT_SPEED = 80;

        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            try
            {
                //todo : ráp api lấy thông tin địa chỉ theo việt bản đồ vào đây
                //nếu thời gian của lần sync chưa có thì gán giá trị và return
                if (!device.Temp.LastSyncDevice.IsValidDatetime())
                {
                    SaveLastData(device);
                    return;
                }

                // SpeedZoneUtils.GetInstance().IsInSpeedZone(device.Status.BasicStatus.CurrentLocation);
                var mSpeed = (int)(device.SetupInfo?.OverSpeed??0);
                if (mSpeed == 0) mSpeed = DEFAULT_SPEED;

                #region không lấy tốc độ tối đa từ vdb mà lấy theo thiết bị
                //if (device.Temp.OldLocation != null &&
                //    (int)device.Temp.OldLocation.Lat != 0 && (int)device.Temp.OldLocation.Lng != 0)
                //{
                //    var s = VbdServices.GetSpeedOfStreet(device.Temp.OldLocation.Lat, device.Temp.OldLocation.Lng,
                //        device.Status.BasicStatus.GpsInfo.Lat, device.Status.BasicStatus.GpsInfo.Lng);
                //    if (s > 0)
                //        mSpeed = s;
                //}
                #endregion không lấy tốc độ tối đa từ vdb mà lấy theo thiết bị

                //tính thời gian giữa 2 bản tin N và N-1
                var time2Sync = (int)
                    device.Status.BasicStatus.ClientSend.Subtract(device.Temp.LastSyncDevice).TotalSeconds;
                if (//(zoneCheck != null && device.Status.BasicStatus.Speed > mSpeed + 5) ||
                    //(zoneCheck == null &&
                     device.Status.BasicStatus.Speed > mSpeed + 5)
                {
                    if (time2Sync <= 60)
                    {
                        //nếu lần sync mà lớn hơn > 10 giây, khoảng cách tính theo m
                        var distance = StarSg.Utils.Geos.GeoUtil.Distance(device.Status.BasicStatus.GpsInfo.Lat,
                                device.Status.BasicStatus.GpsInfo.Lng, device.Temp.LastLocation.Lat,
                                device.Temp.LastLocation.Lng);
                        //test
                        //distance = 1000;
                        //tính vận tốc trung bình
                        var vtb = (distance / time2Sync) * 3.6f;
                        //trường hợp thời gian sync giữa 2 lần <= 10 giây
                        if (time2Sync <= 10)
                        {
                            vtb = (device.Temp.OldSpeed09 + device.Status.BasicStatus.Speed) / 2.0;
                            distance = (vtb * time2Sync * 1000f) / 3600f;
                        }
                        var vss = vtb - 5;
                        //uTils.Log.Debug("OverSpeed09Logic", $"OverSpeed09Logic:{time2Sync}:::{device.Status.BasicStatus.ClientSend}:::{device.DeviceTemp.LastSyncDevice}:vss={vss}");
                        if (//(zoneCheck != null && vss > zoneCheck.MaxSpeed) ||
                            //(zoneCheck == null && vss > int.Parse(device.SetupInfo.SpeedMax ?? "90")))
                            vss > mSpeed
                            )
                        {
                            //nếu đã có bản ghi quá tốc độ 
                            //if (device.Temp.OldOverSpeed09Id > 0)
                            if (device.Temp.OverSpeedLog09!=null)
                            {
                                //update thông tin
                                var over = device.Temp.OverSpeedLog09;
                                    //device.Temp.PopObject<OverSpeedLog09>(device.Temp.OldOverSpeed09Id,false);
                                if (over.MaxSpeed < vss)
                                {
                                    over.MaxSpeed = (int)vss;
                                }
                                over.TotalSpeed += (int)vss;
                                over.TotalTimeOver += time2Sync;
                                over.TotalDistance += (int)distance;
                                over.CountSpeed += 1;
                            }
                            else
                            {
                                //lưu bắt đầu quá tốc độ
                                // bắt đầu quá vận tốc ở đây
                                //var limitSpeed = device.SetupInfo.OverSpeed;
                                //if (zoneCheck != null)
                                //{
                                //    limitSpeed = zoneCheck.MaxSpeed;
                                //}
                                var over = new OverSpeedLog09()
                                {
                                    BeginTime =
                                        new DateTime(device.Status.BasicStatus.ClientSend.Year,
                                            device.Status.BasicStatus.ClientSend.Month,
                                            device.Status.BasicStatus.ClientSend.Day,
                                            device.Status.BasicStatus.ClientSend.Hour,
                                            device.Status.BasicStatus.ClientSend.Minute,
                                            device.Status.BasicStatus.ClientSend.Second),
                                    CompanyId = company.Id,
                                    DbId = company.DbId,
                                    DriverId = device.Status.DriverStatus?.DriverId ?? 0,
                                    GroupId = device.GroupId,
                                    LimitSpeed = mSpeed,
                                    Id = 0,
                                    EndTime = DateTime.Now,
                                    MaxSpeed = (int)vss,
                                    BeginPoint =
                                        new Model.Components.GpsLocation
                                        {
                                            Lat = device.Status.BasicStatus.GpsInfo.Lat,
                                            Lng = device.Status.BasicStatus.GpsInfo.Lng
                                        },
                                    Serial = device.Serial,
                                    Indentity = device.Indentity,
                                    CountSpeed = 1,
                                    TotalSpeed = (int)vss,
                                    AverageSpeed = (int)vss,
                                    TotalTimeOver = time2Sync,
                                    TotalDistance = (int)distance
                                };
                                uTils.LocationQuery.GetAddress(over.BeginPoint);

                                //device.Temp.OldOverSpeed09Id = device.Temp.PushObject(over);
                                device.Temp.OverSpeedLog09 = over;

                                uTils.Log.Debug("LOGIC",
                                    $"Thiết bị {device.Serial} bắt đầu quá vận tốc thông tư 09");
                                //$"Thiết bị {device.Serial} bắt đầu quá vận tốc thông tư 09 {device.Temp.OldOverSpeed09Id}");
                            }
                        }
                        else
                        {
                            //kết thúc quá tốc độ
                            FunctionEndOverSpeed(device, uTils);
                        }
                    }
                    else
                    {
                        //kết thúc quá tốc độ
                        FunctionEndOverSpeed(device, uTils);
                    }
                }
                else
                {
                    //kết thúc quá tốc độ
                    FunctionEndOverSpeed(device, uTils);
                }
                //lưu thông tin last device, để tính toán vận tốc trung bình của device
                SaveLastData(device);
            }
            catch (Exception ex)
            {
                uTils.Log.Exception("hàm OverSpeed09Logic", ex, "hàm OverSpeed09Logic");
            }
        }


        /// <summary>
        /// lưu thời gian và vị trí hiện tại cho lần sync tiếp theo
        /// </summary>
        /// <param name="device"></param>
        public void SaveLastData(Device device)
        {
            device.Temp.OldSpeed09 = device.Status.BasicStatus.Speed;
            device.Temp.LastSyncDevice = new DateTime(device.Status.BasicStatus.ClientSend.Year,
                                            device.Status.BasicStatus.ClientSend.Month,
                                            device.Status.BasicStatus.ClientSend.Day,
                                            device.Status.BasicStatus.ClientSend.Hour,
                                            device.Status.BasicStatus.ClientSend.Minute,
                                            device.Status.BasicStatus.ClientSend.Second);
            device.Temp.LastLocation = new Model.Components.GpsLocation() { Address = device.Status.BasicStatus.GpsInfo.Address, Lat = device.Status.BasicStatus.GpsInfo.Lat, Lng = device.Status.BasicStatus.GpsInfo.Lng };
        }

        /// <summary>
        /// tính toán kết thúc quá tốc độ
        /// </summary>
        /// <param name="device"></param>
        /// <param name="uTils"></param>
        public void FunctionEndOverSpeed(Device device, ILogicUtil uTils)
        {
            //nếu có bản ghi quá tốc độ
            //if (device.Temp.OldOverSpeed09Id > 0)
            if (device.Temp.OverSpeedLog09!=null)
            {
                //var over = device.Temp.PopObject<OverSpeedLog09>(device.Temp.OldOverSpeed09Id);
                var over = device.Temp.OverSpeedLog09;device.Temp.OverSpeedLog09 = null;
                //device.Temp.OldOverSpeed09Id = 0;
                if (over != null && over.TotalTimeOver > 30)
                {
                    over.EndTime = over.BeginTime.AddSeconds(over.TotalTimeOver);
                    //tính vận tốc quá tốc độ trung bình
                    over.AverageSpeed = over.CountSpeed != 0 ? over.TotalSpeed / over.CountSpeed : 0;
                    over.EndPoint = new Model.Components.GpsLocation
                    {
                        Lat = device.Status.BasicStatus.GpsInfo.Lat,
                        Lng = device.Status.BasicStatus.GpsInfo.Lng,
                        Address = ""
                    };
                    //over.TotalTimeOver /= 60;
                    uTils.LocationQuery.GetAddress(over.EndPoint);
                    uTils.Log.Debug("LOGIC", $"Thiết bị {device.Serial} kết thúc quá vận tốc theo thông tư 09");

                    //kiem tra 0
                    if (over.LimitSpeed == 0) over.LimitSpeed = DEFAULT_SPEED;

                    //cập nhật xử lý gửi email nếu có ( theo thông tư 09)
                    if (device.EmailAlarm && !String.IsNullOrWhiteSpace(device.EmailAddess))
                    {
                        var driver = uTils.DataCache.GetQueryContext<Driver>().GetByKey(device.Status?.DriverStatus.DriverId ?? 0L);
                        uTils.CacheLog.PushOverSpeedAlarmEmail(device
                            , driver
                            , over.EndPoint.Lat
                            , over.EndPoint.Lng
                            , over.MaxSpeed
                            , over.AverageSpeed
                            , over.TotalDistance
                            ,over.EndTime
                            );
                    }

                    //cập nhật db
                    uTils.DataContext.Insert(over, uTils.MotherSqlId);
                    uTils.DataContext.Commit();
                }
            }

        }


    }
}