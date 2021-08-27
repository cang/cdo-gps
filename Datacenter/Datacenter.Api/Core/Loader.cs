#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : Loader.cs
// Time Create : 11:21 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using ConfigFile;
using DaoDatabase;
using DaoDatabase.AutoMapping;
using Datacenter.Api.Models;
using Datacenter.Model;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.QueryRoute;
using DataCenter.Core;
using Log;
using StarSg.Core;
using System.Collections.Generic;
using Datacenter.Model.Log.ZipLog;
using System.Runtime.InteropServices;
using StarSg.Utils.Models.DatacenterResponse.Status;
using StarSg.Utils.Models.Tranfer;
using Datacenter.Model.Utils;

#endregion

namespace Datacenter.Api.Core
{
    /// <summary>
    ///     quản lý thông tin cấu hình của hệ thống và entry point loader
    /// </summary>
    [Export]
    [Export(typeof (IModuleFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Loader : IPartImportsSatisfiedNotification, IModuleFactory
    {
        private const string Path = "Datacenter.Api.xml";


        [Import] private IConfigManager _cfg;
        [Import] private ILog _log;
        [Import] private ReponsitoryFactory _reponsitory;
        [Import] private IDataStore _cache;
        [Import] private ILocationQuery _locationQuery;

        [Import] private CacheLogManager _cacheLog;
        [Import] private RealtimeDeviceSatus _realtimeDeviceSatus;

        /// <summary>
        ///     Cấu hình load từ file
        /// </summary>
        public ResponseDataConfig Config { get; private set; }

        private const int INTERVAL_JOB_MINUTES = 5;//5 minutes
        private DateTime LastMidnight;
        private DateTime LastIntervalJob;
        private DateTime LastMinuteJob;
        private DateTime Last4Am;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            try
            {
                //load Datacenter.Api.xml
                Config = _cfg.Read<ResponseDataConfig>(HostingEnvironment.MapPath("~/bin/Config/") + Path);
                //_cfg.Write<ResponseDataConfig>(Config, HostingEnvironment.MapPath("~/bin/Config/") + "Datacenter.Out.xml");

                //_cacheLog.MotherSqlId = Config?.MotherSql?.Id ?? 0;
                _cacheLog.Config = Config;

                if (Config.GeoCode != null) ResponseDataConfig.GeoServerUrl = Config.GeoCode.GeoServerUrl;
                if (!String.IsNullOrWhiteSpace(Config.RouteDomain))
                {
                    ResponseDataConfig.RouteDomainUrl = Config.RouteDomain;
                    _log.Info("SYSTEM", $"Update RouteDomain {ResponseDataConfig.RouteDomainUrl} {Config.RouteDomain}");
                }
                else
                {
                    _log.Info("SYSTEM", $"Default RouteDomain {ResponseDataConfig.RouteDomainUrl} {Config.RouteDomain}");
                }
                
                ResponseDataConfig.UseTramForwardUrl = Config.UseTramForward;
                if (!String.IsNullOrWhiteSpace(Config.TramForwardEvent)) ResponseDataConfig.TramForwardEventUrl = Config.TramForwardEvent;
                if (!String.IsNullOrWhiteSpace(Config.TramForwardSync)) ResponseDataConfig.TramForwardSyncUrl = Config.TramForwardSync;

                _log.Info("SYSTEM", $"UseTramForward {Config.UseTramForward}");
                _log.Info("SYSTEM", $"TramForwardEvent {Config.TramForwardEvent}");
                _log.Info("SYSTEM", $"TramForwardSync {Config.TramForwardSync}");

                _log.Info("SYSTEM", $"GeoCode.GeoServerUrl {Config?.GeoCode?.GeoServerUrl ?? "NULL"}");
                _log.Info("SYSTEM", $"ID {Config?.Id ?? Guid.Empty}");
                _log.Info("SYSTEM", $"MotherSql.DataName {Config?.MotherSql?.DataName ?? "NULL"}");


                //Khởi tạo SignalR Connection đến route 
                _realtimeDeviceSatus.InitHubConnection();

                var mapEntity =
                    Assembly.GetAssembly(typeof (Company))
                        .GetTypes()
                        .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof (IEntity)) != null && m.IsClass)
                        .Select(m =>
                        {
                            var makeme = typeof (FactoryMap<>).MakeGenericType(m);
                            return makeme;
                        }).ToList();

                //mapEntity.AddRange(mapComponent);
                //mapEntity.Reverse();
                _reponsitory.Register(Config.MotherSql.Id, true, Config.MotherSql.DataName, new NhibernateConfig
                {
                    Maps = mapEntity,
                    Config =
                        DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, Config.MotherSql.DataName,
                            Config.MotherSql.User, Config.MotherSql.Pass, false, null)
                });

                //Đăng kí SgsiDataArchive
                String zipdbname = Config.MotherSql.DataName + "Archive";
                int zipid = Config.MotherSql.Id + 1000;
                var mapZip =
                                    Assembly.GetAssembly(typeof(Company))
                                        .GetTypes()
                                        .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                                        .Select(m =>
                                        {
                                            var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                            return makeme;
                                        }).ToList();
                _reponsitory.Register(zipid, true, zipdbname, new NhibernateConfig
                {
                    Maps = mapZip,
                    Config =
                        DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                            Config.MotherSql.User, Config.MotherSql.Pass, false, null)
                });


                var mapReport = Assembly.GetAssembly(typeof (Company))
                    .GetTypes()
                    .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof (IDbLog)) != null && m.IsClass)
                    .Select(
                        m =>
                        {
                            var makeme = typeof (FactoryMap<>).MakeGenericType(m);
                            return makeme;
                        }).ToList();

                foreach (var re in Config.ReportSqls.Values)
                {
                    _log.Debug("Loader", $"Đăng ký report server id {re.Id}");
                    _reponsitory.Register(re.Id, false, re.DataName, new NhibernateConfig
                    {
                        Maps = mapReport,
                        Config =
                            DatabaseConfigFactory.GetDataConfig(false, re.Ip, 0, re.DataName,
                                re.User, re.Pass, false, null)
                    });
                }

                Task.Factory.StartNew(ProcessCronJob);
            }
            catch (Exception e)
            {
                _log.Exception("Loader",e,$"OnImportsSatisfied");
            }
        }

        #endregion

        /// <summary>
        /// Chạy thread riêng thực hiện các tác vụ ngầm
        /// </summary>
        /// <returns></returns>
        private async Task ProcessCronJob()
        {
            LastIntervalJob = LastMinuteJob = DateTime.Now;
            LastMidnight = new DateTime(LastIntervalJob.Year, LastIntervalJob.Month, LastIntervalJob.Day, 0, 0, 0,0);

            //lay mac dinh ngay hom qua VA kiem tra khoang thoi gian tam 4h-5h de tranh truong hop khoi dong lai reset gia tri nay chay them nhieu lan trong ngay khi reset app
            Last4Am = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 4, 0, 0).AddDays(-1);

            //waithandleProcessCronJob = new EventWaitHandle(false, EventResetMode.AutoReset);
            while (true)
            {
                //ProcessCronJobEachMinutes();//gọi chạy từ browser để kích hoạt ứng dụng khi bị tắt
                ProcessCronJobAtMidnight();
                ProcessCronJobAt4AM();
                ProcessCronJobEachMinute();
                ProcessOnlineDeviceForGsm();
                await Task.Delay(1000);
            }
        }


        /// <summary>
        /// Process cron job at mid night
        /// </summary>
        private void ProcessCronJobAtMidnight()
        {
            if ((DateTime.Now - LastMidnight).TotalDays < 1) return;
            try
            {
                _log.Debug("CRONJOB AT midnight", DateTime.Now.ToString());

                //reset for new day
                foreach (var d in _cache.GetQueryContext<Device>().GetAll())
                {
                    if (d.Status != null)
                    {
                        //phai luu lai, nhung cho nay k luu, can luu gia tri tu Temp
                        d.Status.PauseCount = 0;
                        if (d.Status.DriverStatus != null)
                            d.Status.DriverStatus.OverSpeedCount = 0;

                        d.Temp.LastTotalKmUsingOnDay 
                            = d.Status.LastTotalKmUsingOnDay 
                            = d.Status.BasicStatus?.TotalGpsDistance ?? 0;
                    }

                    //reset all temp data
                    d.Temp.Reset();
                }

                #region Collect generalReportLogs to save into database
                try
                {
                    List<GeneralReportLog> generalReportLogs = new List<GeneralReportLog>();
                    foreach (var d in _cache.GetQueryContext<Device>().GetAll())
                    {
                        if (d.Temp.GeneralReportLog != null)
                            generalReportLogs.Add(d.Temp.GeneralReportLog);
                    }

                    //save to db
                    using (var dataContext = _reponsitory.CreateQuery())
                    {
                        dataContext.GetReponsitory().InsertAll(generalReportLogs);
                        dataContext.Commit();
                    }
                    _log.Debug("CRONJOB AT midnight : Save GeneralReportLog to database successfull", DateTime.Now.ToString());
                }
                catch (Exception e)
                {
                    _log.Exception("CRONJOB AT midnight", e, "Save GeneralReportLog");
                }
                #endregion Collect generalReportLogs to save into database


                //reset GeneralReportLog for new day
                foreach (var d in _cache.GetQueryContext<Device>().GetAll())
                {
                    if(d.Temp.GeneralReportLog!=null)
                        lock(d.Temp.GeneralReportLogLock)
                        {
                            d.Temp.GeneralReportLog = null;//reset for new day
                        }
                }

            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessCronJobAtMidnight");
            }

            LastMidnight = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        /// <summary>
        /// Process cron job at 4 am 
        /// </summary>
        private void ProcessCronJobAt4AM()
        {
            if ((DateTime.Now - Last4Am).TotalDays < 1) return;
            if (DateTime.Now.Hour!=4) return;//neu reset lai app trong khoang 4-5h thi task nay se chay nhieu lan
            try
            {
                _log.Debug("CRONJOB AT 4AM", DateTime.Now.ToString());

                #region Collect GeneralGuestLog to save into database
                try
                {
                    List<GeneralGuestLog> generalReportLogs = new List<GeneralGuestLog>();
                    foreach (var d in _cache.GetQueryContext<Device>().GetAll())
                    {
                        if (d.Temp.GeneralGuestLog != null)
                            generalReportLogs.Add(d.Temp.GeneralGuestLog);
                    }

                    //save to db
                    using (var dataContext = _reponsitory.CreateQuery())
                    {
                        dataContext.GetReponsitory().InsertAll(generalReportLogs);
                        dataContext.Commit();
                    }
                    _log.Debug("CRONJOB AT 4AM : Save GeneralGuestLog to database successfull", DateTime.Now.ToString());
                }
                catch (Exception e)
                {
                    _log.Exception("CRONJOB AT 4am", e, "Save GeneralGuestLog");
                }
                #endregion Collect GeneralGuestLog to save into database


                //reset GeneralReportLog for new day
                foreach (var d in _cache.GetQueryContext<Device>().GetAll())
                {
                    if (d.Temp.GeneralGuestLog != null)
                        lock (d.Temp.GeneralGuestLogLock)
                        {
                            d.Temp.GeneralGuestLog = null;//reset for new day
                        }
                }

            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessCronJobAt4AM");
            }

            Last4Am = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,4,0,0);
        }

        /// <summary>
        /// Backup data from cache to memory each time interval
        /// </summary>
        private void ProcessCronJobEachMinutes()
        {
            if ((DateTime.Now - LastIntervalJob).TotalMinutes < INTERVAL_JOB_MINUTES) return;
            try
            {
                _cache.SaveCacheData();
            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessCronJobEachMinutes");
            }
            LastIntervalJob = DateTime.Now;
        }


        /// <summary>
        /// Xử lý gửi tin nhắn nếu như bị mất kết nối
        /// </summary>
        private bool ProcessOnlineDevice()
        {
            if (!IsConnectionAvailable() || String.IsNullOrEmpty(ResponseDataConfig.GeoServerUrl))
                return false;

            DateTime now = DateTime.Now;
            try
            {
                foreach (var d in _cache.GetQueryContext<Device>().GetWhere(m => m.OnlineTimeout > 0))
                {
                    if (String.IsNullOrWhiteSpace(d.OwnerPhone) || d.Temp.SentOnlineSms) continue;

                    int limitmils;
                    if (d.Status.BasicStatus.Machine)
                        limitmils = d.OnlineTimeout * 20;
                    else
                        limitmils = d.OnlineTimeout * 15 * 60;

                    //kiểm tra theo thời gian nhận trên server
                    if ((now - d.Status.BasicStatus.ServerRecv).TotalSeconds > limitmils)
                    {
                        String sms = $"Hop dinh vi tren Xe {d.Bs} da bi mat ket noi qua thoi gian { Math.Round((now - d.Status.BasicStatus.ServerRecv).TotalMinutes, 1)} phut";
                        try
                        {
                            List<String> phonenos = d.OwnerPhone.Split('|', ',', ';').Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => m.Trim()).ToList();
                            if (phonenos.Count > 0)
                            {
                                SmsSendBody smsbody = new SmsSendBody()
                                {
                                    content = sms,
                                    to = phonenos
                                    //to = new List<string>() { d.OwnerPhone }
                                };

                                //gui sms
                                var smsSendResponse = new ForwardApi().Post<SmsSendResponse>($"{ResponseDataConfig.GeoServerUrl}/sms/send", smsbody);
                                if (smsSendResponse != null && "success".Equals(smsSendResponse.status))
                                {
                                    d.Temp.SentOnlineSms = true;

                                    _log.Info("CRONJOB", $"Đã gửi tin nhắn thiết bị {d.Serial} đến số {d.OwnerPhone} nội dung={sms}");
                                }
                                else
                                {
                                    _log.Error("CRONJOB", $"ERROR sms/send thiết bị {d.Serial} đến số {d.OwnerPhone} lỗi={(smsSendResponse != null && smsSendResponse.message != null ? smsSendResponse.message : "")}");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _log.Exception("CRONJOB", e, "sms/send");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessOnlineDevice");
            }

            return true;
        }

        /// <summary>
        /// Xử lý gsm nếu như bị mất kết nối
        /// </summary>
        private bool ProcessOnlineDeviceForGsm()
        {
            if (!IsConnectionAvailable())
                return false;

            DateTime now = DateTime.Now;
            try
            {
                foreach (var d in _cache.GetQueryContext<Device>().GetAll())
                {
                    if (d == null) continue;
                    if (d.Temp == null) continue;
                    if (d.Status == null) continue;
                    if (d.Status.BasicStatus == null) continue;
                    if (d.Temp.DeviceLostGsmLog != null) continue;

                    //kiểm tra theo thời gian nhận trên server
                    if (
                        (d.Status.BasicStatus.Machine && (now - d.Status.BasicStatus.ServerRecv).TotalMinutes > 60)
                        || 
                        (d.Status.BasicStatus.Machine==false && (now - d.Status.BasicStatus.ServerRecv).TotalMinutes > 120)

                        //(now - d.Status.BasicStatus.ClientSend).TotalMinutes > 120
                        )
                    {
                        try
                        {
                            //_cache.GetCompanyById(d.CompanyId).Setting.
                            d.Temp.DeviceLostGsmLog = new DeviceTraceLog()
                            {
                                CompanyId = d.CompanyId,
                                Serial = d.Serial,
                                Indentity = d.Indentity,
                                BeginLocation = d.Status.BasicStatus.GpsInfo??new Model.Components.GpsLocation(),
                                BeginTime = d.Status.BasicStatus.ServerRecv,
                                DbId =0,
                                DriverId = d?.Status?.DriverStatus?.DriverId??0,
                                GroupId = d.GroupId,
                                Type = TraceType.LostGsm,
                                Distance = d.Status.BasicStatus.TotalGpsDistance
                            };
                        }
                        catch (Exception e)
                        {
                            _log.Exception("CRONJOB", e, $"ProcessOnlineDeviceForGsm {d.Serial}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessOnlineDeviceForGsm");
            }

            return true;
        }

        /// <summary>
        /// Xử lý kiểm tra sự kiện xe công trình nếu mất kết nối thì cập nhật sự kiện tắt máy
        /// </summary>
        private bool ProcessXCTDevice()
        {
            if (!IsConnectionAvailable())
                return false;

            DateTime now = DateTime.Now;
            try
            {
                //Duyệt qua danh sách xe công trình
                foreach (var d in _cache.GetQueryContext<Device>().GetWhere(m => m.DeviceType==DeviceType.ConstructionVehicle || m.DeviceType == DeviceType.Dynamo))
                {
                    //Bõ qua nếu không có sự kiện mở máy
                    var trace = d.Temp.GetTrace(TraceType.Machine);
                    if (trace == null) continue;//if (!d.Temp.HasMachineOn)
                    if (d.Status == null) continue;

                    //Bõ qua nếu chưa có gói đồng bộ nào từ khi xuất hiện sự kiện
                    if (d.Status.BasicStatus.ClientSend <= trace.BeginTime) continue;

                    if ( 
                        d.Status.BasicStatus.Machine   //Ưu tiên kiễm tra chìa khóa trước nếu như chìa khóa tắt thì xử lý luôn 
                        && ( //Thời gian nhỏ hơn xx phút mất kết nối tiếp tục theo dõi && Kiểm tra cả client và server time phải luôn hơn xx phút mới xử lý
                            (now - d.Status.BasicStatus.ServerRecv).TotalMinutes < 15
                            || (now - d.Status.BasicStatus.ClientSend).TotalMinutes < 15 // k hiểu vì sao ServerRecv cách ClientSend khá xa mặc dù ClientSend k bi gián đoạn
                        )
                    ) continue;

                    //Cập nhật sự kiện
                    trace = d.Temp.EndTrace(TraceType.Machine);
                    if (trace == null)
                    {
                        _log.Debug("CRONJOB", $"Không tìm ra cuốc {d.Serial}");
                        continue;
                    }

                    //thời gian kết thúc sự kiện là thời gian đồng bộ cuối cùng trước khi mất kết nối
                    trace.EndTime = d.Status.BasicStatus.ClientSend;

                    //Lấy địa chỉ begin location
                    _locationQuery.GetAddress(trace.BeginLocation);
                    trace.EndLocation = trace.BeginLocation;

                    //tính khoảng cách theo thông tin thiết bị gửi lên
                    trace.Distance = 0;

                    //Ghi lại thời gian chạy dựa theo dự kiện mở máy
                    var tracetime = trace.EndTime - trace.BeginTime;
                    d.Temp.MachineSeconds += (int)Math.Round(tracetime.TotalSeconds);

                    trace.Note = "tắt nguồn " + tracetime.ToString("g");

                    //cập nhật lại thời gian chạy trong GeneralReportLog : trong trường hợp không tồn tại gói đồng bộ nào nữa trong ngày
                    //kiểm tra đúng ngày cập nhật là hôm nay, cập nhật data mới
                    if (d.Temp.GeneralReportLog.UpdateTime.Date == DateTime.Now.Date)
                    {
                        d.Temp.GeneralReportLog.OverTimeInday = d.Temp.MachineSeconds / 60;
                        d.Temp.GeneralReportLog.OverTimeIndayCount = d.Temp.GeneralReportLog.OverTimeInday / 600;
                    }

                    _cacheLog.PushTraceLog(trace);

                    //Xử lý kết thúc sự kiện
                    //d.Temp.TimeHandlePause = trace.EndTime;

                    //Cập nhật lại tình trạng của máy
                    if (d.Status.BasicStatus.Machine)
                    {
                        d.Status.BasicStatus.Machine = false;

                        //Cập nhật SignalR
                        UpdateStatus(d);

                        //Cập nhật DB
                        using (var dataContext = _reponsitory.CreateQuery())
                        {
                            dataContext.GetReponsitory().Update(d.Status);
                            dataContext.Commit();
                        }
                    }

                    _log.Debug("CRONJOB", $"Save Trace Machine : {d.Serial}");
                }
            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessXCTDevice");
            }

            return true;
        }

        /// <summary>
        /// Cập nhật sự kiện qua RignalR
        /// </summary>
        /// <param name="Device"></param>
        private void UpdateStatus(Device Device)
        {
            if (Device.Status != null)
                Task.Factory.StartNew(() =>
                {
                    try
                    {
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

                            //useGuest = Device.Temp?.HasGuestSensor ?? false,
                            useGuest = Device.DeviceType == DeviceType.TaxiVehicle,
                            Guest = Device.Status.BasicStatus.UseTemperature,

                            PauseTime = Device.Temp?.TimePause ?? DateTimeFix.Min
                        });
                    }
                    catch (Exception ex)
                    {
                        _log.Exception("CRONJOB", ex, "UpdateStatus ");
                    }
                });
        }

        /// <summary>
        /// Process each minute interval task
        /// </summary>
        private void ProcessCronJobEachMinute()
        {
            if ((DateTime.Now - LastMinuteJob).TotalMinutes < 1) return;

            try
            {
                ProcessOnlineDevice();
                ProcessXCTDevice();
            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "ProcessCronJobEachMinute");
            }
            finally
            {
                LastMinuteJob = DateTime.Now;
            }
        }


        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connDescription, int ReservedValue);

        /// <summary>
        /// check if a connection to the Internet can be established 
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionAvailable()
        {
            try
            {
                int connDesc;return InternetGetConnectedState(out connDesc, 0);
            }
            catch (Exception e)
            {
                _log.Exception("CRONJOB", e, "IsConnectionAvailable");
                return false;
            }
        }

        //public static bool DnsTest()
        //{
        //    try
        //    {
        //        System.Net.IPHostEntry ipHe = System.Net.Dns.GetHostByName("www.google.com");
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public bool PingTest()
        //{
        //    System.Net.NetworkInformation.PingReply pingStatus = new System.Net.NetworkInformation.Ping().Send("google.com", 1000);
        //    return (pingStatus.Status == System.Net.NetworkInformation.IPStatus.Success);
        //    //try
        //    //{
        //    //    Ping myPing = new Ping();
        //    //    byte[] buffer = new byte[32];
        //    //    PingOptions pingOptions = new PingOptions();
        //    //    PingReply reply = myPing.Send("google.com", 1000, buffer, pingOptions);
        //    //    return (reply.Status == IPStatus.Success);
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    return false;
        //    //}
        //}

        public class SmsSendResponse
        {
            public string status { get; set; }
            public string message { get; set; }
        }
        public class SmsSendBody
        {
            public List<String> to { get; set; }
            public string content { get; set; }
            public string type { get; set; }//brand or empty
        }

    }
}



#region phục vụ cho báo cáo 9: 'Cập nhật xe dừng' sau mỗi 15 phút đối phó bộ 

////todo: phục vụ cho báo cáo 9: 'Cập nhật xe dừng' sau mỗi 15 phút đối phó bộ 
//try
//{
//    var db = _reponsitory.CreateQuery();
//    foreach (var d in _cache.GetQueryContext<Device>().GetAll())
//    {
//        if (d.Status != null && d.Status.BasicStatus.ClientSend >= DateTime.Now.AddMinutes(-30))
//        {
//            if (d.Temp.TraceMachineId == 0)
//            {
//                if ((DateTime.Now - d.Temp.TimeHandlePause).TotalMinutes >= 15)
//                {
//                    // sinh ra 1 record sự kiện dừng 15 phút ở đây
//                    d.Temp.TimeHandlePause = DateTime.Now;
//                    //_locationQuery.GetAddress(d.Status.BasicStatus.GpsInfo);//Query quá nhiều k cần thiết địa chỉ ở đây
//                    var trace = new DeviceTraceLog
//                    {
//                        CompanyId = d.CompanyId,
//                        Serial = d.Serial,
//                        Indentity = d.Indentity,
//                        BeginLocation = d.Status.BasicStatus.GpsInfo,
//                        BeginTime = DateTime.Now,
//                        DbId = 0,
//                        DriverId = 0,
//                        GroupId = d.GroupId,
//                        Type = TraceType.Stop15,
//                        Distance = 0,
//                        EndLocation = d.Status.BasicStatus.GpsInfo,
//                        DriverTime = DateTime.Now,
//                        EndTime = DateTime.Now,
//                        Note = "Xe dừng 15 phút"
//                    };
//                    db.Insert(trace, 0);
//                    db.Commit();
//                }
//            }
//        }
//    }
//    db.Dispose();
//}
//catch (Exception e)
//{
//    _log.Error("CRONJOB each 15 MINUTE", e.Message);
//}

#endregion