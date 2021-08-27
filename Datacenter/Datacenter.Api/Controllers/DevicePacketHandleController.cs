// from A Phi
// Cái đóng của Và máy lạnh ... thì bỏ
// kg cần lấy dia chỉ làm gì
// Chỉ có mấy sự kiện như
// Bật máy , đi , dừng , tắt máy
// nếu khoản cach =0
// thì có thể bỏ qua
// giữa 2 lần

// From Luật
// Sự kiện dừng/đi không đo khoãng cách chỉ đo thời gian
// Sự kiện đóng mở cửa không xài 

#region

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Core.Utils;
using Datacenter.Api.Core;
using Datacenter.Api.Core.DeviceLogicHandles;
using Datacenter.DataTranport;
using Datacenter.DataTranport.Models;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Setup;
using DataCenter.Core;
using DevicePacketModels;
using DevicePacketModels.Events;
using DevicePacketModels.Setups;
using StarSg.Utils.Models.DatacenterResponse.Status;
using StarSg.Utils.Models.Tranfer;
using System.Collections.Generic;
using CorePacket.Utils;
using System.IO;
using DevicePacketModels.Utils;
using StarSg.Utils.Geos;
using Datacenter.Model.Utils;
using StarSg.Core;


#endregion

namespace Datacenter.Api.Controllers
{
    /// <summary>
    ///     xửa lý các gói tin phát từ thiết bị
    /// </summary>
    [Auth]
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DevicePacketHandleController : BaseDeviceController
    {
        private const int GUEST_MIN_TIME = 60;//seconds //cu la 1
        private const int GUEST_MIN_DISTANCE = 100;//meter //cu la 10
        private const int GUEST_MAX_SPEED = 10;//km/hour

        private readonly static IEnumerable<Type> gLogicHandleType;

        [Import]
        private IBgtTranport _bgtTranport;

        [Import]
        private CacheLogManager _cacheLog;

        [Import]
        private ILocationQuery _locationQuery;
        [Import]
        private RealtimeDeviceSatus _realtimeDeviceSatus;

        static DevicePacketHandleController()
        {
            var logicType = (from t in Assembly.GetExecutingAssembly().GetTypes()
                             let at = t.GetCustomAttribute<SortAttribute>()
                             where at != null
                             select new Tuple<int, Type>(at.Index, t)).ToList();
            // sắp xếp các logic theo các mức ưu tiên
            gLogicHandleType = logicType.OrderBy(m => m.Item1).Select(m => m.Item2);
        }


        private void UpdateStatus()
        {
            if (Device.Status != null)
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _bgtTranport.Push(Device.Serial, new DeviceInfo()
                        {
                            IsRunAirconditioner = Device.Status.BasicStatus.AirMachine,
                            IsOpenDoor = Device.Status.BasicStatus.Door,
                            IsOpenKey = Device.Status.BasicStatus.Machine,
                            NewLat = Device.Status.BasicStatus.GpsInfo.Lat,
                            NewLng = Device.Status.BasicStatus.GpsInfo.Lng,
                            OldLat = Device.Status.BasicStatus.GpsInfo.Lat,
                            OldLong = Device.Status.BasicStatus.GpsInfo.Lng,
                            SerialDriver = Device.Status.DriverStatus?.Gplx,
                            Speed = Device.Status.BasicStatus.Speed,
                            TaxiNumber = Device.Bs,
                            TimeUpdateInClient = Device.Status.BasicStatus.ClientSend
                        },
                        Device.Temp
                        );

                        _realtimeDeviceSatus.UpdateDeviceSatatus(new StatusDeviceTranfer
                        {
                            Bs = Device.Bs,
                            AirMachine = Device.Status.BasicStatus.AirMachine,
                            LostSingnal = (DateTime.Now - Device.Status.BasicStatus.ClientSend).TotalMinutes > 90,
                            Door = Device.Status.BasicStatus.Door,
                            Fuel = Device.Status.BasicStatus.Fuel,
                            Gplx = Device.Status.DriverStatus.Gplx,
                            Gps = Device.Status.BasicStatus.GpsStatus,
                            Gsm = Device.Status.BasicStatus.GsmSignal,
                            Machine = Device.Status.BasicStatus.Machine,
                            Name = Device.Status.DriverStatus.Name,
                            OverTime = Device.Status.DriverStatus.TimeWork,
                            OverTimeInDay = Device.Status.DriverStatus.TimeWorkInDay,
                            OverSpeedCount = Device.Status.DriverStatus.OverSpeedCount,
                            TotalGpsDistance = Device.Status.BasicStatus.TotalGpsDistance,
                            TotalCurrentGpsDistance = Device.Status.BasicStatus.TotalCurrentGpsDistance,
                            KmOnDay =
                                (int)(Device.Status.BasicStatus.TotalGpsDistance - Device.Status.LastTotalKmUsingOnDay),
                            Serial = Device.Serial,
                            Speed = Device.Status.BasicStatus.Speed,
                            Power = Device.Status.BasicStatus.Power,
                            Time = Device.Status.BasicStatus.ClientSend,
                            Point = new GpsPoint
                            {
                                Lat = Device.Status.BasicStatus.GpsInfo?.Lat ?? 0,
                                Lng = Device.Status.BasicStatus.GpsInfo?.Lng ?? 0,
                                Address = Device.Status.BasicStatus.GpsInfo?.Address
                            },
                            PauseCount = Device.Status.PauseCount,
                            Model = Device.ModelName,
                            GroupId = Device.GroupId,
                            EndTime = Device.EndTime,

                            //useGuest = Device.Temp?.HasGuestSensor??false,
                            useGuest = Device.DeviceType == DeviceType.TaxiVehicle,
                            Guest = Device.Status.BasicStatus.UseTemperature,

                            PauseTime = Device.Temp?.TimePause ?? DateTimeFix.Min,

                            CameraId = Device.CameraId

                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Exception("PACKET", ex, "UpdateStatus ");
                    }
                });
        }

        const String FWT18_V1_1 = "FWT18_V1.1";
        static TimeSpan DAYTIMESPAN = TimeSpan.FromDays(1);

        /// <summary>
        ///     xử lý thông tin gói tin đồng bộ 01, bắt tay và bõ qua trùng lặp
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public byte[] Sync(P01SyncPacket packet)
        {
            try
            {
                BuildRawLog(packet.OriginalData, packet.Time, "Sync");

                //Log.Debug("PACKET", "Nhận được thông tin xử lý gói tin đồng bộ 01");

                
                //2019-11-01 : loi thiet bi moi cua KIET 'FWT18_V1.1'=[FirmWareVersion] , thoi gian bi doi qua 24h
                if ((packet.Time - DateTime.Now).TotalHours > 1)
                {
                    if (FWT18_V1_1 == (Device?.SetupInfo?.FirmWareVersion ?? "").Trim())
                    {
                        DateTime correcttime = packet.Time.Subtract(DAYTIMESPAN);
                        if (Device.Status != null && Device.Status.BasicStatus != null)
                        {
                            if(Device.Status.BasicStatus.ClientSend > DateTime.Now
                                || (correcttime - Device.Status.BasicStatus.ClientSend).TotalSeconds > 0
                                )
                            packet.Time = correcttime;
                        }
                        else
                            Log.Info("Sync", $"correcttime {correcttime} ClientSend {Device.Status.BasicStatus.ClientSend}");

                   }

                    //Bõ qua gói tin có thời gian không hợp lý (lỗi thiết bị cùi bắp)
                    //if ((packet.Time - DateTime.Now).TotalHours > 24)
                    if ((packet.Time - DateTime.Now).TotalHours > 2)
                    {
                        Log.Debug("PACKET", $"Thời gian không hợp lý : Serial {Device.Serial} Time: {packet.Time}");
                        return new byte[] { };
                    }

                }

                ////Bõ qua gói tin có thời gian không hợp lý (lỗi thiết bị cùi bắp)
                //if ((packet.Time - DateTime.Now).TotalHours > 24)
                //{
                //    Log.Debug("PACKET", $"Thời gian không hợp lý : Serial {Device.Serial} Time: {packet.Time}");
                //    return new byte[] { };
                //}

                ////Kiểm tra gói tin trùng theo thời gian
                //if (Device.Status != null
                //    && Device.Status.BasicStatus != null
                //    && (packet.Time - Device.Status.BasicStatus.ClientSend).TotalSeconds < 3 // thời gian nhỏ nhất giữa 2 gói tin
                //                                                                             //&& (Device.Status.BasicStatus.ClientSend - DateTime.Now).TotalHours <= 24 //chỉ so sánh khi thời gian của gói tin trước đó không quá 24 giờ kể từ hiện tại
                //    )
                //{
                //    //Log.Debug("PACKET", $"...trùng {Device.Serial} vs {(packet.Time - Device.Status.BasicStatus.ClientSend).TotalSeconds}");
                //    Device.Temp.SyncDuplicate++;
                //    return new byte[] { };
                //}


                //Kiểm tra gói tin trùng theo thời gian
                //2020-01-17 SUA THEO YEU CAU CUA THAI, VI` TIME CUA 1 GOI TIN NAO DO BI SAI > NOW NEN TOAN BO CAC GOI TIN SAU DO BI BO~ QUA
                if (Device.Status != null
                    && Device.Status.BasicStatus != null)
                {

                    //Kiểm tra gói tin trùng theo thời gian
                    if (Device.Status.BasicStatus.ClientSend <= DateTime.Now 
                        && (packet.Time - Device.Status.BasicStatus.ClientSend).TotalSeconds < 3) // thời gian nhỏ nhất giữa 2 gói tin
                    {
                        //Log.Debug("PACKET", $"...trùng {Device.Serial} vs {(packet.Time - Device.Status.BasicStatus.ClientSend).TotalSeconds}");
                        Device.Temp.SyncDuplicate++;
                        return new byte[] { };
                    }



                    //CHI kiem tra GPS  bi sai trong pham vi N PHUT
                    //if(packet.GpsStatus && (packet.Time - Device.Status.BasicStatus.ClientSend).TotalMinutes <=5 )
                    if (packet.GpsStatus && (packet.Time - Device.Status.BasicStatus.ClientSend).TotalMinutes <= 3)
                    {
                        //co gang han che su dung GeoUtil.Distance
                        //GeoUtil.Distance(packet.GpsInfo.Lat, packet.GpsInfo.Lng, Device.Status.BasicStatus.GpsInfo.Lat, Device.Status.BasicStatus.GpsInfo.Lng)
                        //if ( Math.Abs(packet.GpsInfo.Lat - Device.Status.BasicStatus.GpsInfo.Lat) >=1
                        //    || Math.Abs(packet.GpsInfo.Lng - Device.Status.BasicStatus.GpsInfo.Lng) >=1 )
                        //{
                        //    Log.Debug("PACKET", $"Lỗi GPS1 : Serial {Device.Serial} {packet.GpsInfo} Time: {packet.Time}");
                        //    return new byte[] { };
                        //}


                        //lat & long 0.3 <=> 33 km trong 3 phut, tuc la toc do toi da cho phep la tam 600-700KM/h
                        if (Math.Abs(packet.GpsInfo.Lat - Device.Status.BasicStatus.GpsInfo.Lat) >= 0.3
                                || Math.Abs(packet.GpsInfo.Lng - Device.Status.BasicStatus.GpsInfo.Lng) >= 0.3)
                        {
                            Log.Debug("PACKET", $"Lỗi GPS2 : Serial {Device.Serial} {packet.GpsInfo} Time: {packet.Time}");
                            return new byte[] { };
                        }
                    }


                }

                //kiem tra GPS  =0 // rat kho xay ra vi tri nay vi da bi kiem tra o tren kia 
                if (packet.GpsStatus && packet.GpsInfo.Lat == 0f && packet.GpsInfo.Lng == 0f)
                {
                    Log.Debug("PACKET", $"Lỗi GPS=0: Serial {Device.Serial} Time: {packet.Time}");
                    return new byte[] { };
                }
                //if (packet.GpsStatus && !packet.GpsInfo.IsValid())
                //{
                //    Log.Debug("PACKET", $"Lỗi GPS : Serial {Device.Serial} {packet.GpsInfo} Time: {packet.Time}");
                //    return new byte[] { };
                //}

                return SyncBase(packet);
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "Sync");
                return new byte[] { };
            }
        }

        private byte[] Sync301(P01SyncPacket packet, bool compressPacket)
        {
            try
            {
                BuildRawLog(packet.OriginalData, packet.Time, "Sync301");

                //Bõ qua gói tin có thời gian không hợp lý (lỗi thiết bị cùi bắp)
                //if ((packet.Time - DateTime.Now).TotalHours > 24)
                if ((packet.Time - DateTime.Now).TotalHours > 2)
                {
                    Log.Debug("PACKET", $"Thời gian không hợp lý : Serial {Device.Serial} Time: {packet.Time}");
                    return new byte[] { };
                }

                //Kiểm tra gói tin trùng hoàn toàn theo thời gian
                if (Device.Status != null
                    && Device.Status.BasicStatus != null
                    && packet.Time == Device.Status.BasicStatus.ClientSend // thời gian trùng hoàn toàn
                                                                           //&& (Device.Status.BasicStatus.ClientSend - DateTime.Now).TotalHours <= 24 //chỉ so sánh khi thời gian của gói tin trước đó không quá 24 giờ kể từ hiện tại
                    )
                {
                    //Log.Debug("PACKET", $"...trùng {Device.Serial} vs {(packet.Time - Device.Status.BasicStatus.ClientSend).TotalSeconds}");
                    Device.Temp.SyncDuplicate++;
                    return new byte[] { };
                }

                return SyncBase(packet, compressPacket);
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, $"Sync301 {compressPacket}");
                return new byte[] { };
            }
        }

        /// <summary>
        ///     xử lý thông tin gói tin đồng bộ 01 nằm bên trong gói nén, bắt tay và bõ qua trùng lặp
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public byte[] Sync301(P01SyncPacket packet)
        {
            return Sync301(packet, true);
        }

        /// <summary>
        ///     xử lý thông tin gói tin đồng bộ 01 nằm bên trong gói nén, bắt tay và bõ qua trùng lặp
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public byte[] Sync301last(P01SyncPacket packet)
        {
            return Sync301(packet,false);
        }

        /// <summary>
        ///     xử lý thông tin gói tin đồng bộ 10, không băt tay (CHƯA SỬ DỤNG)
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public byte[] Sync10(P10SyncPacket packet)
        {
            Log.Debug("PACKET", "Nhận được thông tin xử lý gói tin đồng bộ 10");

            if (Device.Status != null
                && Device.Status.BasicStatus != null
                    //&& Device.Status.BasicStatus.ClientSend >= packet.Time
                    && (packet.Time - Device.Status.BasicStatus.ClientSend).TotalSeconds < 3
                    )
                Log.Debug("PACKET", $"time :  {packet.Time}");

            return SyncBase(packet);
        }

        private void TramForwardSync(PBaseSyncPacket packet, bool compressPacket = false)
        {
            if (!Models.ResponseDataConfig.UseTramForwardUrl) return;
            if (Company == null) return;
            //forward toi xe dien
            if (Company.Type == 1 && !compressPacket)
            {
                try
                {
                    String opcode = compressPacket ? "301" : "01";
                    var fw = new ForwardApi();
                    fw.AddHeader("serial", packet.Serial.ToString());
                    fw.AddHeader("opcode", opcode);
                    var url = $"{Models.ResponseDataConfig.TramForwardSyncUrl}?serial={packet.Serial}&opcode={opcode}&guest={Device?.Status?.BasicStatus?.UseTemperature??false}&machine={Device?.Status?.BasicStatus?.Machine ?? false}";
                    Log.Debug("TRAM", $"URL request sync : {url}");
                    fw.PostWithoutResult<byte[]>(url, packet);
                }
                catch (Exception ex)
                {
                    Log.Exception("TRAM", ex, $"forward sync packet fail {packet.Serial}");
                }
            }
        }

        private void TramForwardEvent(BaseEvent packet, string opcode)
        {
            if (!Models.ResponseDataConfig.UseTramForwardUrl) return;
            if (Company == null) return;
            //forward toi xe dien
            if (Company.Type == 1)
            {
                try
                {
                    var fw = new ForwardApi();
                    fw.AddHeader("serial", packet.Serial.ToString());
                    fw.AddHeader("opcode", opcode);
                    var url = $"{Models.ResponseDataConfig.TramForwardEventUrl}?serial={packet.Serial}&opcode={opcode}";
                    Log.Debug("TRAM", $"URL request event : {url}");
                    fw.PostWithoutResult<byte[]>(url, packet);
                }
                catch (Exception ex)
                {
                    Log.Exception("TRAM", ex, $"forward sync event fail {packet.Serial}");
                }
            }
        }


        private byte[] SyncBase(PBaseSyncPacket packet, bool compressPacket = false)
        {
            //move to before filter 
            //BuildRawLog(packet.OriginalData, packet.Time, "Sync");

            //xử lý logic
            var logicName = "";
            foreach (var handle in gLogicHandleType.Select(t => Activator.CreateInstance(t) as ILogic))
            {
                try
                {
                    logicName = handle?.GetType().Name;
                    var utils = new LogicUtils
                    {
                        DataCache = Cache,
                        DataContext = DataContext,
                        Log = Log,
                        MotherSqlId = MotherSqlId,
                        LocationQuery = _locationQuery,
                        BgtTranport = _bgtTranport,
                        CacheLog = _cacheLog,
                        PacketType = compressPacket ? (byte)1 : (byte)0
                    };

                    handle?.Handle(packet, utils, Device, Company);
                }
                catch (Exception e)
                {
                    Log.Exception("DeviceController", e,
                        $"Logic {logicName} xử lý lỗi {packet.Serial} ");

                    Log.Info("PACKET", Device.Status != null ? "Device.Status OK" : "Device.Status NULL");
                }
            }

            //Xe công trình, kiểm tra và xử lý sự kiện nếu chưa có
            if ( (Device.DeviceType == DeviceType.ConstructionVehicle || Device.DeviceType == DeviceType.Dynamo) // xe công trình , may phat dien
                && Device.Status.BasicStatus.Machine  //máy đang mở
                && !Device.Temp.HasMachineOn //chưa có trace
                )
            {
                BuildBeginTraceLog(new BaseEvent() { TimeUpdate = packet.Time, GpsInfo = packet.GpsInfo }, TraceType.Machine);
            }

            //xe TAXI
            if(Device.DeviceType == DeviceType.TaxiVehicle && packet.GpsInfo.Speed>0)
            {
                //Cap nhat lai su kien don khach khi xe chay(yeu cau cua THAI)
                var trace = Device.Temp.GetTrace(TraceType.HasGuest);
                if (trace != null && trace.Note==null)
                {
                    trace.BeginTime = packet.Time;//cap nhat thoi gian
                    trace.Note = "";
                    //trace.BeginLocation = new GpsLocation { Lat = packet.GpsInfo.Lat, Lng = packet.GpsInfo.Lng };//cap nhat vi tri
                }
            }

            //Nếu là gói nén thì kết thúc tại đây để tiết kiệm tài nguyên
            if (compressPacket) return new byte[] { };

            //forward toi xe dien
            TramForwardSync(packet, compressPacket);

            //cập nhật status
            UpdateStatus();

            //chỉ cập nhật định kì sau 300 giây ~ 5 phút (chưa sử dụng query trực tiếp từ db nên làm cách này đc)
            //Tương lai có thể bõ luôn phần này và tự lưu theo từng phút (ngoại trừ khi đổi tài)
            if (Device.Status != null)
            {
                if ((packet.Time - Device.Temp.DeviceStatusUpdateTime).TotalSeconds > 300)
                {
                    Device.Temp.DeviceStatusUpdateTime = packet.Time;
                    DataContext.Update(Device.Status, MotherSqlId);
                    DataContext.Commit(MotherSqlId);
                }
            }

            //Send setup lấy yêu cầu lệnh từ db để gửi : lấy từng cái ra gửi !!!! cần xử lý lại chỗ này
            var setup =
                DataContext.CreateQuery<DeviceSetupRequest>(MotherSqlId)
                    .Where(m => m.Serial == Device.Serial && !m.Complete)
                    .Take(1)
                    .Execute()
                    .FirstOrDefault();
            if (setup != null)
            {
                //Log.Info("DeviceSetupRequest", $"Serial {Device.Serial} Note={setup.Note}");

                setup.Complete = true;
                setup.Response = DateTime.Now;
                DataContext.Update(setup, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                return setup.Data;
            }

            byte[] ret = null;

            //Gửi lệnh lấy thông tin sau lệnh cấu hình tự động
            if (Device != null && Device.SetupInfo != null)
            {
                if (Device.SetupInfo.RequestInforCommand != null)
                {
                    ret = Device.SetupInfo.RequestInforCommand;
                    Device.SetupInfo.RequestInforCommand = null;
                }
            }
            if (ret != null) return ret;

            //Kiểm tra thay đổi vận tốc tối đa dựa theo tọa độ và thông số đã cấu hình
            ret = DetectOverSpeed(packet);
            if (ret != null) return ret;

            return new byte[] { };
        }


        /// <summary>
        ///     cập nhật raw log vào cache
        /// </summary>
        /// <param name="data"></param>
        /// <param name="time"></param>
        /// <param name="note"></param>
        private void BuildRawLog(byte[] data, DateTime time, string note)
        {
            if (!Cache.ContainRawLogSerial(Device.Serial)) return;

            _cacheLog.PushRawLog(new DeviceRawLog
            {
                ClientSend = time,
                ServerRecv = DateTime.Now,
                Data = data,
                DbId = Company.DbId,
                Indentity = Device.Indentity,
                Id = 0,
                Note = note,
                Serial = Device.Serial
            });
        }

        private bool BuildRawLogWithCheck(byte[] data, DateTime time, string note)
        {
            BuildRawLog(data, time, note);
            if (Device.Status == null)
            {
                Log.Error("PACKET", $"{note} Status is null : Serial {Device.Serial}");
                return false;
            }
            return true;
        }

        /// <summary>
        ///     tạo 1 cache trace log trên memory
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool BuildBeginTraceLog(BaseEvent packet, TraceType type)
        {
            if (Device.Temp.ExistTrace(type))
            {
                ////Với xe công trình thì xét theo sự kiện mở máy sau cùng
                //if(Device.DeviceType == DeviceType.ConstructionVehicle && type == TraceType.Machine)
                //    Device.Temp.EndTrace(type);

                ////nếu không phải thì theo sự kiện đầu tiên
                //else
                Device.Temp.EventWithoutEnd++;
                return false;
            }

            #region Tạm thời sửa lỗi thời gian sai
            if (!packet.TimeUpdate.IsValidDatetime())
                packet.TimeUpdate = Device.Status.BasicStatus.ClientSend;
            #endregion Tạm thời sửa lỗi thời gian sai

            var trace = new DeviceTraceLog
            {
                CompanyId = Company.Id,
                Serial = Device.Serial,
                Indentity = Device.Indentity,
                BeginLocation = new GpsLocation { Lat = packet.GpsInfo.Lat, Lng = packet.GpsInfo.Lng },
                BeginTime = packet.TimeUpdate,
                DbId = Company.DbId,
                DriverId = Driver?.Id ?? 0,
                GroupId = Group?.Id ?? 0,
                Type = type,
                Distance = Device.Status.BasicStatus.TotalGpsDistance
            };
            if (type == TraceType.Machine)
                trace.DriverTime = packet.TimeUpdate;

            return Device.Temp.BeginTrace(trace);
        }

        /// <summary>
        /// lưu trữ thông tin trace log sau khi kết thúc sự kiện,
        /// Mặc định 50 met giới hạn cho cùng địa chỉ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        private bool BuildEndTraceLog(TraceType type, BaseEvent packet)
        {
            return BuildEndTraceLog(type, packet, 50, 0, 0);
        }

        /// <summary>
        ///     lưu trữ thông tin trace log sau khi kết thúc sự kiện
        /// </summary>
        /// <param name="type"></param>
        /// <param name="packet"></param>
        /// <param name="limitDistance4EndAddress">khoảng cách tối thiểu để lấy địa chỉ kết thúc theo tọa độ bắt đầu, không xử lý nếu = 0</param>
        /// <param name="limitTimePeriod">thời gian tối thiểu để xử lý theo giây, không kiểm tra nếu = 0 </param>
        /// <param name="limitDistance">khoảng cách tối thiểu xử lý theo mét, không kiểm tra nếu = 0</param>
        /// <returns></returns>
        private bool BuildEndTraceLog(TraceType type, BaseEvent packet, double limitDistance4EndAddress, double limitTimePeriod, double limitDistance)
        {
            var trace = Device.Temp.EndTrace(type);
            if (trace == null)
            {
                //Log.Debug("PACKET", $"Không tìm ra cuốc {Device.Serial}");
                Device.Temp.EventWithoutStart++;
                return false;
            }

            if (packet.TimeUpdate < trace.BeginTime)
            {
                Log.Debug("PACKET", $"Thời gian sai {Device.Serial} {trace.BeginTime} {packet.TimeUpdate} ");
                return false;
            }

            //thời gian sự kiện ngắn hơn limitTimePeriod, bõ qua
            trace.EndTime = packet.TimeUpdate;

            TimeSpan tracetime = trace.EndTime - trace.BeginTime;
            if (limitTimePeriod > 0 && tracetime.TotalSeconds < limitTimePeriod)
            {
                return false;// true;
            }

            //khoảng cách ngắn hơn limitDistance, bõ qua
            if (limitDistance > 0 && (Device.Status.BasicStatus.TotalGpsDistance - trace.Distance) < limitDistance)
            {
                return false;// true;
            }

            //Lấy địa chỉ begin location
            _locationQuery.GetAddress(trace.BeginLocation);

            trace.EndLocation = new GpsLocation { Lat = packet.GpsInfo.Lat, Lng = packet.GpsInfo.Lng };

            //tính khoảng cách theo thông tin thiết bị gửi lên
            trace.Distance = Device.Status.BasicStatus.TotalGpsDistance - trace.Distance;

            //Bõ qua cuốc xe không di chuyển
            if (trace.Type == TraceType.Machine)
            {
                //Xe công trình thì km=0 vẫn tính cuốc
                //String loaixe = Device.ModelName ?? "";
                if (trace.Distance <= 0 && Device.DeviceType != DeviceType.ConstructionVehicle && Device.DeviceType != DeviceType.Dynamo)
                {
                    return false;//true;
                }
            }
            if (trace.Type == TraceType.HasGuest
                || trace.Type == TraceType.NoGuest
                )
            {
                if (trace.Distance <= 0)
                {
                    return false;//true;
                }
            }

            //Nếu như vị trí kết thúc gần với vị trí ban đầu thì lấy địa chỉ ban đầu
            if (limitDistance4EndAddress > 0
                && trace.BeginLocation != null
                && GeoUtil.Distance(trace.BeginLocation.Lat, trace.BeginLocation.Lng
                    , trace.EndLocation.Lat, trace.EndLocation.Lng) < limitDistance4EndAddress)
                trace.EndLocation.Address = trace.BeginLocation.Address;
            else
                _locationQuery.GetAddress(trace.EndLocation);

            if (trace.Type == TraceType.Machine)
            {
                trace.Note = tracetime.ToString("g");

                //Ghi lại thời gian chạy dựa theo dự kiện mở máy
                Device.Temp.MachineSeconds += (int)Math.Round(tracetime.TotalSeconds);

                //if (Driver != null)
                //{
                //    var dr = new DriverTraceSessionLog
                //    {
                //        BeginTime = trace.BeginTime,
                //        BeginLocation = trace.BeginLocation,
                //        DbId = trace.DbId,
                //        Distance = trace.Distance,
                //        DriverId = Driver.Id,
                //        EndTime = trace.EndTime,
                //        EndLocation = trace.EndLocation,
                //        Id = 0,
                //        OverTime = trace.EndTime - trace.DriverTime
                //    };
                //    DataContext.Insert(dr, Company.DbId);
                //    DataContext.Commit(Company.DbId);
                //}
            }


            if (trace.Type == TraceType.HasGuest)
            {
                if (Device.Temp.GeneralGuestLog != null)
                {
                    Device.Temp.GeneralGuestLog.KmGuestOnDay += (int)trace.Distance;
                    Device.Temp.GeneralGuestLog.GuestTimeInday += (int)Math.Round((trace.EndTime - trace.BeginTime).TotalSeconds);
                }
            }

            if (trace.Type == TraceType.NoGuest)
            {
                if (Device.Temp.GeneralGuestLog != null)
                {
                    Device.Temp.GeneralGuestLog.KmNoGuestOnDay += (int)trace.Distance;
                    Device.Temp.GeneralGuestLog.NoGuestTimeInday += (int)Math.Round((trace.EndTime - trace.BeginTime).TotalSeconds);
                }
            }

            if (trace.Type == TraceType.OverSpeed)
            {
                var overSpeed = new DeviceOverSpeedLog()
                {
                    BeginTime = trace.BeginTime,
                    CompanyId = Company.Id,
                    DbId = Company.DbId,
                    Distance = (int)trace.Distance,
                    DriverId = Driver?.Id ?? 0,
                    EndTime = trace.EndTime,
                    GroupId = Device.GroupId,
                    Id = 0,
                    Indentity = Device.Indentity,
                    LimitSpeed = Device.SetupInfo?.OverSpeed ?? Core.DeviceLogicHandles.Logics.OverSpeed09Logic.DEFAULT_SPEED,
                    MaxSpeed = Device.Temp.MaxSpeed,
                    Point = trace.BeginLocation,
                    Serial = Device.Serial
                };
                DataContext.Insert(overSpeed, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                Device.Temp.MaxSpeed = 0;
            }
            else
            {
                _cacheLog.PushTraceLog(trace);
            }

            Log.Debug("PACKET", $"Save Trace {type} : {Device.Serial}");

            return true;
        }


        /// <summary>
        ///     Thông tin lái xe liên tục của tái xế
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        [HttpPost]
        public bool EndOverTime(P114EndOvertime packet)
        {
            Log.Debug("PACKET", $"Nhận được gói tin báo kết thúc lái xe liên tục {Device.Serial} tài xế {packet.DriverId}");

            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.EndTime, "Cuốc liên tục 114")) return false;

                //Bõ qua gói tin có thời gian không hợp lý (lỗi thiết bị cùi bắp)
                if ((packet.BeginTime - DateTime.Now).TotalHours > 24)
                {
                    Log.Error("PACKET", $"EndOverTime thời gian sai : Serial {Device.Serial} Time: {packet.BeginTime} - {packet.EndTime}");
                    return false;
                }

                //Kiểm tra gói tin trùng theo thời gian, do thiết bị phát trùng
                if (Device.Temp.LastEndOverTime == packet.BeginTime)
                {
                    Log.Debug("PACKET", $"EndOverTime...trùng {Device.Serial}");
                    return false;
                }
                Device.Temp.LastEndOverTime = packet.BeginTime;


                //vẫn ghi nhận thông tin không có tài xế và khoảng cách bằng 0

                //if (packet.DriverId <= 0)
                //    return false;

                ////bõ qua khoảng cách = 0
                //if (packet.Distance <= 0)
                //    return false;

                //bõ qua thời gian bị sai
                if (!Model.Utils.DateTimeFix.IsValidDatetime(packet.BeginTime))
                {
                    Log.Error("PACKET", $"EndOverTime sai BeginTime {Device.Serial} {packet.BeginTime}");
                    return false;
                }

                //bõ qua tọa độ GpsBegin = 0 , do có 1 số xe quá nhiều
                if (packet.GpsBegin.Lat == 0f && packet.GpsBegin.Lng == 0f)
                {
                    Log.Error("PACKET", $"EndOverTime sai GpsBegin {Device.Serial}");
                    return false;
                }

                //kiểm tra tài xế thay đổi chưa ( thêm vô để kiểm tra 1 tg thôi)
                if (Driver != null && Driver.Id != packet.DriverId)
                {
                    Log.Error("PACKET", $"EndOverTime Driver# {Device.Serial} server {Driver.Id} tb {packet.DriverId}");
                }

                //tao log
                var dr = new DriverTraceSessionLog
                {
                    BeginTime = packet.BeginTime,
                    BeginLocation = new GpsLocation() { Lat = packet.GpsBegin.Lat, Lng = packet.GpsBegin.Lng },
                    DbId = Company.DbId,
                    Distance = packet.Distance,
                    DriverId = packet.DriverId,
                    EndTime = packet.EndTime,
                    Indentity = Device.Indentity,
                    EndLocation = new GpsLocation() { Lat = packet.GpsEnd.Lat, Lng = packet.GpsEnd.Lng },
                    Id = 0,
                    OverTime = packet.EndTime - packet.BeginTime,
                    Serial = Device.Serial // luu vo ngay 2017-04-05
                    , CompanyId = Device.CompanyId // luu vo ngay 2017-12-25
                };

                //Lấy địa chỉ
                _locationQuery.GetAddress(dr.BeginLocation);

                //Nếu như vị trí kết thúc gần với vị trí ban đầu (< 50m)thì lấy địa chỉ ban đầu
                if (GeoUtil.Distance(dr.BeginLocation.Lat, dr.BeginLocation.Lng
                        , dr.EndLocation.Lat, dr.EndLocation.Lng) < 50)
                    dr.EndLocation.Address = dr.BeginLocation.Address;
                else
                    _locationQuery.GetAddress(dr.EndLocation);

                //Xử lý kiểm tra quá 4 giờ
                Process4HOverTime(dr);

                //Xử lý kiểm tra quá 10 giờ theo ngày
                Process10HOverTime(dr);

                DataContext.Insert(dr, Company.DbId);
                DataContext.Commit();

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, $"EndOverTime {packet.Serial}");
                return false;
            }
        }

        private void Process4HOverTime(DriverTraceSessionLog dr)
        {
            //Cập nhật biến đếm thể hiện ta tình trạng hiện tại trên map
            if (dr.OverTime.TotalHours > 4 
                && dr.OverTime.TotalHours <= 8 //a Phi bổ sung ngày 2018-11-13
                )
                Device.Temp.OverTime4HCount++;

            //cập nhật xử lý gửi email nếu cuốc chạy liên tục quá 4 giờ
            if (Device.EmailAlarm && !String.IsNullOrWhiteSpace(Device.EmailAddess))
            {
                if (dr.OverTime.TotalHours > 4
                    && dr.OverTime.TotalHours <= 8 //a Phi bổ sung ngày 2018-11-13
                    )
                    _cacheLog.PushOver4HAlarmEmail(
                        Device,
                        Cache.GetQueryContext<Driver>().GetByKey(dr.DriverId),
                        dr.EndLocation.Lat, dr.EndLocation.Lng
                        );
            }
        }

        /// <summary>
        /// Chỉ xử lý cho cuốc xuyên qua 1 đêm
        /// </summary>
        /// <param name="dr"></param>
        private void Process10HOverTime(DriverTraceSessionLog dr)
        {
            //cuốc trước 0 giờ
            DriverTraceDaily10HLog dr10first = null;
            //cuốc sau 0 giờ ( nếu có )
            DriverTraceDaily10HLog dr10last = null;

            #region  Thêm và kiểm tra cho gói quá 10h

            //tổng time cuốc đầu
            TimeSpan totaltime0 = dr.EndTime - dr.BeginTime;
            //tồng sau cuốc sau ( nếu có )
            TimeSpan totaltime1 = TimeSpan.MinValue;

            //kiểm tra tách cuốc lúc 0 giờ
            if (dr.BeginTime.Date < dr.EndTime.Date)
            {
                //cuốc đầu
                totaltime0 = dr.EndTime.Date - dr.BeginTime;
                //cuốc sau
                totaltime1 = dr.EndTime - dr.EndTime.Date;
            }

            //cập nhật thêm cuốc trước đó (nếu có)
            if (Device.Temp.Last10hTrace != null)
            {
                if (Device.Temp.Last10hTrace.BeginTime.Date == dr.BeginTime.Date) //chỉ xét khi trùng ngày
                    totaltime0 = totaltime0.Add(TimeSpan.FromSeconds(Device.Temp.Last10hTrace.TotalSeconds));
                else
                    Device.Temp.Last10hTrace = null;//over new date, reset it
            }

            //xử lý cuốc đầu : dr10first
            if (totaltime0.TotalHours > 10)
            {
                dr10first = new DriverTraceDaily10HLog
                {
                    DbId = Company.DbId,
                    DriverId = dr.DriverId,
                    Indentity = Device.Indentity,
                    Id = 0,
                    Serial = Device.Serial,
                    CompanyId = Device.CompanyId,
                    OverTime = totaltime0
                };

                //nếu có cuốc trước đó
                if (Device.Temp.Last10hTrace != null)
                {
                    dr10first.BeginTime = Device.Temp.Last10hTrace.BeginTime;
                    dr10first.BeginLocation = new GpsLocation() { Lat = Device.Temp.Last10hTrace.Lat, Lng = Device.Temp.Last10hTrace.Lng };
                }
                else
                {
                    dr10first.BeginTime = dr.BeginTime;
                    dr10first.BeginLocation = dr.BeginLocation;
                }

                //nếu không qua 0 giờ
                if (totaltime1 == TimeSpan.MinValue)
                {
                    dr10first.EndTime = dr.EndTime;
                    dr10first.EndLocation = dr.EndLocation;
                }
                else
                {
                    dr10first.EndTime = dr.EndTime.Date.Subtract(TimeSpan.FromSeconds(1));
                    dr10first.EndLocation = Device.Temp.MidnightLocation;
                }
            }


            //Nếu có tách cuốc
            if (totaltime1 > TimeSpan.MinValue)
            {
                //nếu cuốc này quá 10 , ghi nhận luôn cuốc sau : dr10last
                if (totaltime1.TotalHours > 10)
                {
                    dr10last = new DriverTraceDaily10HLog
                    {
                        DbId = Company.DbId,
                        DriverId = dr.DriverId,
                        Indentity = Device.Indentity,
                        Id = 0,
                        Serial = Device.Serial,
                        CompanyId = Device.CompanyId,
                        BeginTime = dr.EndTime.Date,
                        BeginLocation = Device.Temp.MidnightLocation,
                        EndTime = dr.EndTime,
                        EndLocation = dr.EndLocation,
                        OverTime = totaltime1
                    };

                    //reset lại cuốc chờ
                    Device.Temp.Last10hTrace = null;
                }
                //tồn tại tách cuốc nhưng chưa quá 10h --> ghi nhận thành cuốc chờ mới
                else
                {
                    Device.Temp.Last10hTrace = new Daily10HTemp()
                    {
                        BeginTime = dr.EndTime.Date,
                        Lat = Device.Temp.MidnightLocation.Lat,
                        Lng = Device.Temp.MidnightLocation.Lng,
                        TotalSeconds = (int)totaltime1.TotalSeconds
                    };
                }
            }
            //nếu k có tách
            else
            {
                //nếu có cuốc quá 10h --> reset lại cuốc chờ
                if (dr10first != null)
                    Device.Temp.Last10hTrace = null;
                else //chưa quá thì ghi nhận cuốc chờ
                {
                    if (Device.Temp.Last10hTrace != null)
                        Device.Temp.Last10hTrace.TotalSeconds += (int)dr.OverTime.TotalSeconds;
                    else
                        Device.Temp.Last10hTrace = new Daily10HTemp()
                        {
                            BeginTime = dr.BeginTime,
                            Lat = dr.BeginLocation.Lat,
                            Lng = dr.BeginLocation.Lng,
                            TotalSeconds = (int)dr.OverTime.TotalSeconds
                        };
                }

            }


            #endregion  Thêm và kiểm tra cho gói quá 10h

            //lưu db
            if (dr10first != null)
            {
                if (dr10first.BeginLocation.IsEmptyAddress)
                    _locationQuery.GetAddress(dr10first.BeginLocation);

                //Nếu như vị trí kết thúc gần với vị trí ban đầu (< 50m)thì lấy địa chỉ ban đầu
                if (dr10first.EndLocation.IsEmptyAddress)
                {
                    if (GeoUtil.Distance(dr10first.BeginLocation.Lat, dr10first.BeginLocation.Lng
                                            , dr10first.EndLocation.Lat, dr10first.EndLocation.Lng) < 50)
                        dr10first.EndLocation.Address = dr10first.BeginLocation.Address;
                    else
                        _locationQuery.GetAddress(dr10first.EndLocation);
                }

                DataContext.Insert(dr10first, Company.DbId);
            }

            //lưu db
            if (dr10last != null)
            {
                if (dr10last.BeginLocation.IsEmptyAddress)
                    _locationQuery.GetAddress(dr10last.BeginLocation);

                //Nếu như vị trí kết thúc gần với vị trí ban đầu (< 50m)thì lấy địa chỉ ban đầu
                if (dr10last.EndLocation.IsEmptyAddress)
                {
                    if (GeoUtil.Distance(dr10last.BeginLocation.Lat, dr10last.BeginLocation.Lng
                                            , dr10last.EndLocation.Lat, dr10last.EndLocation.Lng) < 50)
                        dr10last.EndLocation.Address = dr10last.BeginLocation.Address;
                    else
                        _locationQuery.GetAddress(dr10last.EndLocation);
                }

                DataContext.Insert(dr10last, Company.DbId);
            }


            //cập nhật xử lý gửi email nếu cuốc chạy liên tục quá 10 giờ trong ngày
            if (Device.EmailAlarm && !String.IsNullOrWhiteSpace(Device.EmailAddess))
            {
                if (dr10first != null)
                    _cacheLog.PushOver10HAlarmEmail(
                        Device,
                        Cache.GetQueryContext<Driver>().GetByKey(dr.DriverId),
                        dr10first.EndLocation.Lat, dr10first.EndLocation.Lng
                        );

                if (dr10last != null)
                    _cacheLog.PushOver10HAlarmEmail(
                        Device,
                        Cache.GetQueryContext<Driver>().GetByKey(dr.DriverId),
                        dr10last.EndLocation.Lat, dr10last.EndLocation.Lng
                        );
            }


        }

        /// <summary>
        ///     xửa lý thông tin mở máy
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool OnMachine(P100OnMachine packet)
        {
            Log.Debug("PACKET", $"Nhân được gói tin báo mở chìa khóa {Device.Serial}");
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Mở máy")) return false;

                //kiểm tra và bõ qua sự kiện trùng
                if(!BuildBeginTraceLog(packet, TraceType.Machine))
                {
                    //Log.Debug("PACKET", $"Đã có sự kiện mở máy : Serial {Device.Serial}");
                    return false;
                }

                //Có cảm biến đón khách
                if(Device.DeviceType== DeviceType.TaxiVehicle)
                {
                    ////Có khách
                    //if (Device.Status.BasicStatus.UseTemperature)
                    //{
                    //    if (!BuildBeginTraceLog(packet, TraceType.HasGuest))
                    //        Log.Debug("PACKET", $"Đã có sự kiện có khách : Serial {Device.Serial}");
                    //}
                    //else//không khách
                    if (!Device.Status.BasicStatus.UseTemperature)
                    {
                        if (!BuildBeginTraceLog(packet, TraceType.NoGuest))
                            Log.Debug("PACKET", $"Đã có sự kiện không khách : Serial {Device.Serial}");
                    }
                }

                Device.Status.BasicStatus.Machine = true;
                //Device.Temp.TimeHandleRun = packet.TimeUpdate;
                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "100");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "OnMachine");
                return false;
            }
        }


        /// <summary>
        ///     xửa lý thông tin tắt máy
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool OffMachine(P101OffMachine packet)
        {
            Log.Debug("PACKET", $"Nhân được gói tin báo tắt chìa khóa {Device.Serial}");

            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Tắt máy")) return false;

                Device.Status.BasicStatus.Machine = false;
                //Device.Temp.TimeHandlePause = packet.TimeUpdate;

                //Chỉ xử lý sự kiện khi thời gian bật khóa >= 1 phút và khoảng cách 50m mới tính query địa chỉ kết thúc
                if (!BuildEndTraceLog(TraceType.Machine, packet, 50, 60, 0))
                {
                    UpdateStatus();
                    return false;
                }

                //Có cảm biến đón khách
                if (Device.DeviceType== DeviceType.TaxiVehicle)
                {
                    //tắt máy trong khi xe không có khách, tính là 1 cuộc xe không , hơn 60 giây mới ghi nhận
                    //if (Device.Status.BasicStatus.UseTemperature)
                    //    BuildEndTraceLog(TraceType.HasGuest, packet, 50,60, 10);
                    //else
                    if (!Device.Status.BasicStatus.UseTemperature)
                        BuildEndTraceLog(TraceType.NoGuest, packet, 50, GUEST_MIN_TIME, GUEST_MIN_DISTANCE);
                }

                Device.Temp.OffMachineCount++;
                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "101");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "OffMachine");
                return false;
            }

        }

        /// <summary>
        ///     xử lý sự kiện mở máy lạnh
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool OnAirCondition(P102OnAirCondition packet)
        {
            if (Device.InvertAir)
                return OffAirConditionReal(packet);
            else
                return OnAirConditionReal(packet);
        }

        /// <summary>
        ///     xử lý sự kiện tắt máy lạnh
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool OffAirCondition(P103OffAirCondition packet)
        {
            if (Device.InvertAir)
                return OnAirConditionReal(packet);
            else
                return OffAirConditionReal(packet);
        }

        /// <summary>
        ///     xử lý sự kiện mở máy lạnh thực tế
        /// </summary>
        /// <param name="packet"></param>
        private bool OnAirConditionReal(BaseEvent packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Mở điều hòa")) return false;

                //kiểm tra và bõ qua sự kiện trùng
                if (!BuildBeginTraceLog(packet, TraceType.AirMachine))
                {
                    Log.Debug("PACKET", $"Đã có sự kiện mở máy lạnh : Serial {Device.Serial}");
                    return false;
                }

                Device.Status.BasicStatus.AirMachine = true;
                Device.Temp.AirConditionCount++;

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "102");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "OnAirCondition");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện tắt máy lạnh thực tế
        /// </summary>
        /// <param name="packet"></param>
        private bool OffAirConditionReal(BaseEvent packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "tắt điều hòa")) return false;

                Device.Status.BasicStatus.AirMachine = false;

                //Chỉ xử lý sự kiện khi thời gian bật máy lạnh >= 15 phút và khoảng cách 50m mới tính query địa chỉ kết thúc
                if (!BuildEndTraceLog(TraceType.AirMachine, packet, 50, 900, 0))
                {
                    UpdateStatus();
                    return false;
                }

                Device.Temp.AirConditionCount++;
                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "103");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "OffAirCondition");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện mở cửa
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool OpenDoor(P104OpenDoor packet)
        {
            if (Device.InvertDoor)
                return CloseDoorReal(packet);
            else
                return OpenDoorReal(packet);
        }

        /// <summary>
        ///     xử lý sự kiện đóng cửa
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool CloseDoor(P105CloseDoor packet)
        {
            if (Device.InvertDoor)
                return OpenDoorReal(packet);
            else
                return CloseDoorReal(packet);
        }

        /// <summary>
        ///     xử lý sự kiện mở cửa thực tế
        /// </summary>
        /// <param name="packet"></param>
        private bool OpenDoorReal(BaseEvent packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Mở cửa")) return false;

                //kiểm tra và bõ qua sự kiện trùng
                if (!BuildBeginTraceLog(packet, TraceType.Door))
                {
                    Log.Debug("PACKET", $"Đã có sự kiện mở cửa : Serial {Device.Serial}");
                    return false;
                }

                Device.Status.BasicStatus.Door = true;
                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "104");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "OpenDoor");
                return false;
            }
        }


        /// <summary>
        ///     xử lý sự kiện đóng cửa thực tế
        /// </summary>
        /// <param name="packet"></param>
        private bool CloseDoorReal(BaseEvent packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Đóng cửa")) return false;

                Device.Status.BasicStatus.Door = false;//lẽ ra nằm bên dưới nhưng do sự kiện này k bị trùng nên nằm đâu cũng đc
                if (!BuildEndTraceLog(TraceType.Door, packet))
                {
                    UpdateStatus();
                    return false;
                }

                Device.Temp.OpenDoorCount++;
                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "105");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "CloseDoor");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện bắt đầu quá trình quá vận tốc
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool BeginOverSpeed(P106BeginOverSpeed packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Băt đầu quá vận tốc")) return false;

                //cập nhật tốc độ trong sự kiện
                if (Device.Temp.MaxSpeed < packet.GpsInfo.Speed) Device.Temp.MaxSpeed = packet.GpsInfo.Speed;
                Device.Status.BasicStatus.Speed = packet.GpsInfo.Speed;

                //kiểm tra và bõ qua sự kiện trùng
                if (!BuildBeginTraceLog(packet, TraceType.OverSpeed))
                {
                    Log.Debug("PACKET", $"Đã có sự kiện quá vận tốc : Serial {Device.Serial}");
                    return false;
                }

                ////cập nhật xử lý gửi email nếu có ( cái này là qc31, email theo cái thông tư 09 )
                //if(Device.EmailAlarm && !String.IsNullOrWhiteSpace(Device.EmailAddess))
                //    _cacheLog.PushOverSpeedAlarmEmail(Device,Driver, packet.GpsInfo.Lat, packet.GpsInfo.Lng);

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "106");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "BeginOverSpeed");
                return false;
            }
        }

        private void ResetDriver()
        {
            if (Device.Status.DriverStatus.LastUpdateOverSpeed <
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
            {
                Device.Status.DriverStatus.LastUpdateOverSpeed = DateTime.Now;
                Device.Status.DriverStatus.OverSpeedCount = 0;
            }
        }

        /// <summary>
        ///     xử lý sự kiện kết thúc quắ vận tốc
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool EndOverSpeed(P107EndOverSpeed packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Kết thúc quá vận tốc")) return false;

                //cập nhật tốc độ trong sự kiện
                if (Device.Temp.MaxSpeed < packet.GpsInfo.Speed) Device.Temp.MaxSpeed = packet.GpsInfo.Speed;
                Device.Status.BasicStatus.Speed = packet.GpsInfo.Speed;

                if (BuildEndTraceLog(TraceType.OverSpeed, packet))
                {
                    ResetDriver();
                    Device.Status.DriverStatus.OverSpeedCount++;
                    Device.Temp.OverSpeedCount++;
                    UpdateStatus();

                    //forward toi xe dien
                    TramForwardEvent(packet, "107");
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "EndOverSpeed");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện bắt đầu dừng
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool BeginStop(P108BeginStop packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Bắt đầu dừng")) return false;

                Device.Temp.HasStopEvent = true;

                //cap nhat thoi gian dung o day, vi no bi thoat ra tai cho kiem tra khoang cach
                Device.Temp.TimePause = packet.TimeUpdate;


                ////kiểm tra khoảng cách thật sự có đúng là xe đang đi sau đó dừng lại hay không
                //if (Device.Temp.TraceStartDistance > 0 
                //    && (Device?.Status?.BasicStatus?.TotalGpsDistance??0) - Device.Temp.TraceStartDistance <=0)
                //{
                //    Log.Debug("PACKET", $"Xe chưa thật sự đi : Serial {Device.Serial}");
                //    return false;
                //}

                //kiểm tra khoảng cách thật sự có đúng là xe đang đi sau đó dừng lại hay không
                if (Device.Temp.TraceStartDistance > 0
                    && (Device?.Status?.BasicStatus?.TotalGpsDistance ?? 0) - Device.Temp.TraceStartDistance == 0) 
                    // 2019-09-17 (theo phan cung KIET) bo qua tinh huong bi AM do nhieu~
                {
                    Log.Debug("PACKET", $"Xe chưa thật sự đi : Serial {Device.Serial}");
                    return false;
                }

                //kiểm tra và bõ qua sự kiện trùng
                if (!BuildBeginTraceLog(packet, TraceType.Stop))
                {
                    Log.Debug("PACKET", $"Đã có sự kiện dừng : Serial {Device.Serial}");
                    return false;
                }

                //Device.Temp.TimePause = packet.TimeUpdate;

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "108");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "BeginStop");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện kết thúc dừng
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool EndStop(P109EndStop packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Kết thúc dừng")) return false;

                Device.Temp.HasStopEvent = false;

                //Chỉ xử lý sự kiện khi thời gian dừng >= 1 phút
                if (BuildEndTraceLog(TraceType.Stop,packet,50,60,0))
                {
                    Device.Status.PauseCount++;
                    Device.Temp.PauseCount++;

                    //ghi lại khoảng cách khi xe bắt đầu đi
                    Device.Temp.TraceStartDistance = Device?.Status?.BasicStatus?.TotalGpsDistance ?? 0;
                    Device.Temp.TimePause = DateTime.MinValue;

                    //2019-10-08 : Cap nhat lai su kien don khach khi xe chay (yeu cau cua THAI)
                    var trace = Device.Temp.GetTrace(TraceType.HasGuest);
                    if (trace != null && trace.Note==null)
                    {
                        trace.BeginTime = packet.TimeUpdate;//cap nhat thoi gian
                        trace.Note = "";
                        //trace.BeginLocation = new GpsLocation { Lat = packet.GpsInfo.Lat, Lng = packet.GpsInfo.Lng };//cap nhat vi tri
                    }

                    UpdateStatus();

                    //forward toi xe dien
                    TramForwardEvent(packet, "109");
                }
                else // bõ qua nếu chưa có sự kiện dừng
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "EndStop");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện thiết bị reset
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool DeviceReset(P110DeviceReset packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Reset")) return false;
                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "110");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "DeviceReset");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện đổi tài
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool ChangeDriver(P111ChangeDriver packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Đổi tài")) return false;

                BuildEndTraceLog(TraceType.ChangeDriver,packet);

                // kiểm tra thông tin tài xế mới
                Log.Info("SERVER", $"Đổi sang tài id {packet.NewDriverId} serial {packet.Serial}");
                Driver = Cache.GetQueryContext<Driver>().GetByKey((long)packet.NewDriverId);

                Device.Status.DriverStatus.DriverId = Driver?.Id ?? 0;
                Device.Status.DriverStatus.Name = Driver?.Name ?? "--";
                Device.Status.DriverStatus.Gplx = Driver?.Gplx ?? "--";
                Device.Temp.OverSpeedCount = Device.Status.DriverStatus.OverSpeedCount = 0;
                Device.Status.DriverStatus.TimeBeginWorkInSession = packet.TimeUpdate;
                DataContext.Update(Device.Status, MotherSqlId);
                DataContext.Commit(MotherSqlId);
                ResetDriver();
                
                // tạo ra 1 điểm bắt đàu phiên làm việc cho tài xế 
                BuildBeginTraceLog(packet, TraceType.ChangeDriver);

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "111");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "ChangeDriver");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện reset phiên làm việc
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool ResetTimeWork(P112ResetDriverTimeWork packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Reset thời gian lái xe")) return false;

                Device.Status.DriverStatus.TimeBeginWorkInSession = DateTime.Now;
                
                // xem thử có thông tin cuốc xe ko nếu có thì ghi nhận reset thời gian
                var trace = Device.Temp.GetTrace(TraceType.Machine);
                if(trace!=null)
                {
                    trace.DriverTime = packet.TimeUpdate;
                }

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "112");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "ResetTimeWork");
                return false;
            }
        }

        /// <summary>
        ///     xử lý sự kiện báo đổi sim
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool ChangeSim(P113ChangeSim packet)
        {
            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Đổi sim")) return false;

                var trace = new DeviceTraceLog
                {
                    CompanyId = Company.Id,
                    Serial = Device.Serial,
                    Indentity = Device.Indentity,
                    BeginLocation = Device.Status.BasicStatus.GpsInfo,
                    EndLocation = Device.Status.BasicStatus.GpsInfo,
                    BeginTime = packet.TimeUpdate,
                    EndTime = packet.TimeUpdate,
                    DbId = Company.DbId,
                    DriverId = Driver?.Id ?? 0,
                    GroupId = Group?.Id ?? 0,
                    Type = TraceType.ChangeSim,
                    Id = 0,
                    Note = ""
                };

                _locationQuery.GetAddress(trace.BeginLocation);
                _cacheLog.PushTraceLog(trace);

                UpdateStatus();

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "ChangeSim");
                return false;
            }
        }

        /// <summary>
        ///     Xử lý thông tin cài đặt của thiết bị gưởi lên
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool DeviceInfo(P205DeviceInfo packet)
        {
            try
            {
                BuildRawLog(packet.OriginalData, packet.TimeUpdate, "Thông tin cài đặt");

                bool iscreate = Device.SetupInfo == null;
                if (iscreate)
                    Device.SetupInfo = new DeviceSetupInfo() { Serial = Device.Serial, Device = Device };

                //cập nhật thông tin thêm từ thiết bị
                Device.SetupInfo.OverSpeed = packet.OverSpeed;

                //Do người dùng không cài đặt thông tin cơ bản nên tạm thời lấy OverSpeedDefault bằng OverSpeed nếu OverSpeedDefault chưa có
                if (Device.SetupInfo.OverSpeedDefault == 0)
                    Device.SetupInfo.OverSpeedDefault = Device.SetupInfo.OverSpeed;

                Device.SetupInfo.FirmWareVersion = packet.FirmWareVersion;
                Device.SetupInfo.HardWareVersion = packet.HardWareVersion;
                Device.SetupInfo.TimeUpdate = packet.TimeUpdate;

                //thông tin chuẩn có sẳng trong cấu hình
                Device.SetupInfo.OverTimeInDay = packet.OverTimeInDay;
                Device.SetupInfo.OverTimeInSession = packet.OverTimeInSession;
                Device.SetupInfo.TimeSync = packet.TimeSync;

                if (packet.PhoneSystemControl.Count > 0)
                    Device.SetupInfo.AllPhoneSystem = packet.PhoneSystemControl.Aggregate((c, a) => c + '|' + a);
                else
                    Device.SetupInfo.AllPhoneSystem = "";

                if(iscreate)
                    DataContext.Insert(Device.SetupInfo, MotherSqlId);
                else
                    DataContext.Update(Device.SetupInfo, MotherSqlId);

                DataContext.Commit(MotherSqlId);

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "DeviceInfo");
                return false;
            }
        }

        /// <summary>
        ///     thông tin số điện thoại
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool SimPhone(P207DeviceSimPhoneInfo packet)
        {
            try
            {
                BuildRawLog(packet.OriginalData, packet.TimeUpdate, "Cập nhất SĐT");

                if (string.IsNullOrEmpty(packet.Phone))
                {
                    Log.Warning("PacketHandle", $"Cập nhật số điện thoại cho thiết bị {Device.Serial} không hợp lệ");
                    return false;
                }
                Device.SimInfo.Phone = packet.Phone;
                Device.SimInfo.PhoneUpdate = packet.TimeUpdate;

                DataContext.Update(Device.SimInfo, MotherSqlId, m => m.Phone, m => m.PhoneUpdate);
                DataContext.Commit(MotherSqlId);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "SimPhone");
                return false;
            }
        }

        /// <summary>
        ///     thông tin tiền trong sim
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool SimMoney(P209DeviceSimeMoneyInfo packet)
        {
            try
            {
                BuildRawLog(packet.OriginalData, packet.TimeUpdate, "Cập nhật tiền sim");

                if (string.IsNullOrEmpty(packet.Money))
                {
                    Log.Warning("PacketHandle", $"Cập nhật số tiền trong sim cho thiết bị {Device.Serial} không hợp lệ");
                    return false;
                }
                Device.SimInfo.Money = packet.Money;
                Device.SimInfo.MoneyUpdate = packet.TimeUpdate;

                DataContext.Update(Device.SimInfo, MotherSqlId, m => m.Money, m => m.MoneyUpdate);
                DataContext.Commit(MotherSqlId);

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "SimMoney");
                return false;
            }
        }


        /// <summary>
        /// Kiểm tra quá tốc độ và trả về lệnh nếu có
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private byte[] DetectOverSpeed(PBaseSyncPacket packet)
        {
            if (Device.SetupInfo == null) return null;
            if (packet.GpsInfo == null) return null;

            try
            {
                //Lấy thông tin đường cao tốc
                RouteGpsLogic route = Cache.GetRoute(packet.GpsInfo.Lat, packet.GpsInfo.Lng);

                //nằm trong vùng đường cao tốc
                byte maxspeed = 0;
                if (route != null)
                {
                    if (Device.SetupInfo.OverSpeed == Device.SetupInfo.OverSpeedDefault)
                        maxspeed = route.GetMaxSpeed(0);
                }
                else //ra khu tốc độ mặc định
                {
                    if (Device.SetupInfo.OverSpeed != Device.SetupInfo.OverSpeedDefault)
                    {
                        maxspeed = Device.SetupInfo.OverSpeedDefault;
                        //Log.Warning("PACKET", $"Thiết bị {Device.Serial} ROUTE {Device.SetupInfo.OverSpeed} {Device.SetupInfo.OverSpeedDefault} {maxspeed}");
                    }
                }

                if (maxspeed > 0)
                {
                    byte[] ret;
                    var data = new P203SetOverSpeed { OverSpeed = maxspeed }.Serializer();
                    //var opcode = typeof(P203SetOverSpeed).GetCustomAttribute<DeviceOpCodeAttribute>().Opcode;
                    using (var memory = new MemoryStream())
                    {
                        using (var w = new BinaryWriter(memory))
                        {
                            w.Write((short)203);
                            w.Write((short)data.Length);
                            w.Write(data);
                            //w.Write(new Crc32().ComputeChecksum(data));
                            w.Write(Crc32.ComputeChecksum(data));
                            ret = memory.ToArray();
                        }
                    }

                    Log.Warning("PACKET", $"Thiết bị {Device.Serial} gửi lệnh xong {maxspeed}");

                    //lưu lại lệnh lấy yêu cầu để gửi lệh cập nhật tốc độ
                    Device.SetupInfo.RequestInforCommand = GetRequestInforCommand();

                    return ret;
                }
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, $" DetectOverSpeed xử lý lỗi {Device.Serial}");
            }

            return null;
        }


        /// <summary>
        /// Lấy lệnh dữ liệu đọc thông tin
        /// </summary>
        /// <returns></returns>
        private byte[] GetRequestInforCommand()
        {
            try
            {
                var data = new P204ReadDeviceInfo().Serializer();
                //var opcode = typeof(P204ReadDeviceInfo).GetCustomAttribute<DeviceOpCodeAttribute>().Opcode;
                using (var memory = new MemoryStream())
                {
                    using (var w = new BinaryWriter(memory))
                    {
                        w.Write((short)204);
                        w.Write((short)data.Length);
                        w.Write(data);
                        //w.Write(new Crc32().ComputeChecksum(data));
                        w.Write(Crc32.ComputeChecksum(data));
                        return memory.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, $" GetRequestInforCommand {Device.Serial}");
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem có đúng là sự kiện khách không
        /// </summary>
        /// <returns></returns>
        private bool ValidGuestEvent(BaseEvent packet)
        {
            //nếu tắt máy
            if (!Device.Status.BasicStatus.Machine) return true;

            //nếu đã dừng
            if (Device.Temp.HasStopEvent) return true;

            //Kiểm tra theo khoảng tốc độ của gói Sync trước đó (<= GUEST_MAX_SPEEDs)
            //if (Device.Status.BasicStatus.Speed <= GUEST_MAX_SPEED) return true;
            if (packet.GpsInfo.Speed <= GUEST_MAX_SPEED) return true;

            return false;
        }


        /// <summary>
        ///     Xử lý thông tin đón khách
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool BeginGuest(P115BeginGuest packet)
        {
            if (Device.FuelQuoteHour > 0)
            {
                Log.Debug("PACKET", $"Nhận được gói tin mở báo rung {Device.Serial} thời gian {packet.TimeUpdate}");
                return true;
            }

            if (Device.DeviceType == DeviceType.TaxiVehicle)
                Log.Info("PACKET", $"Đón khách {Device.Serial} thời gian {packet.TimeUpdate} GPS {packet.GpsInfo.Lat},{packet.GpsInfo.Lng} {packet.GpsInfo.Speed} {Device?.Status?.BasicStatus?.TotalGpsDistance}");
            else
                Log.Debug("PACKET", $"Đón khách {Device.Serial} thời gian {packet.TimeUpdate} GPS {packet.GpsInfo.Lat},{packet.GpsInfo.Lng} {packet.GpsInfo.Speed} {Device?.Status?.BasicStatus?.TotalGpsDistance}");

            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Đón khách")) return false;

                ////cập nhật tình trạng cảm ứng đón khách
                //if (!Device.Temp.HasGuestSensor) Device.Temp.HasGuestSensor = true;

                if (!ValidGuestEvent(packet))
                {
                    //Log.Debug("PACKET", $"{Device.Serial} sự kiện giả đón khách");
                    Log.Info("PACKET", $"{Device.Serial} sự kiện giả đón khách SYNC Speed {Device.Status.BasicStatus.Speed} EVENT Speed {packet.GpsInfo.Speed}");
                    return false;
                }

                //cập nhật tình trạng đón khách
                if(!Device.Status.BasicStatus.UseTemperature) Device.Status.BasicStatus.UseTemperature = true;

                //kiểm tra và bõ qua sự kiện trùng
                if (!BuildBeginTraceLog(packet, TraceType.HasGuest))
                {
                    //Log.Debug("PACKET", $"Đã có sự kiện đón khách : Serial {Device.Serial}");
                    Log.Info("PACKET", $"Đã có sự kiện đón khách : Serial {Device.Serial}");
                    UpdateStatus();
                    return false;
                }

                //kết thúc 1 cuốc xe không và hơn 60 giây mới ghi nhận
                BuildEndTraceLog(TraceType.NoGuest, packet, 50,GUEST_MIN_TIME,GUEST_MIN_DISTANCE);

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "115");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "BeginGuest");
                return false;
            }
        }


        /// <summary>
        ///     Xử lý thông tin trả khách
        /// </summary>
        /// <param name="packet"></param>
        [HttpPost]
        public bool EndGuest(P116EndGuest packet)
        {
            if (Device.FuelQuoteHour > 0)
            {
                Log.Debug("PACKET", $"Nhận được gói tin tắt báo rung {Device.Serial} thời gian {packet.TimeUpdate}");
                return true;
            }

            if (Device.DeviceType == DeviceType.TaxiVehicle)
                Log.Info("PACKET", $"Trả khách {Device.Serial} thời gian {packet.TimeUpdate} GPS {packet.GpsInfo.Lat},{packet.GpsInfo.Lng} {packet.GpsInfo.Speed} {Device?.Status?.BasicStatus?.TotalGpsDistance}");
            else
                Log.Debug("PACKET", $"Trả khách {Device.Serial} thời gian {packet.TimeUpdate} GPS {packet.GpsInfo.Lat},{packet.GpsInfo.Lng} {packet.GpsInfo.Speed} {Device?.Status?.BasicStatus?.TotalGpsDistance}");

            try
            {
                if (!BuildRawLogWithCheck(packet.OriginalData, packet.TimeUpdate, "Trả khách")) return false;

                ////cập nhật tình trạng cảm ứng đón khách
                //if (!Device.Temp.HasGuestSensor) Device.Temp.HasGuestSensor = true;

                if (!ValidGuestEvent(packet))
                {
                    //Log.Debug("PACKET", $"{Device.Serial} sự kiện giả trả khách");
                    Log.Info("PACKET", $"{Device.Serial} sự kiện giả trả khách SYNC Speed { Device.Status.BasicStatus.Speed} EVENT Speed { packet.GpsInfo.Speed}");
                    return false;
                }

                //cập nhật tình trạng trả khách
                if(Device.Status.BasicStatus.UseTemperature) Device.Status.BasicStatus.UseTemperature = false;

                //Chỉ xử lý sự kiện khi khoảng cách 50m mới tính query địa chỉ kết thúc và hơn 60 giây mới ghi nhận
                if (!BuildEndTraceLog(TraceType.HasGuest, packet, 50, GUEST_MIN_TIME, GUEST_MIN_DISTANCE))
                {
                    UpdateStatus();
                    return false;
                }

                //Có cảm biến đón khách, bắt đầu cuốc xe không
                BuildBeginTraceLog(packet, TraceType.NoGuest);

                UpdateStatus();

                //forward toi xe dien
                TramForwardEvent(packet, "116");

                return true;
            }
            catch (Exception e)
            {
                Log.Exception("PACKET", e, "EndGuest");
                return false;
            }

        }


    }
}