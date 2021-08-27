#region header

// /*********************************************************************************************/
// Project :DataTranport
// FileName : BGTTranport.cs
// Time Create : 2:08 PM 17/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Datacenter.DataTranport.Models;
using Log;
using ProtoBuf;
using RabbitMQ.Client;
using ufms;
using Datacenter.Model.Utils;
using System.Collections.Concurrent;

namespace Datacenter.DataTranport
{
    /// <summary>
    ///     truyền thông tin lên tổng cục đường bộ
    /// </summary>
    [Export(typeof(IBgtTranport))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BgtTranport : IPartImportsSatisfiedNotification, IBgtTranport
    {
        private const int SAVE_LIMIT = 100000;
        private readonly List<byte[]> _allData = new List<byte[]>();
        private readonly object _listLock = new object();
        private IConnection _connection;
        private bool _isComplete;
        public static readonly string BgtUser = "ctngoisaosg"; //ConfigurationManager.AppSettings["bgtUser"];
        public static readonly string BgtPass = ""; //ConfigurationManager.AppSettings["bgtPass"];
        public static string BgtIp = "127.0.0.1";// ConfigurationManager.AppSettings["bgtIp"];
        public static readonly int BgtPort = 5676;//5674;//int.Parse(ConfigurationManager.AppSettings["bgtPort"]);
        [Import] private ILog _log;

        private string configpath;
        private List<String> tracks = new List<string>();

        private readonly Timer _timeSend = new Timer(20000);

        private readonly ConcurrentDictionary<long,DeviceTemp> _allDeviceTemps = new ConcurrentDictionary<long,DeviceTemp>();

        private volatile bool _hasSavedFile = true;

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            _timeSend.AutoReset = true;
            _timeSend.Elapsed += _timeSend_Elapsed;
            _timeSend.Enabled = true;

            OpenConnect();

            _timeSend_bus.AutoReset = true;
            _timeSend_bus.Elapsed += _timeSend_bus_Elapsed;
            _timeSend_bus.Enabled = true;
            OpenConnect_buss();
        }

        public void setConfigPath(string datapath, string configpath)
        {
            this.datapath = datapath;
            this.configpath = configpath;
            String path = Path.Combine(configpath, "BgtTranport.txt");
            tracks.Clear();
            if (!File.Exists(path))
            {
                _log.Info("BGTTranport", $"File {path} does not exists, only track1");
                tracks.Add("track1");
                return;
            }

            //load config
            try
            {
                String[] alllines = File.ReadAllLines(path);
                foreach (var line in alllines)
                {
                    if (String.IsNullOrWhiteSpace(line)) continue;
                    if (line.Trim().StartsWith("#")) continue;
                    tracks.Add(line.Trim());
                }
                _log.Info("BGTTranport", "TRACKS : " + String.Join(",",tracks.ToArray()));
            }
            catch (Exception e)
            {
                _log.Exception("BGTTranport",e, "setConfigPath ");
            }
        }

        private DateTime _lasTime = DateTime.Now;
        private void _timeSend_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_isComplete)
            {
                _log.Fatal("BGTTranport", "hệ thống truyền dữ liệu đang bận");
                if ((DateTime.Now - _lasTime).TotalMinutes > 3)
                    _isComplete = false;
                return;
            }

            //kiem tra co du lieu moi
            int dataCount = 0;
            lock (_listLock)
            {
                dataCount = _allData.Count;
            }

            //load tu cache neu co
            if (_hasSavedFile && _connection != null && _connection.IsOpen && dataCount < 5000)
            {
                LoadToMemory();
                lock (_listLock)
                {
                    dataCount = _allData.Count;
                }
            }

            if (dataCount == 0) return;

            //xu ly truyen
            _isComplete = true;
            try
            {
                if (_connection == null)
                    OpenConnect();
                if (_connection != null)
                {
                    if (!_connection.IsOpen)
                        OpenConnect();

                    IModel[] channels = new IModel[tracks.Count];
                    int trackcounterready = 0;
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        try
                        {
                            channels[i] = _connection.CreateModel();
                            trackcounterready++;
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("BGTTranport", ex, $"tạo channel {tracks[i]} kết nối lên server bộ không thành công");
                        }
                    }


                    if (trackcounterready>0)
                    {
                        byte[][] arr;
                        lock (_listLock)
                        {
                            arr = _allData.ToArray();
                            _allData.Clear();
                        }

                        //tách ra thành trackcounterready thành phần
                        List<IEnumerable<byte[]>> arrs;
                        arrs = Split<byte[]>(arr, 1 + arr.Length/trackcounterready ).ToList();

                        int arrindex = 0;
                        for (int i = 0; i < tracks.Count; i++)
                        {
                            if (channels[i] == null) continue;

                            var st = new Stopwatch();
                            st.Start();
                            
                            _log.Fatal("BGTTranport", $"Bắt đầu truyền {arrs[arrindex].Count()} record lên bộ ở kênh {tracks[i]}");
                            int count = 0;
                            foreach (var d in arrs[arrindex])
                            {
                                channels[i].BasicPublish("tracking.ctngoisaosg", tracks[i], null, d);
                                count++;
                            }
                            channels[i].Close();
                            st.Stop();
                            _lasTime = DateTime.Now;
                            _log.Success("BGTTranport", $"Truyền thành công {count} record lên server bộ! ({st.ElapsedMilliseconds} ms) ở kênh {tracks[i]}");
                            arrindex++;
                        }


                        #region  update device temp status
                        foreach (var item in _allDeviceTemps.Values)
                        {
                            item.BgtvCountTransfer = item.BgtvCount;
                            item.BgtvLastTransfer = DateTime.Now;
                        }
                        _allDeviceTemps.Clear();
                        #endregion  update device temp status

                    }//if (trackcounterready>0)

                }
            }
            catch (Exception ex)
            {
                _log.Exception("BGTTranport", ex, "_timeSend_Elapsed ");
            }
            finally
            {
                _isComplete = false;
            }
        }

        private void OpenConnect()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    UserName = BgtUser,
                    Password = BgtPass,
                    VirtualHost = "/",
                    Protocol = Protocols.DefaultProtocol,
                    HostName = BgtIp,
                    Port = BgtPort
                };
                _connection = factory.CreateConnection();
                _log.Success("BGTTranport", "Tạo kết nối với server bộ thành công !");
            }
            catch (Exception ex)
            {
                _log.Exception("BGTTranport", ex, "Mở kết nối lên server bộ giao thông không thành công");
                SaveFromMemory();
            }
        }

        private void SaveFromMemory()
        {
            byte[][] savebytes = null;
            lock (_listLock)
            {
                if (_allData.Count >= SAVE_LIMIT)
                {
                    savebytes = _allData.ToArray();
                    _allData.Clear();
                }
            }

            if(savebytes!=null)
            {
                String path = Path.Combine(datapath, "Bgt");
                String filepath = Path.Combine(path, DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));
                try
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    if (File.Exists(filepath)) File.Delete(filepath);
                    using (FileStream fs = File.Create(filepath))
                    using (BinaryWriter stream = new BinaryWriter(fs))
                    {
                        int n = savebytes.Length;
                        stream.Write(n);
                        foreach (var obj in savebytes)
                        {
                            stream.Write(obj.Length);
                            stream.Write(obj);
                        }
                    }

                    _hasSavedFile = true;
                }
                catch (Exception e)
                {
                    _log.Exception("BgtTranport", e, $"SaveFromMemory {filepath}");
                }
            }
        }

        private void LoadToMemory()
        {
            String path = Path.Combine(datapath, "Bgt");
            if (!Directory.Exists(path))
            {
                _hasSavedFile = false;
                return;
            }
            String[] lspath = Directory.GetFiles(path);
            if (lspath.Length == 0)
            {
                _hasSavedFile = false;
                return;
            }

            String filepath = lspath[lspath.Length-1];
            List<byte[]> loadbytes=null;
            try
            {
                using (FileStream fs = File.OpenRead(filepath))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int n = stream.ReadInt32();
                    loadbytes = new List<byte[]>(n);
                    for (int i = 0; i < n; i++)
                    {
                        int len = stream.ReadInt32();
                        loadbytes.Add(stream.ReadBytes(len));
                    }
                }

                if (File.Exists(filepath)) File.Delete(filepath);
            }
            catch (Exception e)
            {
                _log.Exception("BgtTranport", e, $"LoadToMemory {filepath}");
            }

            if(loadbytes!=null && loadbytes.Count > 0)
            {
                lock (_listLock)
                {
                    _allData.AddRange(loadbytes);
                }
            }
        }


        public bool Push(long serial,DeviceInfo his,DeviceTemp devicetemp)
        {
            if (his == null) return false;
            if (his.Speed > 180)
                return false;
            if (string.IsNullOrEmpty(his.TaxiNumber))
                return false;
            if (his.TaxiNumber[0] == ' ')
                return false;

            Push_buss(serial, his, devicetemp);

            try
            {
                var wp = new WayPoint
                {
                    driver = his.SerialDriver,
                    vehicle = his.TaxiNumber.Replace("-", "").Replace(".", "").Replace(" ", "").Trim(),
                    y = his.NewLat,
                    x = his.NewLng,
                    datetime = DateTimeToUnixTimestamp(his.TimeUpdateInClient),
                    speed = his.Speed,
                    door = his.IsOpenDoor,
                    ignition = his.IsOpenKey,
                    aircon = his.IsRunAirconditioner,
                    heading =
                                    (float)
                                        CalculateHeading(his.OldLat, his.OldLong, his.NewLat,
                                            his.NewLng)
                };
                var msg = new BaseMessage { msgType = BaseMessage.MsgType.WayPoint };

                Extensible.AppendValue(msg, BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
                var data = Serialize(msg);

                lock (_listLock)
                {
                    _allData.Add(data);
                }

                //update devicetemp
                if (devicetemp!=null)
                {
                    devicetemp.BgtvCount++;
                    _allDeviceTemps.TryAdd(serial, devicetemp);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception("BGTTranport", ex, "Mở kết nối lên server bộ giao thông không thành công");
            }

            return false;
        }

        private int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var span = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTime = span.TotalSeconds;
            return (int) unixTime; // 1371802570000
        }

        public byte[] Serialize(BaseMessage wp)
        {
            byte[] b = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, wp);
                b = new byte[ms.Position];
                var fullB = ms.GetBuffer();
                Array.Copy(fullB, b, b.Length);
            }
            return b;
        }

        public static double CalculateHeading(double lat1, double long1, double lat2, double long2)
        {
            var a = lat1*Math.PI/180;
            var b = long1*Math.PI/180;
            var c = lat2*Math.PI/180;
            var d = long2*Math.PI/180;

            if (Math.Cos(c)*Math.Sin(d - b) == 0)
                if (c > a)
                    return 0;
                else
                    return 180;
            var angle = Math.Atan2(Math.Cos(c)*Math.Sin(d - b),
                Math.Sin(c)*Math.Cos(a) - Math.Sin(a)*Math.Cos(c)*Math.Cos(d - b));
            return (angle*180/Math.PI + 360)%360;
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        //public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        public static IEnumerable<IEnumerable<T>> Split<T>(T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        private string datapath;
        private IConnection _connection_BUS;
        private readonly List<byte[]> _allData_BUS = new List<byte[]>();
        private readonly object _listLock_BUSS = new object();
        public static readonly string BgtUser_BUS = "sgstar";
        public static readonly string BgtPass_BUS = "Sgstar";
        public static readonly string BgtIp_BUS = "120.72.102.13";
        public static readonly int BgtPort_BUS = 5672;
        public static readonly string BgtExchange_BUS = "sgstar";


        /// <summary>
        /// Danh sách cần truyền qua bus
        /// </summary>
        private readonly ConcurrentBag<long> BusSerials = new ConcurrentBag<long>();

        private DateTime lasttime_bus = DateTime.MinValue;

        private bool _isComplete_bus;
        private readonly Timer _timeSend_bus = new Timer(20000);

        private bool Push_buss(long serial, DeviceInfo his, DeviceTemp devicetemp)
        {
            if (!BusSerials.Contains(serial)) return false;

            try
            {
                var wp = new UFMS_BUS.BusWayPoint
                {
                    driver = his.SerialDriver,
                    vehicle = his.TaxiNumber.Replace("-", "").Replace(".", "").Replace(" ", "").Trim(),
                    y = his.NewLat,
                    x = his.NewLng,
                    datetime = DateTimeToUnixTimestamp(his.TimeUpdateInClient),
                    speed = his.Speed,
                    door_down = his.IsOpenDoor,
                    door_up = his.IsOpenDoor,
                    ignition = his.IsOpenKey,
                    aircon = his.IsRunAirconditioner,
                    heading =
                                    (float)
                                        CalculateHeading(his.OldLat, his.OldLong, his.NewLat,
                                            his.NewLng)
                };
                var msg = new UFMS_BUS.BaseMessage { msgType = UFMS_BUS.BaseMessage.MsgType.BusWayPoint };

                Extensible.AppendValue(msg, UFMS_BUS.BaseMessage.MsgType.BusWayPoint.GetHashCode(), wp);
                var data = Serialize(msg);

                lock (_listLock_BUSS)
                {
                    _allData_BUS.Add(data);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception("BGTTranport", ex, "Mở kết nối lên hãng xe BUS không thành công");
            }

            return false;
        }

        private byte[] Serialize(UFMS_BUS.BaseMessage wp)
        {
            byte[] b = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, wp);
                b = new byte[ms.Position];
                var fullB = ms.GetBuffer();
                Array.Copy(fullB, b, b.Length);
            }
            return b;
        }

        private void OpenConnect_buss()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    UserName = BgtUser_BUS,
                    Password = BgtPass_BUS,
                    VirtualHost = "/",
                    Protocol = Protocols.DefaultProtocol,
                    HostName = BgtIp_BUS,
                    Port = BgtPort_BUS
                };
                _connection_BUS = factory.CreateConnection();
                _log.Success("BGTTranport", "Tạo kết nối với server Bus thành công !");
            }
            catch (Exception ex)
            {
                _log.Exception("BGTTranport", ex, "Mở kết nối lên server bus không thành công");
            }
        }

        private DateTime _lasTime_bus = DateTime.Now;
        private void _timeSend_bus_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshBusList();

            if (_isComplete_bus)
            {
                _log.Fatal("BGTTranport", "hệ thống truyền dữ liệu BUS đang bận");
                if ((DateTime.Now - _lasTime_bus).TotalMinutes > 3)
                    _isComplete_bus = false;
                return;
            }

            //kiem tra co du lieu moi
            lock (_listLock_BUSS)
            {
                if (_allData_BUS.Count == 0) return;
            }

            //xu ly truyen
            _isComplete_bus = true;
            try
            {
                if (_connection_BUS == null)
                    OpenConnect_buss();
                if (_connection_BUS != null)
                {
                    if (!_connection_BUS.IsOpen)
                        OpenConnect_buss();

                    IModel channel;
                    try
                    {
                        channel = _connection_BUS.CreateModel();
                    }
                    catch (Exception ex)
                    {
                        _log.Exception("BGTTranport", ex, $"tạo channel kết nối lên server bus không thành công");
                        return;
                    }


                    byte[][] arr;
                    lock (_listLock_BUSS)
                    {
                        arr = _allData_BUS.ToArray();
                        _allData_BUS.Clear();
                    }

                    var st = new Stopwatch();
                    st.Start();

                    _log.Fatal("BGTTranport", $"Bắt đầu truyền {arr.Count()} record lên server BUS");
                    int count = 0;
                    foreach (var d in arr)
                    {
                        channel.BasicPublish(BgtExchange_BUS, "", null, d);
                        count++;
                    }
                    channel.Close();

                    st.Stop();
                    _lasTime_bus = DateTime.Now;
                    _log.Success("BGTTranport", $"Truyền thành công {count} record lên server BUS ! ({st.ElapsedMilliseconds} ms)");
                }
            }
            catch (Exception ex)
            {
                _log.Exception("BGTTranport", ex, "_timeSend_bus_Elapsed ");
            }
            finally
            {
                _isComplete_bus = false;
            }
        }

        private void RefreshBusList()
        {
            String path = Path.Combine(datapath, "BgtBUS.txt");
            if (!File.Exists(path))
            {
                _log.Info("BGTTranport", $"File {path} does not exists");
                //clean old list
                if (BusSerials.Count > 0) { long serial; while (!BusSerials.IsEmpty) { BusSerials.TryTake(out serial); } }
                return;
            }

            DateTime lasttime = File.GetLastWriteTime(path);
            if (lasttime <= lasttime_bus) return;

            //clean old list
            if (BusSerials.Count > 0){long serial; while (!BusSerials.IsEmpty) { BusSerials.TryTake(out serial); }}
            //load config
            try
            {
                lasttime_bus = lasttime;
                foreach (String serial in File.ReadAllLines(path))
                {
                    if (String.IsNullOrWhiteSpace(serial)) continue;
                    if (serial.Trim().StartsWith("#")) continue;
                    BusSerials.Add(long.Parse(serial));
                }
                _log.Info("BGTTranport", $"BusSerials.Count {BusSerials.Count}");
            }
            catch (Exception e)
            {
                _log.Exception("BGTTranport", e, "RefreshBusList ");
            }
        }
     
    }
}