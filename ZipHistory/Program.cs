using ConfigFile;
using CorePacket;
using DaoDatabase;
using DaoDatabase.AutoMapping;
using Datacenter.Api.Models;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Log.ZipLog;
using Datacenter.Model.Utils;
using DevicePacketModels;
using DevicePacketModels.Utils;
using Newtonsoft.Json;
using NHibernate;
using StarSg.Utils.Models.DatacenterResponse.Enterprise;
using StarSg.Utils.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZipHistory
{
    class Program
    {
        static void Main(string[] args)
        {

            //double asss= StarSg.Utils.Geos.GeoUtil.Distance(10.80724, 106.7545, 11.80724, 106.7545); 100km
            //double asss1 = StarSg.Utils.Geos.GeoUtil.Distance(10.80724, 106.7545, 10.80724, 107.7545); 100km
            //double asss2 = StarSg.Utils.Geos.GeoUtil.Distance(10.80724, 106.7545, 11.80724, 107.7545); 150km

            //double asss3 = StarSg.Utils.Geos.GeoUtil.Distance(10.30724, 106.4545, 10.60724, 106.4545); 33km
            //double asss4 = StarSg.Utils.Geos.GeoUtil.Distance(10.30724, 106.4545, 10.30724, 106.7545); 33km 
            //double asss5 = StarSg.Utils.Geos.GeoUtil.Distance(10.30724, 106.4545, 10.60724, 106.7545);48km

            

            TestPacket();

            //filterByText(@"d:\datatest\log6_10_2019  Info.txt", "10041909");
            //filterByText(@"d:\datatest\log1_10_2019  Info.txt");

            //LoadCacheData(@"d:\SAGOSTAR\devicetemp.bin");

            ////TestLoadSaveCacheData();
            //TestCacheCompare(@"d:\SAGOSTAR\devicetemp.bin", @"d:\SAGOSTAR\devicetemp1.bin");
            ////TestSleep();
            ////TestInsertBatch();

            ////TestFind();

            //TestMongdb();

            //FuelSheet x = new FuelSheet();
            //x.BarrelType = 1;
            //x.ParamList = new int[] { 1000,500,2000};


            //FuelSheet y = new FuelSheet();
            //y.BarrelType = 2;
            //y.ParamList = new int[] { 3100,1585 };

            //String aa = File.ReadAllText(@"C:\Users\Admin\Desktop\abc.txt");
            //String bb = aa.CompressDeflate();
            //String cc = bb.DecompressDeflate();

            //File.WriteAllText(@"C:\Users\Admin\Desktop\abc1.txt", bb);

            //String dd = aa.CompressDeflateBase64();
            //String ee = dd.DecompressDeflateBase64();
            //File.WriteAllText(@"C:\Users\Admin\Desktop\abc2.txt", dd);

            //while (true)
            //{
            //testFuelZip();
            //    Console.ReadLine();
            //}

            //Console.WriteLine(new List<String>().Capacity);

            //testFuelZipAll();


            //load Datacenter.Api.xml
            //RunUnZip(args[0]);

            RunZipAgain();

            //LoadTrip();

            //List<String> phonenos = "  0989701361 ".Split('|', ',', ';').Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => m.Trim()).ToList();

            //DateTime t1 =  DateTime.ParseExact("2017-11-11 23:27:19", "yyyy-MM-dd HH:mm:ss",CultureInfo.CurrentCulture);
            ////DateTime t2 = DateTime.ParseExact("2017-11-12 13:18:04", "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
            //DateTime t2 = DateTime.ParseExact("2017-11-12 13:18:04", "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);

            //String outpath = "";
            //if (args.Length > 0) outpath = args[0];

            //RunZip(outpath);
            //RunZipTrace(outpath);

            //int notest = 10;
            //if (args.Length > 0) notest = int.Parse(args[0]);

            //TestInsertDeviceLog(notest);

            //Console.ReadLine();
        }

        #region Nén và giải nén DeviceLog

        /// <summary>
        /// Gọi hàm này để chạy nén dữ liệu chép qua SgsiDataArchive và đồng thời lưu dữ liệu cả ngày vô file nén
        /// </summary>
        /// <param name="backuppath">chưa mục chứa file nén</param>
        public static void RunZip(String backuppath)
        {
            string Path = "Datacenter.Api.xml";

            ConfigManager _cfg = new ConfigManager();
            ResponseDataConfig Config = _cfg.Read<ResponseDataConfig>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path));
            Config.Fix();



            //Đăng kí SgsiData
            var mapEntity = Assembly.GetAssembly(typeof(Company))
                               .GetTypes()
                               .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                               .Select(
                                   m =>
                                   {
                                       var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                       return makeme;
                                   }).ToList();

            UnitOfWorkFactory.RegisterDatabase("SgsiData", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, Config.MotherSql.DataName,
                            Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            //Đăng kí SgsiDataArchive
            var mapZip = Assembly.GetAssembly(typeof(Company))
                           .GetTypes()
                           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                           .Select(
                               m =>
                               {
                                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                   return makeme;
                               }).ToList();
            String zipdbname = Config.MotherSql.DataName + "Archive";
            UnitOfWorkFactory.RegisterDatabase("SgsiDataArchive", new NhibernateConfig
            {
                Maps = mapZip,
                Config =
                       DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                           Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer);
            IDictionary<Guid, Device> dic = dataContext.GetAll<Guid, Device>(m => m.Indentity);
            dataContext.Dispose();

            //TestZipDeviceLog(dic, new DateTime(2017, 1, 1), new DateTime(2017, 2, 28));
            // chuyển thời gian về ngày hôm qua , task này luôn chạy sau 0h nên dữ liệu phải đọc của ngày hôm trước
            //ZipDeviceLog(backuppath, dic, DateTime.Now);

            Stopwatch w = new Stopwatch();
            w.Start();
            ZipDeviceLog(backuppath, dic, DateTime.Now.AddDays(-1),true);
            w.Stop();
            Console.WriteLine( "Total miliseconds " + w.ElapsedMilliseconds );

            Console.WriteLine("Done process");
        }

        /// <summary>
        /// Nén dữ liệu trong một ngày của danh sách thiết bị đưa vào
        /// </summary>
        /// <param name="backuppath">thư mục chứa file nén</param>
        /// <param name="dic">danh sách thiết bị</param>
        /// <param name="rundate">ngày chạy</param>
        /// <param name="updateHistory">cập nhật lại dữ liệu cũ (dành cho dữ liệu không quá 1 tuần)</param>
        private static void ZipDeviceLog(String backuppath, IDictionary<Guid, Device> dic, DateTime rundate, bool updateHistory=false)
        {
            Console.WriteLine("PROCESS DATE " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));

            var now = rundate;// new DateTime(2017, 02, 22, 0, 0, 1);
            var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
            var end = begin.AddDays(1); // cuối ngày

            FileStream fs = null;
            BinaryWriter bw = null;

            if (!String.IsNullOrWhiteSpace(backuppath))
            {
                try
                {
                    if (!Directory.Exists(backuppath))
                        Directory.CreateDirectory(backuppath);
                    if (!Directory.Exists(backuppath)) backuppath = null;
                    if (backuppath != null)
                        backuppath = Path.Combine(backuppath, begin.ToString("yyyy-MM-dd") + ".dat");
                    if (File.Exists(backuppath)) File.Delete(backuppath);

                    if (backuppath != null)
                    {
                        fs = new FileStream(backuppath, FileMode.Create, FileAccess.Write, FileShare.None, 102400);
                        bw = new BinaryWriter(fs);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("LOI TAO THU MUC" + ex.Message);
                }
            }

            if (bw != null)
            {
                bw.Write(1);//version
                bw.Write(dic.Keys.Count);//count
            }

            foreach (Guid guid in dic.Keys)
            {
                try
                {
                    //Console.WriteLine($"process : {dic[guid].Serial}");
                    //Guid guid = Guid.Parse("6619A23A-DE45-4F68-B34F-B35A2474FF4C");//17010029

                    using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer))
                    using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive", DbSupportType.MicrosoftSqlServer))
                    {
                        ICollection<DeviceLogMoving> allLogOfMoving;
                        if(updateHistory)//đọc log (cả history)
                            allLogOfMoving =
                                dataContext.GetWhere<DeviceLogMoving>(
                                    m =>
                                        m.DeviceStatus.ClientSend < end && //m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend < end && 
                                        m.Indentity == guid);
                        else//chỉ đọc trong ngày
                            allLogOfMoving =
                                dataContext.GetWhere<DeviceLogMoving>(
                                    m =>
                                        m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend < end && 
                                        m.Indentity == guid);

                        //check data nén
                        if (allLogOfMoving.Count == 0)
                        {
                            //Console.WriteLine("Count == 0");
                            if (bw != null) bw.Write(false);//Empty Row
                            continue;
                        }

                        bool hasHistory = updateHistory && allLogOfMoving.Count(m => m.DeviceStatus.ClientSend < begin) > 0;
                        ICollection<DeviceLogMoving> allLog;
                        DeviceLogMoving first;
                        byte[] data;

                        #region  xử lý dữ liệu cho rundate
                        //nếu có dữ liệu cũ
                        if (hasHistory)
                            allLog = allLogOfMoving.Where(m => m.DeviceStatus.ClientSend >= begin).ToList();
                        else//nếu chỉ có dữ liệu mới
                            allLog = allLogOfMoving;

                        if(allLog.Count>0)
                        {
                            first = allLog.FirstOrDefault();

                            // nén dữ liệu lại 
                            //todo: do một số trường trong record ko cần thiết nên tạo thêm 1 lớp chuyển đổi nữa
                            //var data = allLog.ObjectToByteArray().Zip();
                            DeviceLogCollection sellog = new DeviceLogCollection(allLog);
                            data = sellog.Serializer().Zip();

                            // lưu vào bảng mới
                            dataContextZip.Insert<DeviceLogZip>(new DeviceLogZip
                            {
                                //Id = first.Id,
                                Data = data,
                                Serial = first.Serial,
                                TimeUpdate = now,
                                DbId = 0,
                                CompanyId = first.CompanyId,
                                GroupId = first.GroupId,
                                Indentity = guid,
                            });
                            dataContextZip.Commit();

                            //Lưu vào backup
                            if (bw != null)
                            {
                                bw.Write(true);
                                bw.Write(first.Serial);
                                bw.Write(first.CompanyId);
                                bw.Write(first.GroupId);
                                bw.Write(guid.ToString());
                                bw.Write(data.Length);
                                bw.Write(data);
                            }

                        }

                        #endregion  xử lý dữ liệu cho rundate

                        #region  xử lý dữ liệu quá khứ nếu có ( trong vòng 7 ngày )
                        if (hasHistory)
                        {
                            for (int i = 1; i <= 7; i++)
                            {
                                DateTime begin0 = begin.AddDays(-i);//lùi i ngày
                                DateTime end0 = begin0.AddDays(1);//cuối ngày

                                allLog = allLogOfMoving.Where(m => m.DeviceStatus.ClientSend >= begin0 && m.DeviceStatus.ClientSend < end0).ToList();
                                if (allLog.Count == 0) break;//kết thúc nếu hết dữ liệu cũ

                                first = allLog.FirstOrDefault();

                                //lấy nội dung data cũ lên
                                List<DeviceLogMoving> oldlog = null;
                                var tmp = dataContextZip.GetWhere<DeviceLogZip>(m => m.Serial == first.Serial && m.TimeUpdate >= begin0 && m.TimeUpdate < end0).ToList();
                                if (tmp.Count > 0 && tmp[0] != null)
                                {
                                    DeviceLogAgainCollection seloldlog = new DeviceLogAgainCollection();
                                    seloldlog.Deserializer(tmp[0].Data.UnZip());
                                    if (seloldlog.listout != null && seloldlog.listout.Count > 0)
                                        oldlog = seloldlog.listout;
                                }

                                // lưu vào bảng mới
                                if (oldlog == null)
                                {
                                    // nén dữ liệu lại 
                                    DeviceLogAgainCollection selloghis = new DeviceLogAgainCollection(allLog);
                                    data = selloghis.Serializer().Zip();

                                    dataContextZip.Insert<DeviceLogZip>(new DeviceLogZip
                                    {
                                        //Id = first.Id,
                                        Data = data,
                                        Serial = first.Serial,
                                        TimeUpdate = begin0,
                                        DbId = 0,
                                        CompanyId = first.CompanyId,
                                        GroupId = first.GroupId,
                                        Indentity = guid,
                                    });
                                }
                                //cập nhật lại 
                                else
                                {
                                    //add new log
                                    oldlog.AddRange(allLog);
                                    // nén dữ liệu lại 
                                    DeviceLogAgainCollection selloghis = new DeviceLogAgainCollection(oldlog);
                                    data = selloghis.Serializer().Zip();

                                    //update data
                                    tmp[0].Data = data;
                                    dataContextZip.Update<DeviceLogZip>(tmp[0]);
                                }
                                dataContextZip.Commit();

                            }//end for (int i = 1; i <= 7; i++)
                        }
                        #endregion  xử lý dữ liệu quá khứ nếu có ( trong vòng 7 ngày )

                        //Console.WriteLine($"done for : {dic[guid].Serial}");
                    }
                }
                catch (Exception e)
                {
                    if (bw != null) bw.Write(false);//Empty Row
                    Console.WriteLine($"Lỗi nén data device: {dic[guid].Serial} ERROR: {e.Message} TRACE : {e.StackTrace}");
                }
            }

            if (bw != null) bw.Dispose();
            if (fs != null) fs.Dispose();

            Console.WriteLine("done date " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        ///// <summary>
        ///// Nén dữ liệu trong một ngày của danh sách thiết bị đưa vào
        ///// </summary>
        ///// <param name="backuppath">thư mục chứa file nén</param>
        ///// <param name="dic">danh sách thiết bị</param>
        ///// <param name="rundate">ngày chạy</param>
        //private static void ZipDeviceLog(String backuppath, IDictionary<Guid, Device> dic, DateTime rundate)
        //{
        //    Console.WriteLine("PROCESS DATE " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));

        //    var now = rundate;// new DateTime(2017, 02, 22, 0, 0, 1);
        //    var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
        //    var end = begin.AddDays(1); // cuối ngày

        //    FileStream fs = null;
        //    BinaryWriter bw = null;
        //    try
        //    {
        //        if (!Directory.Exists(backuppath))
        //            Directory.CreateDirectory(backuppath);
        //        if (!Directory.Exists(backuppath)) backuppath = null;
        //        if (backuppath != null)
        //            backuppath = Path.Combine(backuppath, begin.ToString("yyyy-MM-dd") + ".dat");
        //        if (File.Exists(backuppath)) File.Delete(backuppath);

        //        if (backuppath != null)
        //        {
        //            fs = new FileStream(backuppath, FileMode.Create, FileAccess.Write, FileShare.None, 102400);
        //            bw = new BinaryWriter(fs);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("LOI TAO THU MUC" + ex.Message);
        //    }

        //    //Ghi toàn bộ theo so luong devices
        //    int ndevcount = dic.Count;
        //    if (bw != null)
        //    {
        //        bw.Write(1);//version
        //        bw.Write(ndevcount);//count
        //    }

        //    IList<DeviceLogMoving> allLog;
        //    bool isEnd = false; long minkey = 0; int limit = 50000, processrow = 0;//Xử lý mỗi lần N mẫu tin
        //    while (!isEnd)
        //    {
        //        //get data 
        //        using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer))
        //        {
        //            allLog = dataContext.TakeAsc<DeviceLogMoving>(m => m.Serial > minkey, limit, m => m.Serial);
        //        }

        //        //process
        //        int rowcount = allLog.Count;
        //        isEnd = rowcount < limit;//hết dữ liệu

        //        //add to lookup
        //        Dictionary<Guid, List<DeviceLogMoving>> lookup = new Dictionary<Guid, List<DeviceLogMoving>>();
        //        for (int row = 0; row < rowcount; row++)
        //        {
        //            DeviceLogMoving obj = allLog[row];
        //            if (!lookup.ContainsKey(obj.Indentity)) lookup.Add(obj.Indentity, new List<DeviceLogMoving>());
        //            lookup[obj.Indentity].Add(obj);
        //        }

        //        //remove the last miss data
        //        if (!isEnd)
        //            lookup.Remove(allLog[rowcount - 1].Indentity);

        //        //zip and insert to archive
        //        using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive", DbSupportType.MicrosoftSqlServer))
        //        {
        //            var keys = lookup.Keys;
        //            foreach (var guid in keys)
        //            {
        //                try
        //                {
        //                    DeviceLogMoving first = lookup[guid].First();
        //                    if (minkey < first.Serial) minkey = first.Serial;

        //                    // nén dữ liệu lại 
        //                    var listtrace = lookup[guid];//.OrderBy(m => m.DeviceStatus.ClientSend).ToArray();//cần sort không ?
        //                    DeviceLogCollection sellog = new DeviceLogCollection(listtrace);
        //                    var data = sellog.Serializer().Zip();

        //                    // lưu vào bảng mới
        //                    dataContextZip.Insert<DeviceLogZip1>(new DeviceLogZip1
        //                    {
        //                        //Id = first.Id,
        //                        Data = data,
        //                        Serial = first.Serial,
        //                        TimeUpdate = now,
        //                        DbId = 0,
        //                        CompanyId = first.CompanyId,
        //                        GroupId = first.GroupId,
        //                        Indentity = guid
        //                    });
        //                    dataContextZip.Commit();

        //                    //Lưu vào backup
        //                    if (bw != null && processrow < ndevcount)
        //                    {
        //                        bw.Write(true);
        //                        bw.Write(first.Serial);
        //                        bw.Write(first.CompanyId);
        //                        bw.Write(first.GroupId);
        //                        bw.Write(guid.ToString());
        //                        bw.Write(data.Length);
        //                        bw.Write(data);

        //                        processrow++;
        //                    }

        //                    Console.WriteLine($"done for : {dic[guid].Serial}");
        //                }
        //                catch (Exception e)
        //                {
        //                    if (bw != null && processrow < ndevcount)
        //                    {
        //                        bw.Write(false);//Empty Row
        //                        processrow++;
        //                    }
        //                    Console.WriteLine("ZipDeviceTraceLogLogic", e, $"Lỗi nén data device: {dic[guid].Serial}");
        //                }
        //            }//end foreach
        //        }//end using

        //    }//end while

        //    //write remain empty devices
        //    while (bw != null && processrow++ < ndevcount)
        //        bw.Write(false);

        //    if (bw != null) bw.Dispose();
        //    if (fs != null) fs.Dispose();

        //    Console.WriteLine("done date " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));
        //}


        /// <summary>
        /// Hàm này tương tự RunZip nhưng nó được chạy tay để cập nhật lại dữ liệu nén mà ngày hôm trước dữ liệu này chưa xuất hiện bị delay do server không nhận
        /// </summary>
        public static void RunZipAgain()
        {
            string Path = "Datacenter.Api.xml";

            ConfigManager _cfg = new ConfigManager();
            ResponseDataConfig Config = _cfg.Read<ResponseDataConfig>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path));
            Config.Fix();



            //Đăng kí SgsiData
            var mapEntity = Assembly.GetAssembly(typeof(Company))
                               .GetTypes()
                               .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                               .Select(
                                   m =>
                                   {
                                       var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                       return makeme;
                                   }).ToList();

            UnitOfWorkFactory.RegisterDatabase("SgsiData", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, Config.MotherSql.DataName,
                            Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });




            //Đăng kí SgsiDataArchive
            var mapZip = Assembly.GetAssembly(typeof(Company))
                           .GetTypes()
                           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                           .Select(
                               m =>
                               {
                                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                   return makeme;
                               }).ToList();
            String zipdbname = Config.MotherSql.DataName + "Archive";
            UnitOfWorkFactory.RegisterDatabase("SgsiDataArchive", new NhibernateConfig
            {
                Maps = mapZip,
                Config =
                       DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                           Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer);
            IDictionary<Guid, Device> dic = dataContext.GetAll<Guid, Device>(m => m.Indentity);
            dataContext.Dispose();

            // chuyển thời gian về ngày hôm qua , task này luôn chạy sau 0h nên dữ liệu phải đọc của ngày hôm trước
            ZipAgainDeviceLog(dic, DateTime.Now.AddDays(-1));

            Console.WriteLine("Done process");
        }

        /// <summary>
        /// Bõ qua backup files cho dữ liệu bị delay (chạy bằng tay)
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="rundate"></param>
        private static void ZipAgainDeviceLog(IDictionary<Guid, Device> dic, DateTime rundate)
        {
            Console.WriteLine("PROCESS DATE " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));

            var now = rundate;// new DateTime(2017, 02, 22, 0, 0, 1);
            var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
            var end = begin.AddDays(1); // cuối ngày

            foreach (Guid guid in dic.Keys)
            {
                try
                {
                    Console.WriteLine($"process : {dic[guid].Serial}");

                    using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer))
                    using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive", DbSupportType.MicrosoftSqlServer))
                    {
                        // đọc log trong 1 ngày ra
                        var allLog =
                            dataContext.GetWhere<DeviceLogMoving>(
                                m =>
                                    m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend < end &&
                                    m.Indentity == guid);

                        //check data nén
                        if (allLog.Count == 0)
                        {
                            Console.WriteLine("Count == 0");
                            continue;
                        }

                        DeviceLogMoving first = allLog.FirstOrDefault();

                        //lấy nội dung data cũ lên
                        List<DeviceLogMoving> oldlog = null;
                        var tmp = dataContextZip.GetWhere<DeviceLogZip>(m => m.Serial == first.Serial && m.TimeUpdate >= begin && m.TimeUpdate < end).ToList();
                        if (tmp.Count > 0 && tmp[0] != null)
                        {
                            DeviceLogAgainCollection seloldlog = new DeviceLogAgainCollection();
                            seloldlog.Deserializer(tmp[0].Data.UnZip());
                            if (seloldlog.listout != null && seloldlog.listout.Count > 0)
                                oldlog = seloldlog.listout;
                        }

                        // lưu vào bảng mới
                        if (oldlog == null)
                        {
                            // nén dữ liệu lại 
                            DeviceLogAgainCollection sellog = new DeviceLogAgainCollection(allLog);
                            var data = sellog.Serializer().Zip();

                            dataContextZip.Insert<DeviceLogZip>(new DeviceLogZip
                            {
                                //Id = first.Id,
                                Data = data,
                                Serial = first.Serial,
                                TimeUpdate = now,
                                DbId = 0,
                                CompanyId = first.CompanyId,
                                GroupId = first.GroupId,
                                Indentity = guid,
                            });
                        }
                        //cập nhật lại 
                        else
                        {
                            //add new log
                            oldlog.AddRange(allLog);
                            // nén dữ liệu lại 
                            DeviceLogAgainCollection sellog = new DeviceLogAgainCollection(oldlog);
                            var data = sellog.Serializer().Zip();

                            //update data
                            tmp[0].Data = data;
                            dataContextZip.Update<DeviceLogZip>(tmp[0]);
                        }

                        dataContextZip.Commit();

                        Console.WriteLine($"done for : {dic[guid].Serial}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ZipDeviceLogLogic", e, $"Lỗi nén data device: {dic[guid].Serial}");
                }

            }
            Console.WriteLine("done date " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        /// <summary>
        /// Duyệt qua danh sách file nén trong thư mục để phục hồi dữ liệu vào SgsiDataArchive
        /// </summary>
        /// <param name="backuppath"></param>
        public static void RunUnZip(String backuppath)
        {
            if (!Directory.Exists(backuppath))
            {
                Console.WriteLine("INVALID PATH " + backuppath);
                return;
            }

            string Path = "Datacenter.Api.xml";

            ConfigManager _cfg = new ConfigManager();
            ResponseDataConfig Config = _cfg.Read<ResponseDataConfig>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path));
            Config.Fix();

            //Đăng kí SgsiDataArchive
            var mapZip = Assembly.GetAssembly(typeof(Company))
                           .GetTypes()
                           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                           .Select(
                               m =>
                               {
                                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                   return makeme;
                               }).ToList();
            String zipdbname = Config.MotherSql.DataName;
            UnitOfWorkFactory.RegisterDatabase("SgsiDataArchive1", new NhibernateConfig
            {
                Maps = mapZip,
                Config =
                       DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                           Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            foreach (var filepath in Directory.GetFiles(backuppath))
            {
                Console.WriteLine("process " + filepath);
                ReadZipDeviceLog(filepath);
            }

            Console.WriteLine("DONE");
        }

        /// <summary>
        /// Giải nén từ một file nén để thêm lại vào SgsiDataArchive ( dùng để backup từ file nén )
        /// </summary>
        /// <param name="backupfile"></param>
        private static void ReadZipDeviceLog(String backupfile)
        {
            if (!File.Exists(backupfile)) return;
            DateTime processDate = DateTime.ParseExact(Path.GetFileNameWithoutExtension(backupfile), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            processDate = processDate.AddMinutes(5);
            Console.WriteLine("PROCESS DATE " + processDate.ToString("yyyy-MM-dd HH:mm:ss"));

            FileStream fs = null;
            BinaryReader bw = null;
            try
            {
                fs = new FileStream(backupfile, FileMode.Open, FileAccess.Read, FileShare.Read, 102400);
                bw = new BinaryReader(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            int version = bw.ReadInt32();
            int n = bw.ReadInt32();

            //List<DeviceLogZip> ret = new List<DeviceLogZip>(n);
            for (int i = 0; i < n; i++)
            {
                if (!bw.ReadBoolean()) continue;

                DeviceLogZip obj = new DeviceLogZip();
                obj.Serial = bw.ReadInt64();
                obj.CompanyId = bw.ReadInt64();
                obj.GroupId = bw.ReadInt64();
                obj.TimeUpdate = processDate;
                obj.DbId = 0;
                obj.Indentity = Guid.Parse(bw.ReadString());

                int len = bw.ReadInt32();
                obj.Data = bw.ReadBytes(len);


                try
                {
                    using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive1", DbSupportType.MicrosoftSqlServer))
                    {
                        // lưu vào bảng mới
                        dataContextZip.Insert<DeviceLogZip>(obj);
                        dataContextZip.Commit();
                        Console.WriteLine($"done for : {obj.Serial}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadZipDeviceLog", e, $"Lỗi nén data device: {obj.Serial}");
                }
            }

            if (bw != null) bw.Dispose();
            if (fs != null) fs.Dispose();

            Console.WriteLine("done date " + Path.GetFileNameWithoutExtension(backupfile));
        }


        #endregion Nén và giải nén DeviceLog



        #region Nén và giải nén DeviceTraceLog

        /// <summary>
        /// Gọi hàm này để chạy nén dữ liệu trace chép qua SgsiDataArchive và đồng thời lưu dữ liệu cả ngày vô file nén
        /// </summary>
        /// <param name="backuppath">chưa mục chứa file nén</param>
        public static void RunZipTrace(String backuppath)
        {
            string Path = "Datacenter.Api.xml";

            ConfigManager _cfg = new ConfigManager();
            ResponseDataConfig Config = _cfg.Read<ResponseDataConfig>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path));
            Config.Fix();



            //Đăng kí SgsiData
            var mapEntity = Assembly.GetAssembly(typeof(Company))
                               .GetTypes()
                               .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                               .Select(
                                   m =>
                                   {
                                       var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                       return makeme;
                                   }).ToList();

            UnitOfWorkFactory.RegisterDatabase("SgsiData", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, Config.MotherSql.DataName,
                            Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            //Đăng kí SgsiDataArchive
            var mapZip = Assembly.GetAssembly(typeof(Company))
                           .GetTypes()
                           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                           .Select(
                               m =>
                               {
                                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                   return makeme;
                               }).ToList();
            String zipdbname = Config.MotherSql.DataName + "Archive";
            UnitOfWorkFactory.RegisterDatabase("SgsiDataArchive", new NhibernateConfig
            {
                Maps = mapZip,
                Config =
                       DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                           Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer);
            IDictionary<Guid, Device> dic = dataContext.GetAll<Guid, Device>(m => m.Indentity);
            dataContext.Dispose();

            ////Chạy dữ liệu từ ngày new DateTime(2017, 2, 1) đến DateTime.Now.AddDays(-41)
            //DateTime dtfrom = new DateTime(2017, 10, 13);
            //DateTime dtmax = DateTime.Now.AddDays(-10);
            //for (DateTime dt = dtfrom; dt < dtmax; dt = dt.AddDays(1))
            //{
            //    //Stopwatch w = new Stopwatch();
            //    //w.Start();
            //    ZipDeviceTraceLog(backuppath, dic, dt);
            //    //w.Stop();
            //    //Console.WriteLine( "" + w.ElapsedMilliseconds );
            //}

            Stopwatch w = new Stopwatch();
            w.Start();
            ZipDeviceTraceLog(backuppath, dic, DateTime.Now.AddDays(-11));
            w.Stop();
            Console.WriteLine("Total miliseconds " + w.ElapsedMilliseconds);

            Console.WriteLine("Done process");
        }

        /// <summary>
        /// Nén dữ liệu trong một ngày của danh sách thiết bị đưa vào
        /// </summary>
        /// <param name="backuppath">thư mục chứa file nén</param>
        /// <param name="dic">danh sách thiết bị</param>
        /// <param name="rundate">ngày chạy</param>
        private static void ZipDeviceTraceLog(String backuppath, IDictionary<Guid, Device> dic, DateTime rundate)
        {
            Console.WriteLine("PROCESS DATE " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));

            var now = rundate;// new DateTime(2017, 02, 22, 0, 0, 1);
            var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
            var end = begin.AddDays(1); // cuối ngày

            FileStream fs = null;
            BinaryWriter bw = null;

            if (!String.IsNullOrWhiteSpace(backuppath))
            {
                try
                {
                    if (!Directory.Exists(backuppath))
                        Directory.CreateDirectory(backuppath);
                    if (!Directory.Exists(backuppath)) backuppath = null;
                    if (backuppath != null)
                        backuppath = Path.Combine(backuppath, begin.ToString("yyyy-MM-dd") + ".dat");
                    if (File.Exists(backuppath)) File.Delete(backuppath);

                    if (backuppath != null)
                    {
                        fs = new FileStream(backuppath, FileMode.Create, FileAccess.Write, FileShare.None, 102400);
                        bw = new BinaryWriter(fs);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("LOI TAO THU MUC" + ex.Message);
                }
            }

            //Ghi toàn bộ theo so luong devices
            int ndevcount = dic.Count;
            if (bw != null)
            {
                bw.Write(1);//version
                bw.Write(ndevcount);//count
            }

            //Console.WriteLine($"process : {dic[guid].Serial}");
            //Guid guid = Guid.Parse("6619A23A-DE45-4F68-B34F-B35A2474FF4C");//17010029
            IList<DeviceTraceLog> allLog=null;
            bool isEnd = false;long minkey = 0; int limit = 10000, processrow = 0;//Xử lý mỗi lần N mẫu tin
            while (!isEnd)
            {
                //get data 
                using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer))
                {
                    allLog = dataContext.TakeAsc<DeviceTraceLog>(m => m.Serial > minkey && m.BeginTime >= begin && m.BeginTime < end, limit, m => m.Serial);
                }

                //process
                int rowcount = allLog.Count;
                isEnd = rowcount < limit;//hết dữ liệu

                //add to lookup
                Dictionary<Guid, List<DeviceTraceLog>> lookup = new Dictionary<Guid, List<DeviceTraceLog>>();
                for (int row = 0; row < rowcount; row++)
                {
                    DeviceTraceLog obj = allLog[row];
                    if (!lookup.ContainsKey(obj.Indentity)) lookup.Add(obj.Indentity, new List<DeviceTraceLog>());
                    lookup[obj.Indentity].Add(obj);
                }

                //Console.WriteLine($"begin = {begin} end = {end} minkey ={minkey} allLog = {allLog?.Count ?? 0} lookup = {lookup.Count} {lookup[allLog[0].Indentity].Count} {allLog[0].Indentity} {allLog[0].Serial}");

                //check  --> du lieu to qua 10k thi bo qua du lieu nay
                if (lookup.Count == 1 && !isEnd)
                {
                    Console.WriteLine($"THIET BI QUA 10K SU KIEN >> BO QUA begin = {begin} end = {end} minkey ={minkey} allLog = {allLog?.Count ?? 0} lookup = {lookup.Count} {lookup[allLog[0].Indentity].Count} {allLog[0].Indentity} {allLog[0].Serial}");
                    minkey = allLog[0].Serial;
                }

                //remove the last miss data
                if (!isEnd)
                    lookup.Remove(allLog[rowcount - 1].Indentity);

                //zip and insert to archive
                using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive", DbSupportType.MicrosoftSqlServer))
                {
                    var keys = lookup.Keys;
                    foreach (var guid in keys)
                    {
                        try
                        {
                            DeviceTraceLog first = lookup[guid].First();
                            if (minkey < first.Serial) minkey = first.Serial;

                            //Console.WriteLine( $"Serial = {first.Serial} guid={guid.ToString()} isEnd={isEnd}");

                            // nén dữ liệu lại 
                            //todo: do một số trường trong record ko cần thiết nên tạo thêm 1 lớp chuyển đổi nữa
                            //var data = allLog.ObjectToByteArray().Zip();
                            var listtrace = lookup[guid].OrderBy(m => m.BeginTime).ToArray();
                            DeviceTraceLogCollection sellog = new DeviceTraceLogCollection(listtrace);
                            var data = sellog.Serializer().Zip();

                            // lưu vào bảng mới
                            dataContextZip.Insert<DeviceTraceZip>(new DeviceTraceZip
                            {
                                //Id = first.Id,
                                Data = data,
                                //Serial = dic[guid].Serial,
                                Serial = first.Serial,
                                TimeUpdate = now,
                                DbId = 0,
                                //CompanyId = dic[guid].CompanyId,
                                CompanyId = first.CompanyId,
                                //GroupId = dic[guid].GroupId,
                                GroupId = first.GroupId,
                                Indentity = guid,
                            });
                            dataContextZip.Commit();

                            //Lưu vào backup
                            if (bw != null && processrow<ndevcount)
                            {
                                bw.Write(true);
                                //bw.Write(dic[guid].Serial);
                                bw.Write(first.Serial);
                                //bw.Write(dic[guid].CompanyId);
                                bw.Write(first.CompanyId);
                                //bw.Write(dic[guid].GroupId);
                                bw.Write(first.GroupId);
                                bw.Write(guid.ToString());
                                bw.Write(data.Length);
                                bw.Write(data);

                                processrow++;
                            }
                        }
                        catch (Exception e)
                        {
                            if (bw != null && processrow < ndevcount)
                            {
                                bw.Write(false);//Empty Row
                                processrow++;
                            }
                            Console.WriteLine("ZipDeviceTraceLogLogic", e, $"Lỗi nén data device: {dic[guid].Serial}");
                        }
                    }//end foreach
                }//end using

            }//end while

            //write remain empty devices
            while (bw!=null && processrow++ < ndevcount)
                bw.Write(false);

            if (bw != null) bw.Dispose();
            if (fs != null) fs.Dispose();

            Console.WriteLine("done date " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// Hàm này tương tự RunZip nhưng nó được chạy tay để cập nhật lại dữ liệu nén mà ngày hôm trước dữ liệu này chưa xuất hiện bị delay do server không nhận
        /// </summary>
        public static void RunZipTraceAgain()
        {
            string Path = "Datacenter.Api.xml";

            ConfigManager _cfg = new ConfigManager();
            ResponseDataConfig Config = _cfg.Read<ResponseDataConfig>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path));
            Config.Fix();



            //Đăng kí SgsiData
            var mapEntity = Assembly.GetAssembly(typeof(Company))
                               .GetTypes()
                               .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                               .Select(
                                   m =>
                                   {
                                       var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                       return makeme;
                                   }).ToList();

            UnitOfWorkFactory.RegisterDatabase("SgsiData", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, Config.MotherSql.DataName,
                            Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });




            //Đăng kí SgsiDataArchive
            var mapZip = Assembly.GetAssembly(typeof(Company))
                           .GetTypes()
                           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                           .Select(
                               m =>
                               {
                                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                   return makeme;
                               }).ToList();
            String zipdbname = Config.MotherSql.DataName + "Archive";
            UnitOfWorkFactory.RegisterDatabase("SgsiDataArchive", new NhibernateConfig
            {
                Maps = mapZip,
                Config =
                       DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                           Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer);
            IDictionary<Guid, Device> dic = dataContext.GetAll<Guid, Device>(m => m.Indentity);
            dataContext.Dispose();

            // chuyển thời gian về ngày hôm qua , task này luôn chạy sau 0h nên dữ liệu phải đọc của ngày hôm trước
            ZipAgainDeviceTraceLog(dic, DateTime.Now.AddDays(-1));

            Console.WriteLine("Done process");
        }

        /// <summary>
        /// Bõ qua backup files cho dữ liệu bị delay (chạy bằng tay)
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="rundate"></param>
        private static void ZipAgainDeviceTraceLog(IDictionary<Guid, Device> dic, DateTime rundate)
        {
            Console.WriteLine("PROCESS DATE " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));

            var now = rundate;// new DateTime(2017, 02, 22, 0, 0, 1);
            var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
            var end = begin.AddDays(1); // cuối ngày

            foreach (Guid guid in dic.Keys)
            {
                try
                {
                    Console.WriteLine($"process : {dic[guid].Serial}");

                    using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer))
                    using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive", DbSupportType.MicrosoftSqlServer))
                    {
                        // đọc log trong 1 ngày ra
                        var allLog =
                            dataContext.GetWhere<DeviceTraceLog>(
                                m =>
                                    m.BeginTime >= begin && m.BeginTime < end && m.Indentity == guid);

                        //check data nén
                        if (allLog.Count == 0)
                        {
                            Console.WriteLine("Count == 0");
                            continue;
                        }

                        DeviceTraceLog first = allLog.FirstOrDefault();

                        //lấy nội dung data cũ lên
                        var tmp = dataContextZip.GetWhere<DeviceTraceZip>(m => m.Serial == first.Serial && m.TimeUpdate >= begin && m.TimeUpdate < end).FirstOrDefault();

                        // nén dữ liệu lại 
                        DeviceTraceLogCollection sellog = new DeviceTraceLogCollection(allLog);
                        var data = sellog.Serializer().Zip();

                        // lưu vào bảng mới
                        if (tmp == null)
                        {
                            dataContextZip.Insert<DeviceLogZip>(new DeviceLogZip
                            {
                                //Id = first.Id,
                                Data = data,
                                Serial = first.Serial,
                                TimeUpdate = now,
                                DbId = 0,
                                CompanyId = first.CompanyId,
                                GroupId = first.GroupId,
                                Indentity = guid,
                            });
                        }
                        //cập nhật lại 
                        else
                        {
                            dataContextZip.Update<DeviceLogZip>(
                                new DeviceLogZip
                                {
                                    Id = tmp.Id,//cập nhật theo cái ni
                                    Data = data,
                                    Serial = first.Serial,
                                    TimeUpdate = now,
                                    DbId = 0,
                                    CompanyId = first.CompanyId,
                                    GroupId = first.GroupId,
                                    Indentity = guid,
                                }
                                );
                        }

                        dataContextZip.Commit();

                        Console.WriteLine($"done for : {dic[guid].Serial}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ZipDeviceLogLogic", e, $"Lỗi nén data device: {dic[guid].Serial}");
                }

            }
            Console.WriteLine("done date " + rundate.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        /// <summary>
        /// Duyệt qua danh sách file nén trong thư mục để phục hồi dữ liệu vào SgsiDataArchive
        /// </summary>
        /// <param name="backuppath"></param>
        public static void RunUnZipTrace(String backuppath)
        {
            if (!Directory.Exists(backuppath))
            {
                Console.WriteLine("INVALID PATH " + backuppath);
                return;
            }

            string Path = "Datacenter.Api.xml";

            ConfigManager _cfg = new ConfigManager();
            ResponseDataConfig Config = _cfg.Read<ResponseDataConfig>(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path));
            Config.Fix();

            //Đăng kí SgsiDataArchive
            var mapZip = Assembly.GetAssembly(typeof(Company))
                           .GetTypes()
                           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IZip)) != null && m.IsClass)
                           .Select(
                               m =>
                               {
                                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                   return makeme;
                               }).ToList();
            String zipdbname = Config.MotherSql.DataName;
            UnitOfWorkFactory.RegisterDatabase("SgsiDataArchive1", new NhibernateConfig
            {
                Maps = mapZip,
                Config =
                       DatabaseConfigFactory.GetDataConfig(false, Config.MotherSql.Ip, 0, zipdbname,
                           Config.MotherSql.User, Config.MotherSql.Pass, false, null)
            });


            foreach (var filepath in Directory.GetFiles(backuppath))
            {
                Console.WriteLine("process " + filepath);
                ReadZipDeviceTraceLog(filepath);
            }

            Console.WriteLine("DONE");
        }

        /// <summary>
        /// Giải nén từ một file nén để thêm lại vào SgsiDataArchive ( dùng để backup từ file nén )
        /// </summary>
        /// <param name="backupfile"></param>
        private static void ReadZipDeviceTraceLog(String backupfile)
        {
            if (!File.Exists(backupfile)) return;
            DateTime processDate = DateTime.ParseExact(Path.GetFileNameWithoutExtension(backupfile), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            processDate = processDate.AddMinutes(5);
            Console.WriteLine("PROCESS DATE " + processDate.ToString("yyyy-MM-dd HH:mm:ss"));

            FileStream fs = null;
            BinaryReader bw = null;
            try
            {
                fs = new FileStream(backupfile, FileMode.Open, FileAccess.Read, FileShare.Read, 102400);
                bw = new BinaryReader(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            int version = bw.ReadInt32();
            int n = bw.ReadInt32();

            //List<DeviceTraceZip> ret = new List<DeviceTraceZip>(n);
            for (int i = 0; i < n; i++)
            {
                if (!bw.ReadBoolean()) continue;

                DeviceTraceZip obj = new DeviceTraceZip();
                obj.Serial = bw.ReadInt64();
                obj.CompanyId = bw.ReadInt64();
                obj.GroupId = bw.ReadInt64();
                obj.TimeUpdate = processDate;
                obj.DbId = 0;
                obj.Indentity = Guid.Parse(bw.ReadString());

                int len = bw.ReadInt32();
                obj.Data = bw.ReadBytes(len);

                try
                {
                    using (var dataContextZip = UnitOfWorkFactory.GetUnitOfWork("SgsiDataArchive1", DbSupportType.MicrosoftSqlServer))
                    {
                        // lưu vào bảng mới
                        dataContextZip.Insert<DeviceTraceZip>(obj);
                        dataContextZip.Commit();
                        Console.WriteLine($"done for : {obj.Serial}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReadZipDeviceTraceLog", e, $"Lỗi nén data device: {obj.Serial}");
                }
            }

            if (bw != null) bw.Dispose();
            if (fs != null) fs.Dispose();

            Console.WriteLine("done date " + Path.GetFileNameWithoutExtension(backupfile));
        }


        #endregion Nén và giải nén DeviceTraceLog



        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static void TestCache()
        {
            List<DeviceTemp> deviceTemps = new List<DeviceTemp>();
            String cachepath = @"d:\SAGOSTAR\devicetemp.bin";
            try
            {
                using (FileStream fs = File.OpenRead(cachepath))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int version = stream.ReadInt32();
                    int n = stream.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        DeviceTemp obj = new DeviceTemp();
                        obj.Deserializer(stream, version);
                        deviceTemps.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {

            }

            List<DeviceTemp> deviceTemps1 = new List<DeviceTemp>();
            String cachepath1 = @"d:\SAGOSTAR\devicetemp1.bin";
            try
            {
                using (FileStream fs = File.OpenRead(cachepath1))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int version = stream.ReadInt32();
                    int n = stream.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        DeviceTemp obj = new DeviceTemp();
                        obj.Deserializer(stream, version);
                        deviceTemps1.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private static void TestCacheCompare(String path1, String path2)
        {
            Dictionary<long, DeviceTemp> deviceTemps = new Dictionary<long, DeviceTemp>();
            try
            {
                using (FileStream fs = File.OpenRead(path1))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int version = stream.ReadInt32();
                    int n = stream.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        DeviceTemp obj = new DeviceTemp();
                        obj.Deserializer(stream, version);
                        deviceTemps[obj.Serial] = obj;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Dictionary<long, DeviceTemp> deviceTemps1 = new Dictionary<long, DeviceTemp>();
            try
            {
                using (FileStream fs = File.OpenRead(path2))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int version = stream.ReadInt32();
                    int n = stream.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        DeviceTemp obj = new DeviceTemp();
                        obj.Deserializer(stream, version);
                        deviceTemps1[obj.Serial] = obj;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Compare
            Console.WriteLine(String.Format("COUNT {0} {1}", deviceTemps.Count, deviceTemps.Count == deviceTemps1.Count));
            foreach (var key in deviceTemps.Keys)
            {
                DeviceTemp d1 = deviceTemps[key];
                DeviceTemp d2 = deviceTemps1[key];

                //compare d1 vs d2
                ICollection<DeviceTraceLog> t1 = d1.LastTraces;
                ICollection<DeviceTraceLog> t2 = d2.LastTraces;

                if (t1.Count != t2.Count)
                    Console.WriteLine(String.Format("TRACECOUNT {2} : {0} {1}", t1.Count, t2.Count, d1.Serial));
            }

        }


        private static Dictionary<long, DeviceTemp> LoadCacheData(String cachepath)
        {
            Dictionary<long, DeviceTemp> deviceTemps = new Dictionary<long, DeviceTemp>();
            if (!File.Exists(cachepath)) return deviceTemps;
            try
            {
                using (FileStream fs = File.OpenRead(cachepath))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int version = stream.ReadInt32();
                    int n = stream.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        DeviceTemp obj = new DeviceTemp();
                        obj.Deserializer(stream, version);
                        deviceTemps[obj.Serial] = obj;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return deviceTemps;
        }

        public static void SaveCacheData(Dictionary<long, DeviceTemp> deviceTemps, String cachepath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(cachepath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachepath));

                if (File.Exists(cachepath)) File.Delete(cachepath);

                using (FileStream fs = File.Create(cachepath))
                using (BinaryWriter stream = new BinaryWriter(fs))
                {
                    stream.Write(1);
                    int n = deviceTemps.Keys.Count;
                    stream.Write(n);
                    foreach (var obj in deviceTemps.Values)
                    {
                        if (obj == null) continue;
                        obj.Serializer(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void TestLoadSaveCacheData()
        {
            Dictionary<long, DeviceTemp> xx = LoadCacheData(@"d:\SAGOSTAR\devicetemp.bin1");
            SaveCacheData(xx, @"d:\SAGOSTAR\devicetemp1.bin1");
            TestCacheCompare(@"d:\SAGOSTAR\devicetemp.bin1", @"d:\SAGOSTAR\devicetemp1.bin1");
        }

        private static void TestPacket()
        {
            String[] ss = new string[] {
                "010046007F6111030000000033446D3800000000000000000000000000000000005300000000000000E80200009600400A1400000000000000000000000000000000000000000000000071A44D01"
                ,"010046007F6111030000000047446D3800000000000000000000000000000000005300000000000000E80200009600400A140000000000000000000000000000000000000000000000009DD37425"
                ,"010046007F611103000000005B446D3800000000000000000000000000000000005300000000000000E80200009600400A14000000000000000000000000000000000000000000000000166E619A"
                ,"010046007F611103000000006F446D3800000000000000000000000000000000005300000000000000E80200009600400A1400000000000000000000000000000000000000000000000005884F3A"
                ,"010046007F6111030000000083446D3800000000000000000000000000000000005300000000000000E80200009600400A14000000000000000000000000000000000000000000000000DEECC1C7"
                ,"010046007F6111030000000097446D3800000000000000000000000000000000005300000000000000E80200009600400A1400000000000000000000000000000000000000000000000012415CC8"
                ,"010046007F61110300000000AB446D3800000000000000000000000000000000005300000000000000E80200009600400A1400000000000000000000000000000000000000000000000046B7FAD8"
                ,"010046007F61110300000000BF446D3800000000000000000000000000000000005300000000000000E80200009600400A140000000000000000000000000000000000000000000000008A1A67D7"
                ,"010046007F61110300000000D3446D3800000000000000000000000000000000005300000000000000E80200009600400A14000000000000000000000000000000000000000000000000EE5BB7F9"
                ,"010046007F61110300000000E7446D3800000000000000000000000000000000005300000000000000E80200009600400A14000000000000000000000000000000000000000000000000FDBD9959"
                ,"010046007F61110300000000FB446D3800000000000000000000000000000000005300000000000000E80200009600400A1400000000000000000000000000000000000000000000000076008CE6"
            };

            foreach (var s in ss)
            {
                IDevicePacket packet = null;
                try
                {
                    packet = DevicePacketFactory.CreatePacket(StringToByteArray(s));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }

            String hexstring = "134F9403000000001DAF105E0080DB170000000000FAAA01000000000011000000000000000000000000005D0D1E00000000000000000000000000000000000000000000000000000000000000009E00";
            byte[]  ret = StringToByteArray(hexstring);
            PBaseSyncPacket x1 = new PBaseSyncPacket(ret);
            bool bx = x1.Deserializer();
        }

        private static void TestSleep()
        {
            DateTime a = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0);

            Console.WriteLine((DateTime.Now - a).TotalDays);
            Console.WriteLine((DateTime.Now - a).TotalHours);
            Console.WriteLine((DateTime.Now - a).TotalMinutes);


        }

        private static void TestInsertBatch()
        {
            List<GeneralReportLog> xx = new List<GeneralReportLog>();
            for (int i = 0; i < 20000; i++)
            {
                GeneralReportLog obj = new GeneralReportLog()
                {
                    DbId = 0,
                    CompanyId = 1,
                    GuidId = Guid.NewGuid(),
                    UpdateTime = DateTime.Now,
                    KmOnDay = 100,
                    PauseCount = 1
                };

                xx.Add(obj);
            }

            var mapEntity = Assembly.GetAssembly(typeof(Company))
                             .GetTypes()
                             .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                             .Select(
                                 m =>
                                 {
                                     var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                     return makeme;
                                 }).ToList();

            UnitOfWorkFactory.RegisterDatabase("ABC", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, "127.0.0.1", 0, "SgsiData",
                            "sa", "123456789", false, null)
            });

            DateTime date = DateTime.Now;
            using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("ABC", DbSupportType.MicrosoftSqlServer))
            {
                dataContext.InsertAll(xx);
                dataContext.Commit();
            }
            Console.WriteLine(DateTime.Now.Subtract(date).TotalSeconds);
        }


        private static void TestBuildAllReport(DateTime begin, DateTime end)
        {
            for (DateTime beginday = begin.Date; beginday <= end.Date; beginday = beginday.AddDays(1))
            {
                var date = beginday;
                var en = beginday.AddDays(1).AddTicks(-1);

                //nếu ngày đầu hoặc ngày cuối thì set lại ngày theo ngày chọn
                if (date.Date == begin.Date) date = begin;
                if (en.Date == end.Date) en = end;

                //tổng số phút chạy được
                ////var run = (int)trace.Where(m => m.Type == TraceType.Machine && m.BeginTime >= date && m.BeginTime <= en).Sum(m => (m.EndTime - m.BeginTime).TotalMinutes);
                ////TimeStop = (int)Math.Round((end - date).TotalMinutes) - run

                Console.WriteLine(date.ToString("yyyy-MM-dd HH:mm:ss") + " --->" + en.ToString("yyyy-MM-dd HH:mm:ss") + "= " + (int)Math.Round((en - date).TotalMinutes));

            }

        }


        private static void TestFind()
        {
            List<String> ss = new List<string>() { "aaa", "bbb", "111", "ccc", "_12", "123" };


            Console.WriteLine(String.Join(",",
            ss.FindAll(m =>
            {
                int ret;
                return int.TryParse(m, out ret);
            })));


        }

        private static void TestProc()
        {
            var mapEntity = Assembly.GetAssembly(typeof(Company))
                               .GetTypes()
                               .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                               .Select(
                                   m =>
                                   {
                                       var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                       return makeme;
                                   }).ToList();

            UnitOfWorkFactory.RegisterDatabase("ABC", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, "127.0.0.1", 0, "SgsiData",
                            "sa", "123456", false, null)
            });

            var dataContext = UnitOfWorkFactory.GetUnitOfWork("ABC", DbSupportType.MicrosoftSqlServer);
            int ret = 0;
            dataContext.CustomHandle<ISession>(m =>
            {

                //using (SqlCommand cmd = new SqlCommand("spChangeDevice",(SqlConnection)m.Connection))
                //{
                //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //    cmd.Parameters.Add("@oldSerial", SqlDbType.BigInt).Value = 3;
                //    cmd.Parameters.Add("@newSerial", SqlDbType.BigInt).Value = 5;

                //    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                //    returnParameter.Direction = ParameterDirection.ReturnValue;

                //    var result = cmd.ExecuteScalar();
                //    ret = Convert.ToInt32(result);
                //}

                NHibernate.IQuery query = m.CreateSQLQuery("exec spChangeDevice @oldSerial=:oldSerial, @newSerial=:newSerial");
                query.SetInt64("oldSerial", 2);
                query.SetInt64("newSerial", 3);
                ret = query.UniqueResult<int>();
            });

            Console.WriteLine(ret);

            dataContext.Dispose();

            Console.WriteLine("Done process");
            Console.ReadLine();
        }


        private static void TestMongdb()
        {
            //MongoConfig config = new MongoConfig() {
            //    DbName = "mytest",
            //    Host = "127.0.0.1",
            //    Port = 27017
            //};

            //MongoFactory fac = new MongoFactory(config);

            //Reponsitory rep = fac.CreateContext("mytest");

            //DeviceLog obj = new DeviceLog()
            //{
            //    Indentity = Guid.NewGuid(),
            //    address = "aaaa",
            //    CompanyId = 1,
            //    GroupId=1,
            //     Name = "Name1",
            //     ClientSend = DateTime.Now.ToBinary()
            //};


            //rep.Insert<DeviceLog>(obj);


        }


        private static void testFuel()
        {
            String filepath = @"d:\SAGOSTAR\dinhvi\test\dbdata_fuel.txt";

            String[] ls = File.ReadLines(filepath).ToArray();

            FuelTest xx = new FuelTest();

            foreach (String s in ls)
            {
                if (String.IsNullOrWhiteSpace(s)) continue;
                String[] ss = s.Split('\t');

                DateTime time = DateTime.Parse(ss[0]);
                float fuelValue = float.Parse(ss[1]);

                List<FuelEventCandidate> ret = xx.CheckAndGetEvents(0, 0, fuelValue, time, 50000, DeviceType.None);
                if (ret != null)
                {
                    if (ret.Count > 1) ret.Sort();
                    foreach (var item in ret)
                    {
                        Console.WriteLine(String.Format("Thay doi={0} counter={1} tai={2} first={3}", item.SumDelta, item.Counter, item.PrevTime, item.FistTime));
                    }
                }
            }
            Console.WriteLine("Done");
        }

        private static void testFuelZip()
        {
            string txt = File.ReadAllText(@"d:\SAGOSTAR\dinhvi\test\zipbin.txt ");
            List<TimeFuel> xxx = JsonConvert.DeserializeObject<List<TimeFuel>>(txt);

            String textoutput = String.Join("\n", xxx.OrderBy(m => m.Time).Select(m => m.Time + "\t" + m.Fuel).ToArray());
            File.WriteAllText(@"d:\SAGOSTAR\dinhvi\test\zipbinout.txt ", textoutput);


            FuelTest xx = new FuelTest();
            foreach (var s in xxx.OrderBy(m => m.Time).ToList())
            {
                List<FuelEventCandidate> ret = xx.CheckAndGetEvents(0, 0, s.Fuel, s.Time, 1000000, DeviceType.None);
                if (ret != null)
                {
                    if (ret.Count > 1) ret.Sort();
                    foreach (var item in ret)
                    {
                        //Tính lại giá trị thay đổi dựa vào gá trị dầu ở sự kiện trước và giá trị tại sự kiện này
                        Console.WriteLine(String.Format("Thay doi={0} counter={1} tai={2} first={3} luc {4}", item.SumDelta, item.Counter, item.PrevTime, item.FistTime, item.MaxDeltaTime));
                    }
                }
            }

            Console.WriteLine("Done");
        }

        private static void testFuelZipAll()
        {
            String[] ls = Directory.GetFiles(@"d:\SAGOSTAR\dinhvi\test\fueltest");

            foreach (String fp in ls)
            {
                Console.WriteLine(Path.GetFileName(fp));

                String[] sss = Path.GetFileNameWithoutExtension(fp).Split('-');
                int flen = int.Parse(sss[sss.Length - 1]);


                string txt = File.ReadAllText(fp);
                List<TimeFuel> xxx = JsonConvert.DeserializeObject<List<TimeFuel>>(txt);

                FuelTest xx = new FuelTest();
                foreach (var s in xxx.OrderBy(m => m.Time).ToList())
                {
                    List<FuelEventCandidate> ret = xx.CheckAndGetEvents(0, 0, s.Fuel, s.Time, flen, DeviceType.OilVehicle);

                    if (ret != null)
                    {
                        if (ret.Count > 1) ret.Sort();
                        foreach (var item in ret)
                        {
                            //Tính lại giá trị thay đổi dựa vào gá trị dầu ở sự kiện trước và giá trị tại sự kiện này
                            Console.WriteLine(String.Format("Thay doi={0} counter={1} tai={2} first={3} luc {4} tre {5}", item.SumDelta, item.Counter, item.PrevTime, item.FistTime, item.MaxDeltaTime, (s.Time - item.PrevTime).TotalMinutes));
                        }
                    }
                }

            }


            Console.WriteLine("Done");
        }


        private static void LoadTrip()
        {
            String text = File.ReadAllText(@"d:\SAGOSTAR\dinhvi\test\Fuel_17060105_2017_07_28.txt");
            DeviceTripGet trip = JsonConvert.DeserializeObject<DeviceTripGet>(text);

            StringBuilder sb = new StringBuilder();
            if (trip != null && trip.Status > 0)
            {
                foreach (var line in trip.Datas)
                {
                    sb.Append(line.TimeUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append('\t');
                    sb.Append(line.Fuel);
                    sb.Append('\n');
                }
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            File.WriteAllText(@"d:\SAGOSTAR\dinhvi\test\Fuel_2.txt", sb.ToString());
        }


        public static void TestInsertDeviceLog(int notest)
        {
            Console.WriteLine($"Initdb");
            var mapEntity = Assembly.GetAssembly(typeof(Company))
                            .GetTypes()
                            .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
                            .Select(
                                m =>
                                {
                                    var makeme = typeof(FactoryMap<>).MakeGenericType(m);
                                    return makeme;
                                }).ToList();

            UnitOfWorkFactory.RegisterDatabase("SgsiData", new NhibernateConfig
            {
                Maps = mapEntity,
                Config =
                        DatabaseConfigFactory.GetDataConfig(false, "127.0.0.1", 0, "SgsiData",
                            "sa", "123456", false, null)
            });

            while (true)
            {
                Console.WriteLine("Nhap so dong can insert : ");
                String line = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(line)) break;

                notest = int.Parse(line);

                List<DeviceLog> xx = new List<DeviceLog>();

                Console.WriteLine($"Generate {notest} records");

                for (int i = 0; i < notest; i++)
                {
                    DeviceLog obj = new DeviceLog()
                    {
                        CompanyId = 1,
                        DbId = 0,
                        Indentity = Guid.Parse("A2B00A48-22A7-4387-90F8-0D45FB765BD5"),
                        Serial = 15151515,
                        GroupId = 146,

                        DriverStatus = new Datacenter.Model.Components.DriverStatusInfo
                        {
                            DriverId = 899,
                            Gplx = "",
                            TimeBeginWorkInSession = DateTime.Parse("2018-06-06 09:07:07.000")
                        },
                        DeviceStatus = new Datacenter.Model.Components.DeviceStatusInfo
                        {
                            ClientSend = DateTime.Now.AddDays(-1).AddMilliseconds(i),
                            ServerRecv = DateTime.Now.AddDays(-1).AddMilliseconds(i),
                            GpsStatus = true,
                            TotalGpsDistance = 118126 + i,
                            Temperature = 150,
                            GsmSignal = 88,
                            Power = 25,
                            Machine = true,
                            AirMachine = true,
                            UseFuel = true,
                            Door = true,
                            SpeedTrace = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,",
                            GpsInfo = new Datacenter.Model.Components.GpsLocation() { Lat = 10.78766f, Lng = 106.6242f, Address = "" }

                        }

                    };

                    xx.Add(obj);
                }

                DateTime date = DateTime.Now;
                Console.WriteLine($"Insert {notest} records...");

                using (var dataContext = UnitOfWorkFactory.GetUnitOfWork("SgsiData", DbSupportType.MicrosoftSqlServer))
                {
                    //dataContext.InsertAll(xx);
                    //dataContext.Commit();

                    var connectionString = $@"Server={"127.0.0.1"};Database={"SgsiData"};User Id={"sa"};Password={"123456"};";
                    using (var sqlBulk = new SqlBulkCopy(connectionString))
                    {
                        //sqlBulk.BatchSize = 5000;
                        sqlBulk.NotifyAfter = 1000;
                        sqlBulk.SqlRowsCopied += SqlBulk_SqlRowsCopied;
                        sqlBulk.DestinationTableName = "DeviceLogMoving";
                        sqlBulk.WriteToServer(DeviceLog.CreateTable(xx));
                    }


                }
                Console.WriteLine($"Insert {notest} records done {DateTime.Now.Subtract(date).TotalSeconds} seconds");


            }

        }

        private static void SqlBulk_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs eventArgs)
        {
            Console.WriteLine("Wrote " + eventArgs.RowsCopied + " records.");
        }

        class TimeFuel
        {
            public DateTime Time;
            public int Fuel;
        }

        private static void filterByText(string path,string text)
        {
            if (!File.Exists(path)) return;

            try
            {
                List<String> ret = new List<string>();
                String[] sss = File.ReadAllLines(path);
                for (int i = 0; i < sss.Length; i++)
                {
                    if (sss[i].Contains(text))
                        ret.Add(sss[i]);
                }

                File.WriteAllLines(Path.ChangeExtension(path,"out"), ret.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void filterByText(string path)
        {
            if (!File.Exists(path)) return;

            try
            {
                Dictionary<string, int> ret = new Dictionary<string, int>();
                String[] sss = File.ReadAllLines(path);
                for (int i = 0; i < sss.Length; i++)
                {
                    int idx1 = sss[i].IndexOf("khách");
                    int idx2 = sss[i].IndexOf(" ",idx1+1);
                    int idx3 = sss[i].IndexOf(" ", idx2+1);
                    String aa =sss[i].Substring(idx2 + 1, idx3 - idx2);

                    long abc;
                    if (long.TryParse(aa, out abc))
                    {
                        if (ret.ContainsKey(aa))
                            ret[aa]++;
                        else
                            ret.Add(aa, 1);
                    }
                }

                String sret = "";
                var sortedDict = from entry in ret orderby entry.Value descending select entry;
                foreach(KeyValuePair<string,int> kk in sortedDict)
                {
                    sret+= kk.Key + "=" + kk.Value + "\n";
                }

                File.WriteAllText(Path.ChangeExtension(path, "out1"), sret);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }
}
