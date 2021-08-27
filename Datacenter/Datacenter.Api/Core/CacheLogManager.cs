#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : CacheLogManager.cs
// Time Create : 10:07 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Datacenter.Model.Log;
using Datacenter.QueryRoute;
using Log;
using Datacenter.Api.Models;
using System.Data.SqlClient;
using System.Data;
using Datacenter.Model.Entity;
using StarSg.Core;
using System.Threading;

#endregion

namespace Datacenter.Api.Core
{
    /// <summary>
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CacheLogManager : IPartImportsSatisfiedNotification
    {
        private const int UPDATE_DB_INTERVAL = 10000;
        private const int LIMIT_COUNTER = 10000;
        private const int RETRY_TIME = 3;

        private readonly ConcurrentQueue<DeviceLog> _queueDeviceLogs = new ConcurrentQueue<DeviceLog>();
        private readonly ConcurrentQueue<DeviceTraceLog> _queueDeviceTraceLogs = new ConcurrentQueue<DeviceTraceLog>();
        private readonly ConcurrentQueue<DeviceRawLog> _queueRawLogs = new ConcurrentQueue<DeviceRawLog>();

        [Import] private ReponsitoryFactory _factory;
        [Import] private ILog _log;

        private object DataContextLock = new object();
        private IQueryRoute DataContext { get; set; }

        /// <summary>
        /// Cấu hình máy chủ , db ...
        /// </summary>
        public ResponseDataConfig Config { get; set; }

        //Send email
        private readonly ConcurrentQueue<AlarmEmailRequest> _queueAlarmEmailRequest = new ConcurrentQueue<AlarmEmailRequest>();
        private const int SENDEMAIL_INTERVAL = 6000;
        //private const double SENDEMAIL_RATE = 1000.0 / 14;//1000 ms / 14 
        //private DateTime LasttimeSendEmail = DateTime.Now;

        /// <summary>
        /// MotherSqlId
        /// </summary>
        public int MotherSqlId
        {
            get
            {
                if (Config == null || Config.MotherSql == null) return -1;
                return Config?.MotherSql?.Id ?? 0;
            }
        }


        /// <summary>
        /// ConnectionString sử dụng cho bulk insert
        /// </summary>
        public String ConnectionString
        {
            get
            {
                if (Config == null || Config.MotherSql==null) return null;
                return $@"Server={Config.MotherSql.Ip};Database={Config.MotherSql.DataName};User Id={Config.MotherSql.User};Password={Config.MotherSql.Pass};";
            }
        }

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            Task.Factory.StartNew(ImportData);
            Task.Factory.StartNew(SendEmails);
            _log.Success("Cache Log", "Khởi động hệ thống cập nhật log thành thông");
        }

        #endregion


        /// <summary>
        ///     Thêm dữ liệu thô vào Cache memory
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool PushRawLog(DeviceRawLog log)
        {
            _queueRawLogs.Enqueue(log);
            return true;
        }

        /// <summary>
        /// Thêm dữ sự kiện vào Cache memory
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool PushTraceLog(DeviceTraceLog log)
        {
            _queueDeviceTraceLogs.Enqueue(log);
            return true;
        }

        /// <summary>
        /// Thêm dữ log thiết bị vào Cache memory
        /// </summary>
        /// <param name="log"></param>
        public void PushHistoryDevice(DeviceLog log)
        {
            _queueDeviceLogs.Enqueue(log);
        }


        /// <summary>
        /// Cập nhật dữ liệu thô từ Cache vô db
        /// </summary>
        private void UpdateRawLog()
        {
            if (_queueRawLogs.Count > 0)
            {
                var tmp = new List<DeviceRawLog>();
                while (!_queueRawLogs.IsEmpty)
                {
                    DeviceRawLog log;
                    if (_queueRawLogs.TryDequeue(out log))
                    {
                        tmp.Add(log);
                    }
                    if (tmp.Count >= LIMIT_COUNTER) break;
                }

                //Chỉ cập nhật mỗi ngày 1 lần log để cần thì coi lại - phần dọn dẹp dữ liệu cũ sẽ nhờ sql server agent
                try
                {
                    // save database
                    DataContext.InsertAll(tmp, MotherSqlId);
                    DataContext.Commit(MotherSqlId);

                    _log.Debug("Cache Log",
                        $"Cập nhật thành công {tmp.Count} record RawLog vào database  {_queueRawLogs.Count}");
                }
                catch (Exception exception)
                {
                    _log.Exception("Cache Log", exception,$"Cập nhật thất bại UpdateRawLog");
                }
            }
        }

        /// <summary>
        /// Cập nhật sự kiện từ Cache vào db
        /// </summary>
        private void UpdateDeviceTraceLog()
        {
            if (_queueDeviceTraceLogs.Count > 0)
            {
                var tracelist = new List<DeviceTraceLog>();
                var guestlist = new List<DeviceGuestLog>();
                var changeDriverlist = new List<DeviceChangeDriverLog>();
                var changeSimlist = new List<DeviceChangeSimLog>();
                var lostGsmlist = new List<DeviceLostGsmLog>();

                while (!_queueDeviceTraceLogs.IsEmpty)
                {
                    DeviceTraceLog log;

                    if (_queueDeviceTraceLogs.TryDequeue(out log))
                    {
                        if (log.Type == TraceType.HasGuest || log.Type == TraceType.NoGuest)
                            guestlist.Add(log.CopyTo<DeviceGuestLog>());
                        else if (log.Type == TraceType.ChangeDriver)
                            changeDriverlist.Add(log.CopyTo<DeviceChangeDriverLog>());
                        else if (log.Type == TraceType.ChangeSim)
                            changeSimlist.Add(log.CopyTo<DeviceChangeSimLog>());
                        else if (log.Type == TraceType.LostGsm)
                            lostGsmlist.Add(log.CopyTo<DeviceLostGsmLog>());
                        else
                            tracelist.Add(log);
                    }

                    if (tracelist.Count >= LIMIT_COUNTER
                        || guestlist.Count >= LIMIT_COUNTER
                        || changeDriverlist.Count >= LIMIT_COUNTER
                        || changeSimlist.Count >= LIMIT_COUNTER
                        || lostGsmlist.Count >= LIMIT_COUNTER
                        ) break;
                }

                // save database
                if (tracelist.Count > 0)
                {
                    try
                    {
                        DataContext.InsertAll(tracelist, MotherSqlId);
                        DataContext.Commit(MotherSqlId);
                        _log.Debug("Cache Log",
                            $"Cập nhật thành công {tracelist.Count} record DeviceTraceLog vào database  {_queueDeviceTraceLogs.Count}");
                    }
                    catch (Exception exception)
                    {
                        _log.Exception("Cache Log", exception, $"Cập nhật thất bại DeviceTraceLog");
                    }
                }

                if (guestlist.Count > 0)
                {
                    try
                    {
                        DataContext.InsertAll(guestlist, MotherSqlId);
                        DataContext.Commit(MotherSqlId);
                        _log.Debug("Cache Log",
                            $"Cập nhật thành công {guestlist.Count} record DeviceGuestLog vào database  {_queueDeviceTraceLogs.Count}");
                    }
                    catch (Exception exception)
                    {
                        _log.Exception("Cache Log", exception, $"Cập nhật thất bại DeviceGuestLog");
                    }
                }
                if (changeDriverlist.Count > 0)
                {
                    try
                    {
                        DataContext.InsertAll(changeDriverlist, MotherSqlId);
                        DataContext.Commit(MotherSqlId);
                        _log.Debug("Cache Log",
                            $"Cập nhật thành công {changeDriverlist.Count} record DeviceChangeDriverLog vào database  {_queueDeviceTraceLogs.Count}");
                    }
                    catch (Exception exception)
                    {
                        _log.Exception("Cache Log", exception, $"Cập nhật thất bại DeviceChangeDriverLog");
                    }
                }
                if (changeSimlist.Count > 0)
                {
                    try
                    {
                        DataContext.InsertAll(changeSimlist, MotherSqlId);
                        DataContext.Commit(MotherSqlId);
                        _log.Debug("Cache Log",
                            $"Cập nhật thành công {changeSimlist.Count} record DeviceChangeSimLog vào database  {_queueDeviceTraceLogs.Count}");
                    }
                    catch (Exception exception)
                    {
                        _log.Exception("Cache Log", exception, $"Cập nhật thất bại DeviceChangeSimLog");
                    }
                }
                if (lostGsmlist.Count > 0)
                {
                    try
                    {
                        DataContext.InsertAll(lostGsmlist, MotherSqlId);
                        DataContext.Commit(MotherSqlId);
                        _log.Debug("Cache Log",
                            $"Cập nhật thành công {lostGsmlist.Count} record DeviceLostGsmLog vào database  {_queueDeviceTraceLogs.Count}");
                    }
                    catch (Exception exception)
                    {
                        _log.Exception("Cache Log", exception, $"Cập nhật thất bại DeviceLostGsmLog");
                    }
                }
            }
        }

        ///// <summary>
        ///// Cập nhật thông tin log thiết bị từ Cache vào db
        ///// </summary>
        //private void UpdateDeviceLog()
        //{
        //    if (_queueDeviceLogs.Count > 0)
        //    {
        //        var tmp = new List<DeviceLog>();
        //        while (!_queueDeviceLogs.IsEmpty)
        //        {
        //            DeviceLog log;
        //            if (_queueDeviceLogs.TryDequeue(out log))
        //            {
        //                tmp.Add(log);
        //            }
        //            if (tmp.Count >= LIMIT_COUNTER) break;
        //        }
        //        // save database
        //        DataContext.InsertAll(tmp, MotherSqlId);
        //        DataContext.Commit(MotherSqlId);
        //        _log.Debug("Cache Log",
        //            $"Cập nhật thành công {tmp.Count} record DeviceLog vào database  {_queueDeviceLogs.Count}");
        //    }
        //}

        /// <summary>
        /// Cập nhật thông tin log thiết bị từ Cache vào db (sử dụng bulk insert)
        /// </summary>
        private void UpdateDeviceLog()
        {
            if (ConnectionString == null) return;

            if (_queueDeviceLogs.Count > 0)
            {
                var tmp = new List<DeviceLog>();
                while (!_queueDeviceLogs.IsEmpty)
                {
                    DeviceLog log;
                    if (_queueDeviceLogs.TryDequeue(out log))
                    {
                        tmp.Add(log);
                    }
                    if (tmp.Count >= LIMIT_COUNTER) break;
                }


                // save database
                for (int i = 0; i < RETRY_TIME; i++)
                {
                    DataTable table = null;
                    try
                    {
                        using (var sqlBulk = new SqlBulkCopy(ConnectionString))
                        {
                            //sqlBulk.NotifyAfter = 1000;
                            //sqlBulk.SqlRowsCopied += SqlBulk_SqlRowsCopied;
                            sqlBulk.BatchSize = 5000;//mặc định vô tận
                            //sqlBulk.BulkCopyTimeout = 30;//mặc đinh 30s
                            sqlBulk.DestinationTableName = "DeviceLog";
                            //DataTable table = DeviceLog.CreateTable(tmp);
                            table = DeviceLog.CreateTable(tmp);
                            sqlBulk.WriteToServer(table);
                        }
                        _log.Debug("Cache Log",$"Cập nhật thành công {tmp.Count} record DeviceLog vào database  {_queueDeviceLogs.Count}");
                        break;
                    }
                    catch (Exception exception)
                    {
                        //if (table != null)
                        //{
                        //    try
                        //    {
                        //        String path = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Data"), System.IO.Path.GetRandomFileName());
                        //        table.WriteXml(path);
                        //    }
                        //    catch (Exception exx)
                        //    {
                        //        _log.Exception("Cache Log", exx,"AAA");
                        //    }
                        //}

                        _log.Exception("Cache Log", exception, $"Cập nhật thất bại DeviceLog lần {i+1}");
                        if (exception != null && exception.Message.Contains("Execution Timeout Expired"))
                        {
                            System.Threading.Thread.Sleep(1000);
                            continue;//retry 
                        }
                        break;
                    }
                    finally
                    {
                        if (table != null) table.Dispose();
                    }

                }//for (int i = 0; i < RETRY_TIME; i++)
            }//if (_queueDeviceLogs.Count > 0)
        }


        /// <summary>
        /// Xử lý cập nhật database từ memory
        /// </summary>
        public void ProcessImportData()
        {
            lock (DataContextLock)
            {
                if (null != DataContext) return;//đang xử lý
                try
                {
                    using (DataContext = _factory.CreateQuery())
                    {
                        UpdateRawLog();
                        UpdateDeviceTraceLog();
                        UpdateDeviceLog();
                    }
                }
                catch (Exception exception)
                {
                    _log.Exception("Cache Log", exception,$"Cập nhật thất bại Log");
                }
                finally
                {
                    DataContext = null;
                }
            }
        }

        private async Task ImportData()
        {
            //chờ MotherSqlId
            await Task.Delay(1000);

            //process
            while (true)
            {
                //xử lý
                if(MotherSqlId>=0) ProcessImportData();

                //delay
                await Task.Delay(UPDATE_DB_INTERVAL);
            }
        }

        /// <summary>
        /// PushOverSpeedAlarmEmail
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="driver"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="MaxSpeed"></param>
        /// <param name="AverageSpeed"></param>
        /// <param name="TotalDistance"></param>
        /// <param name="EndTime"></param>
        public void PushOverSpeedAlarmEmail(Device dev, Driver driver, float lat,float lon,int MaxSpeed,int AverageSpeed,int TotalDistance,DateTime EndTime)
        {
            if (dev == null) return;if (!dev.EmailAlarm) return;if (String.IsNullOrWhiteSpace(dev.EmailAddess)) return;
            var postdata = new AlarmEmailRequest()
            {
                template = "quatocdo.html",
                to = dev.EmailAddess,
                lat = lat,
                lon = lon,
                serial = dev.Serial,
                keys = new string[]
                {
                    "@@serial",
                    "@@bangsoxe",
                    "@@limitspeed",
                    "@@currentspeed",
                    "@@DriverId",
                    "@@DriverName",
                    "@@Gplx",
                    "@@MaxSpeed",
                    "@@AverageSpeed",
                    "@@TotalDistance",
                    "@@EndTime"
               },
                values = new string[]
                {
                    dev.Serial.ToString(),
                    dev.Bs,
                    (dev.SetupInfo?.OverSpeed ?? DeviceLogicHandles.Logics.OverSpeed09Logic.DEFAULT_SPEED).ToString(),
                    dev.Status.BasicStatus.Speed.ToString(),
                    driver?.Id.ToString() ?? "",
                    driver?.Name ?? "",
                    driver?.Gplx ?? "",
                    MaxSpeed.ToString(),
                    AverageSpeed.ToString(),
                    TotalDistance.ToString(),
                    EndTime.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            _queueAlarmEmailRequest.Enqueue(postdata);
        }

        /// <summary>
        /// PushOver4HAlarmEmail
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="driver"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        public void PushOver4HAlarmEmail(Device dev, Driver driver,float lat, float lon)
        {
            if (dev == null) return; if (!dev.EmailAlarm) return; if (String.IsNullOrWhiteSpace(dev.EmailAddess)) return;

            var postdata = new AlarmEmailRequest()
            {
                template = "chaylientuc4gio.html",
                to = dev.EmailAddess,
                lat = lat,
                lon = lon,
                serial = dev.Serial,
                keys = new string[]
                {
                    "@@serial",
                    "@@bangsoxe",
                    "@@DriverId",
                    "@@DriverName",
                    "@@Gplx",
                },
                values = new string[]
                {
                    dev.Serial.ToString(),
                    dev.Bs,
                    driver?.Id.ToString() ?? "",
                    driver?.Name ?? "",
                    driver?.Gplx ?? ""
                }
            };

            _queueAlarmEmailRequest.Enqueue(postdata);
        }


        /// <summary>
        /// PushOver10HAlarmEmail
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="driver"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        public void PushOver10HAlarmEmail(Device dev, Driver driver, float lat, float lon)
        {
            if (dev == null) return; if (!dev.EmailAlarm) return; if (String.IsNullOrWhiteSpace(dev.EmailAddess)) return;

            var postdata = new AlarmEmailRequest()
            {
                template = "chaylientuc10gio.html",
                to = dev.EmailAddess,
                lat = lat,
                lon = lon,
                serial = dev.Serial,
                keys = new string[]
                {
                    "@@serial",
                    "@@bangsoxe",
                    "@@DriverId",
                    "@@DriverName",
                    "@@Gplx",
                },
                values = new string[]
                {
                    dev.Serial.ToString(),
                    dev.Bs,
                    driver?.Id.ToString() ?? "",
                    driver?.Name ?? "",
                    driver?.Gplx ?? ""
                }
            };

            _queueAlarmEmailRequest.Enqueue(postdata);
        }


        private void SendEmails()
        {
            _log.Info("Cache Log", $"SendEmails");
            //process
            while (true)
            {
                //delay
                Thread.Sleep(SENDEMAIL_INTERVAL);
                //xử lý
                SendOverSpeedAlarmEmail();
            }
        }

        /// <summary>
        /// SendOverSpeedAlarmEmail
        /// </summary>
        private void SendOverSpeedAlarmEmail()
        {
            if (_queueAlarmEmailRequest.IsEmpty) return;

            //gui moi lan 1 email, gui cho den khi nào het thi thoi
            AlarmEmailRequest postdata;
            while (_queueAlarmEmailRequest.TryDequeue(out postdata))
            {
                //if(DateTime.Now.Subtract(LasttimeSendEmail).TotalMilliseconds < SENDEMAIL_RATE)
                //    Task.Delay(1000);
                try
                {
                    _log.Debug("Cache Log", $"SendOverSpeedAlarmEmail {postdata.serial}");

                    var alarmEmailResponse = new ForwardApi().Post<AlarmEmailResponse>($"{ResponseDataConfig.GeoServerUrl}/email/sendlocation", postdata);
                    if (alarmEmailResponse != null && alarmEmailResponse.Status>0)
                    {
                        //LasttimeSendEmail = DateTime.Now;
                        _log.Info("Cache Log", $"Đã gửi email thành công xe {postdata.serial} tới email {postdata.to}");
                    }
                    else
                    {
                        _log.Error("Cache Log", $"Đã gửi email thất bại xe {postdata.serial} tới email {postdata.to} lỗi {alarmEmailResponse.Description}");
                    }
                }
                catch (Exception exception)
                {
                    _log.Exception("Cache Log", exception, $"SendOverSpeedAlarmEmail");
                }
            }

        }



    }

    /// <summary>
    /// AlarmEmailRequest : cấu trúc dùng để gửi email
    /// </summary>
    public class AlarmEmailRequest
    {
        /// <summary>
        /// chỉ xài nội bộ
        /// </summary>
        public long serial;
        /// <summary>
        /// Địa chỉ email cần gửi ( có thể gửi nhiều email cách nhau , ;  )
        /// </summary>
        public string   to;

        /// <summary>
        /// Tọa độ lat
        /// </summary>
        public float lat;
        /// <summary>
        /// Tọa đọ lon
        /// </summary>
        public float lon;
        /// <summary>
        /// Tên file email mẫu
        /// </summary>
        public string template;
        /// <summary>
        /// Bộ biến số chèn vô email mẫu
        /// </summary>
        public string[] keys;
        /// <summary>
        /// Bộ giá trị của biến keys, kích thước giống keys
        /// </summary>
        public string[] values;
    }

    /// <summary>
    /// Cấu trúc kết quả trả lời sau khi gửi email
    /// </summary>
    public class AlarmEmailResponse
    {
        /// <summary>
        /// giá trị 0 là lỗi , 1 là thành công
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Mô tả kết quả
        /// </summary>
        public string Description { get; set; }
    }

}