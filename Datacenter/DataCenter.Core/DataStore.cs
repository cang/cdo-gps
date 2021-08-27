#region header

// /*********************************************************************************************/
// Project :DataCenter.Core
// FileName : DataStore.cs
// Time Create : 2:30 PM 07/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Datacenter.Model;
using Datacenter.Model.Entity;
using Datacenter.QueryRoute;
using Log;
using System.IO;
using Datacenter.Model.Utils;
using System.Threading;

namespace DataCenter.Core
{
    [Export(typeof(IDataStore))]
    [Export(typeof(IDataCenterStoreEvent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DataStore : IDataCenterStoreEvent, IDataStore
    {
        private const String DEVICETEMPS_FILENAME = "devicetemp.bin";
        private const String DEVICE_STATUS_FILENAME = "devicestatus.bin";
        private const String RAWLOFSERIALS_FILENAME = "RawLogSerials.txt";
        private const String OLDSERIALS_FILENAME = "OldSerials.txt";
        private const String REPLACESERIALS_FILENAME = "ReplaceSerials.txt";

        public static ManualResetEvent LoadReadyEvent = new ManualResetEvent(false);

        //0 : init
        //1 : serialize for Temp.GeneralReportLog
        //2 : serialize for Temp.BgtvLastTransfer
        //3 : Area,Point
        //4 : Fuel Param
        //5 : add Temp.GeneralReportLog Guest
        //6 : add Temp.HasGuestSensor
        //7 : add Temp.GeneralGuestLog and remove guest in GeneralReportLog
        //8 : add remove Temp.FuelLastTime,FuelLastValue,FuelHasIssue
        //9 : add Temp.FuelTest
        //10 : add Temp.TimePause
        //11 : add Temp.FuelTest.LastSyncDevice,Temp.FuelTest.TimeSecondWorkInDay
        //12 : remove Temp.FuelTest.LastSyncDevice,Temp.FuelTest.TimeSecondWorkInDay
        //13 : add Temp.FuelTest..CounterSeconds
        //14 : add Temp.SentOnlineSms
        //15 : add Temp.FuelTest..MaxDelta,MaxDeltaTime,MaxDeltaLat,MaxDeltaLng
        //16 : remove Temp.TimeHandlePause,TimeHandleRun
        //17 : add MachineSeconds
        //18 : remove HasGuestSensor
        //19 : add Temp.FuelTest.PrevPoint and Temp.FuelTest.CandidatePoints 
        //20 : add Temp.Last10hTrace, Temp.MidnightLocation
        //21 : add Temp.DeviceStatusUpdateTime
        //22 : add Temp.DeviceLostGsmLog
        //23 : add Temp.LastTotalKmUsingOnDay
        private const int DEVICETEMPS_VERSION = 23;


        [Import] private ReponsitoryFactory _factory;

        [Import] private ILog _log;

        //https://msdn.microsoft.com/en-us/library/dd287191(v=vs.110).aspx
        //https://stackoverflow.com/questions/10479867/is-concurrentdictionary-keys-or-values-property-threadsafe
        //https://stackoverflow.com/questions/3216636/is-a-linq-query-to-concurrentdictionary-values-threadsafe


        private readonly ConcurrentDictionary<long, Device> _allDevices = new ConcurrentDictionary<long, Device>();
        private readonly ConcurrentDictionary<string, DeviceModel> _allDeviceModels = new ConcurrentDictionary<string, DeviceModel>();
        private readonly ConcurrentDictionary<long, Driver> _allDrivers = new ConcurrentDictionary<long, Driver>();
        private readonly ConcurrentDictionary<int, PointGps> _allGpsPoints = new ConcurrentDictionary<int, PointGps>();
        private readonly ConcurrentDictionary<int, Area> _allAreas = new ConcurrentDictionary<int, Area>();
        private readonly ConcurrentDictionary<long, DeviceGroup> _allDeviceGroups =
            new ConcurrentDictionary<long, DeviceGroup>();
        private readonly ConcurrentDictionary<long, Company> _allCompany = new ConcurrentDictionary<long, Company>();

        private readonly ConcurrentBag<RouteGpsLogic> _allRouteGps = new ConcurrentBag<RouteGpsLogic>();
        private readonly ConcurrentBag<long> _allRawLogSerials = new ConcurrentBag<long>();

        private readonly ConcurrentDictionary<string, FuelSheet> _allFuelSheets = new ConcurrentDictionary<string, FuelSheet>();

        private readonly ConcurrentBag<long> _allOldSerials = new ConcurrentBag<long>();
        private volatile bool _OldSerialsChanged;

        //private readonly ConcurrentBag<long> _allReplaceSerials = new ConcurrentBag<long>();
        private readonly ConcurrentDictionary<long, bool> _allReplaceSerials = new ConcurrentDictionary<long, bool>();
        private volatile bool _ReplaceSerialsChanged;

        private int _idSql;
        private IQueryRoute _context;

        private String datapath;
        private String configpath;


        public event Action<Company> OnAddCompany;

        public event Action<Company> OnRemoveCompany;

        public event Action<Device> OnAddDevice;

        public event Action<Device> OnRemoveDevice;

        //void IDataCenterStoreEvent.ReSync()
        //{
        //    var tmp = _allCompany.Values.ToList();

        //    foreach (var c in tmp)
        //    {
        //        OnAddCompany?.Invoke(c);
        //        foreach (var device in GetQueryContext<Device>().GetByCompany(c.Id))
        //        {
        //            OnAddDevice?.Invoke(device);
        //        }
        //    }
        //}

        IList<Device> IQueryModel<Device>.GetAll()
        {
            //var result=new List<Device>();
            //var tmp = _allDevices.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allDevices.Values.ToList();
        }

        IList<Driver> IQueryModel<Driver>.GetByCompany(long companyId)
        {
            return GetQueryContext<Driver>().GetWhere(m => m.CompanyId == companyId);
        }

        IList<Driver> IQueryModel<Driver>.GetByGroup(long companyId, long groupId)
        {
            return GetQueryContext<Driver>().GetWhere(m => m.GroupId == groupId && m.CompanyId == companyId);
        }

        Driver IQueryModel<Driver>.GetByKey(object key)
        {
            Driver tmp;
            _allDrivers.TryGetValue((long)key, out tmp);
            return tmp;
        }

        IList<Driver> IQueryModel<Driver>.GetWhere(Func<Driver, bool> @where)
        {
            //return GetQueryContext<Driver>().GetAll().Where(where).ToList();
            return _allDrivers.Values.Where(where).ToList();
        }

        bool IQueryModel<Driver>.Add(Driver obj, long companyId)
        {
            if (GetQueryContext<Driver>().GetByKey(obj.Id) != null) return false;
            return _allDrivers.TryAdd(obj.Id, obj);
        }

        bool IQueryModel<Driver>.Del(Driver obj, long companyId)
        {
            Driver tmp;
            return _allDrivers.TryRemove(obj.Id, out tmp);
        }

        IList<Driver> IQueryModel<Driver>.GetAll()
        {
            //var result = new List<Driver>();
            //var tmp = _allDrivers.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allDrivers.Values.ToList();
        }

        IList<Area> IQueryModel<Area>.GetByCompany(long companyId)
        {
            return GetQueryContext<Area>().GetWhere(m => m.CompanyId == companyId);
        }

        IList<Area> IQueryModel<Area>.GetByGroup(long companyId, long groupId)
        {
            if (groupId > 0)
                return GetQueryContext<Area>().GetWhere(m => m.CompanyId == companyId && m.GroupId == groupId);
            else
                return GetQueryContext<Area>().GetByCompany(companyId);
        }

        Area IQueryModel<Area>.GetByKey(object key)
        {
            Area tmp;
            _allAreas.TryGetValue((int)key, out tmp);
            return tmp;
        }

        IList<Area> IQueryModel<Area>.GetWhere(Func<Area, bool> @where)
        {
            //return GetQueryContext<Area>().GetAll().Where(where).ToList();
            return _allAreas.Values.Where(where).ToList();
        }

        bool IQueryModel<Area>.Add(Area obj, long companyId)
        {
            if (GetQueryContext<Area>().GetByKey(obj.Id) != null) return false;
            return _allAreas.TryAdd(obj.Id, obj);
        }

        bool IQueryModel<Area>.Del(Area obj, long companyId)
        {
            Area tmp;
            return _allAreas.TryRemove(obj.Id, out tmp);
        }

        IList<Area> IQueryModel<Area>.GetAll()
        {
            //var result = new List<Area>();
            //var tmp = _allAreas.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allAreas.Values.ToList();
        }

        IList<PointGps> IQueryModel<PointGps>.GetByCompany(long companyId)
        {
            return GetQueryContext<PointGps>().GetWhere(m => m.CompanyId == companyId);
        }

        IList<PointGps> IQueryModel<PointGps>.GetByGroup(long companyId, long groupId)
        {
            if (groupId > 0)
                return GetQueryContext<PointGps>().GetWhere(m => m.CompanyId == companyId && m.GroupId == groupId);
            else
                return GetQueryContext<PointGps>().GetByCompany(companyId);
        }

        PointGps IQueryModel<PointGps>.GetByKey(object key)
        {
            PointGps tmp;
            _allGpsPoints.TryGetValue((int)key, out tmp);
            return tmp;
        }

        IList<PointGps> IQueryModel<PointGps>.GetWhere(Func<PointGps, bool> @where)
        {
            //return GetQueryContext<PointGps>().GetAll().Where(where).ToList();
            return _allGpsPoints.Values.Where(where).ToList();
        }

        bool IQueryModel<PointGps>.Add(PointGps obj, long companyId)
        {
            if (GetQueryContext<PointGps>().GetByKey(obj.Id) != null) return false;
            return _allGpsPoints.TryAdd(obj.Id, obj);
        }

        bool IQueryModel<PointGps>.Del(PointGps obj, long companyId)
        {
            PointGps tmp;
            return _allGpsPoints.TryRemove(obj.Id, out tmp);
        }

        IList<PointGps> IQueryModel<PointGps>.GetAll()
        {
            //var result = new List<PointGps>();
            //var tmp = _allGpsPoints.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allGpsPoints.Values.ToList();
        }

        IList<DeviceModel> IQueryModel<DeviceModel>.GetByCompany(long companyId)
        {
            throw new NotImplementedException();
        }

        IList<DeviceModel> IQueryModel<DeviceModel>.GetByGroup(long companyId, long groupId)
        {
            throw new NotImplementedException();
        }

        DeviceModel IQueryModel<DeviceModel>.GetByKey(object key)
        {
            DeviceModel tmp;
            _allDeviceModels.TryGetValue((string)key, out tmp);
            return tmp;
        }

        IList<DeviceModel> IQueryModel<DeviceModel>.GetWhere(Func<DeviceModel, bool> @where)
        {
            //return GetQueryContext<DeviceModel>().GetAll().Where(where).ToList();
            return _allDeviceModels.Values.Where(where).ToList();
        }

        bool IQueryModel<DeviceModel>.Add(DeviceModel obj, long companyId)
        {
            if (GetQueryContext<DeviceModel>().GetByKey(obj.Name) != null) return false;
            return _allDeviceModels.TryAdd(obj.Name, obj);
        }

        bool IQueryModel<DeviceModel>.Del(DeviceModel obj, long companyId)
        {
            DeviceModel tmp;
            return _allDeviceModels.TryRemove(obj.Name, out tmp);
        }

        IList<DeviceModel> IQueryModel<DeviceModel>.GetAll()
        {
            //var result = new List<DeviceModel>();
            //var tmp = _allDeviceModels.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allDeviceModels.Values.ToList();
        }

        public IList<FuelSheet> GetByCompany(long companyId)
        {
            throw new NotImplementedException();
        }

        public IList<FuelSheet> GetByGroup(long companyId, long groupId)
        {
            throw new NotImplementedException();
        }

        FuelSheet IQueryModel<FuelSheet>.GetByKey(object key)
        {
            FuelSheet tmp;
            _allFuelSheets.TryGetValue((string)key, out tmp);
            return tmp;
        }

        IList<FuelSheet> IQueryModel<FuelSheet>.GetWhere(Func<FuelSheet, bool> @where)
        {
            //return GetQueryContext<FuelSheet>().GetAll().Where(where).ToList();
            return _allFuelSheets.Values.Where(where).ToList();
        }

        bool IQueryModel<FuelSheet>.Add(FuelSheet obj, long companyId)
        {
            if (GetQueryContext<FuelSheet>().GetByKey(obj.Name) != null) return false;
            return _allFuelSheets.TryAdd(obj.Name, obj);
        }

        bool IQueryModel<FuelSheet>.Del(FuelSheet obj, long companyId)
        {
            FuelSheet tmp;
            return _allFuelSheets.TryRemove(obj.Name, out tmp);
        }

        IList<FuelSheet> IQueryModel<FuelSheet>.GetAll()
        {
            //var result = new List<FuelSheet>();
            //var tmp = _allFuelSheets.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allFuelSheets.Values.ToList();
        }


        IList<DeviceGroup> IQueryModel<DeviceGroup>.GetByCompany(long companyId)
        {
            return GetQueryContext<DeviceGroup>().GetWhere(m => m.CompnayId == companyId);
        }

        IList<DeviceGroup> IQueryModel<DeviceGroup>.GetByGroup(long companyId, long groupId)
        {
            throw new NotImplementedException();
        }

        DeviceGroup IQueryModel<DeviceGroup>.GetByKey(object key)
        {
            DeviceGroup tmp;
            _allDeviceGroups.TryGetValue((long)key, out tmp);
            return tmp;
        }

        IList<DeviceGroup> IQueryModel<DeviceGroup>.GetWhere(Func<DeviceGroup, bool> @where)
        {
            //return GetQueryContext<DeviceGroup>().GetAll().Where(where).ToList();
            return _allDeviceGroups.Values.Where(where).ToList();
        }

        bool IQueryModel<DeviceGroup>.Add(DeviceGroup obj, long companyId)
        {
            if (GetQueryContext<DeviceGroup>().GetByKey(obj.Id) != null) return false;
            return _allDeviceGroups.TryAdd(obj.Id, obj);
        }

        bool IQueryModel<DeviceGroup>.Del(DeviceGroup obj, long companyId)
        {
            DeviceGroup tmp;
            return _allDeviceGroups.TryRemove(obj.Id, out tmp);
        }

        IList<DeviceGroup> IQueryModel<DeviceGroup>.GetAll()
        {
            //var result = new List<DeviceGroup>();
            //var tmp = _allDeviceGroups.GetEnumerator();
            //while (tmp.MoveNext())
            //{
            //    result.Add(tmp.Current.Value);
            //}
            //return result;
            return _allDeviceGroups.Values.ToList();
        }

        IList<Device> IQueryModel<Device>.GetByCompany(long companyId)
        {
            return GetQueryContext<Device>().GetWhere(m => m.CompanyId == companyId);
        }

        IList<Device> IQueryModel<Device>.GetByGroup(long companyId, long groupId)
        {
            return GetQueryContext<Device>().GetWhere(m => m.CompanyId == companyId && m.GroupId == groupId);
        }

        Device IQueryModel<Device>.GetByKey(object key)
        {
            Device tmp;
            _allDevices.TryGetValue((long)key, out tmp);
            return tmp;
        }

        IList<Device> IQueryModel<Device>.GetWhere(Func<Device, bool> @where)
        {
            //return GetQueryContext<Device>().GetAll().Where(where).ToList();
            return _allDevices.Values.Where(where).ToList();
        }

        bool IQueryModel<Device>.Add(Device obj, long companyId)
        {
            if (GetQueryContext<Device>().GetByKey(obj.Serial) != null) return false;

            if (_allDevices.TryAdd(obj.Serial, obj))
            {
                OnAddDevice?.Invoke(obj);
                return true;
            }
            return false;
        }

        bool IQueryModel<Device>.Del(Device obj, long companyId)
        {
            Device tmp;
            if (_allDevices.TryRemove(obj.Serial, out tmp))
            {
                OnRemoveDevice?.Invoke(tmp);
                return true;
            }
            return false;
        }


        void IDataStore.Reload(int idSql, String datapath, String configpath)
        {
            try
            {
                _log.Info("DataStore", $"Reload");

                this.datapath = datapath;
                this.configpath = configpath;
                _idSql = idSql;
                var stopWatch = new Stopwatch();
                stopWatch.Start();


                if (_context == null)
                    _context = _factory.CreateQuery();

                //load route 
                var allRouteGps = _context.GetAll<long, RouteGps>(m => m.Id, idSql);
                foreach (var item in allRouteGps.Values)
                    _allRouteGps.Add(new RouteGpsLogic(item));

                //Load Raw LogSerials 
                LoadRawLogSerials();
                LoadOldSerials();
                LoadReplaceSerials();

                //remain
                var allCompany = _context.GetAll<long, Company>(m => m.Id, idSql);
                var allDevice = _context.GetAll<long, Device>(m => m.Serial, idSql);
                foreach (var company in allCompany)
                {
                    if (company.Value.Setting == null)
                    {
                        company.Value.Setting = new CompanySetting { Company = company.Value, TimeoutHidenDevice = 60 * 24 * 7, TimeoutLostDevice = 120 };
                        _context.Insert(company.Value.Setting, _idSql);
                    }
                }
                _context.Commit(_idSql);

                foreach (var device in allDevice)
                {
                    if (device.Value.Status == null)
                    {
                        _log.Warning("DATABASE", $"Device {device.Key}  null Status");
                    }
                    if (device.Value.Status != null && device.Value.Status.BasicStatus == null)
                        _log.Warning("DATABASE", $"Device {device.Key}  null Status.BasicStatus");
                }

                var allGroup = _context.GetAll<long, DeviceGroup>(m => m.Id, idSql);
                var allGpsCheckPoint = _context.GetAll<long, PointGps>(m => m.Id, idSql);
                var allDriver = _context.GetAll<long, Driver>(m => m.Id, idSql);
                var allCheckZone = _context.GetAll<long, Area>(m => m.Id, idSql);
                var allModel = _context.GetAll<string, DeviceModel>(m => m.Name, idSql);
                var allFuelSheet = _context.GetAll<string, FuelSheet>(m => m.Name, idSql);

                foreach (var deviceModel in allModel)
                {
                    GetQueryContext<DeviceModel>().Add(deviceModel.Value, 0);
                }

                foreach (var fuelSheet in allFuelSheet)
                {
                    GetQueryContext<FuelSheet>().Add(fuelSheet.Value, 0);
                }

                //load temp cache
                Dictionary<long, DeviceTemp> deviceTemps = LoadCacheData();

                //add device to company and update device.Temp, ModelName
                foreach (var company in allCompany)
                {
                    AddCompany(company.Value);
                    var tmp = allDevice.Values.Where(m => m.CompanyId == company.Key);

                    foreach (var device in tmp)
                    {
                        //update deviceTemp 
                        if (deviceTemps.ContainsKey(device.Serial))
                        {
                            device.Temp = deviceTemps[device.Serial];
                            foreach (var item in device.Temp.LastTraces)
                            {
                                item.CompanyId = device.CompanyId;
                                item.GroupId = device.GroupId;
                                item.Serial = device.Serial;
                                item.Indentity = device.Indentity;
                                item.DbId = company.Value.DbId;
                                item.DriverId = device.Status?.DriverStatus?.DriverId ?? 0;
                            }

                            //update more information for GeneralReportLog from device
                            if (device.Temp.GeneralReportLog != null)
                            {
                                lock (device.Temp.GeneralReportLogLock)
                                {
                                    device.Temp.GeneralReportLog.Id = 0;
                                    device.Temp.GeneralReportLog.DbId = company.Value.DbId;
                                    device.Temp.GeneralReportLog.CompanyId = device.CompanyId;
                                    device.Temp.GeneralReportLog.GuidId = device.Indentity;
                                    device.Temp.GeneralReportLog.GroupId = device.GroupId;
                                }
                            }

                            //update more information for GeneralGuestLog from device
                            if (device.Temp.GeneralGuestLog != null)
                            {
                                lock (device.Temp.GeneralGuestLogLock)
                                {
                                    device.Temp.GeneralGuestLog.Id = 0;
                                    device.Temp.GeneralGuestLog.DbId = company.Value.DbId;
                                    device.Temp.GeneralGuestLog.CompanyId = device.CompanyId;
                                    device.Temp.GeneralGuestLog.GuidId = device.Indentity;
                                    device.Temp.GeneralGuestLog.GroupId = device.GroupId;
                                }
                            }

                            //update more information for DeviceLostGsmLog from device
                            if (device.Temp.DeviceLostGsmLog != null)
                            {
                                device.Temp.DeviceLostGsmLog.Serial = device.Serial;
                                device.Temp.DeviceLostGsmLog.DbId = company.Value.DbId;
                                device.Temp.DeviceLostGsmLog.CompanyId = device.CompanyId;
                                device.Temp.DeviceLostGsmLog.Indentity = device.Indentity;
                                device.Temp.DeviceLostGsmLog.GroupId = device.GroupId;
                            }

                            //update status
                            if (device.Status != null)
                            {
                                //if(device.Temp.LastTotalKmUsingOnDay>0 )
                                if(device.Temp.LastTotalKmUsingOnDay> device.Status.LastTotalKmUsingOnDay)
                                    device.Status.LastTotalKmUsingOnDay = device.Temp.LastTotalKmUsingOnDay;//gia tri nay duoc cap nhat chi 1 lan dua nhat vao luc 0 gio

                                device.Status.PauseCount = (short)device.Temp.PauseCount;
                                if (device.Status.DriverStatus != null)
                                    device.Status.DriverStatus.OverSpeedCount = device.Temp.OverSpeedCount;
                            }

                        }

                        //update ModelName
                        if (!string.IsNullOrWhiteSpace(device.ModelName) && allModel.ContainsKey(device.ModelName))
                        {
                            device.Temp.Model = allModel[device.ModelName];
                        }

                        //add to company
                        GetQueryContext<Device>().Add(device, company.Key);
                    }


                    foreach (
                        var gr in allGroup.Where(m => m.Value.CompnayId == company.Key))
                        GetQueryContext<DeviceGroup>().Add(gr.Value, company.Key);

                    foreach (
                        var gr in
                            allGpsCheckPoint.Where(
                                m => m.Value.CompanyId == company.Key))
                        GetQueryContext<PointGps>().Add(gr.Value, company.Key);

                    foreach (
                        var gr in
                            allCheckZone.Where(
                                m => m.Value.CompanyId == company.Key))
                        GetQueryContext<Area>().Add(gr.Value, company.Key);
                    foreach (
                        var gr in
                            allDriver.Where(m => m.Value.CompanyId == company.Key))
                        GetQueryContext<Driver>().Add(gr.Value, company.Key);

                }

                deviceTemps.Clear();
                stopWatch.Stop();

                _log.Info("DataStore", $"Done Reload {stopWatch.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                _log.Exception("DataStore", e, "Reload");
            }

            LoadReadyEvent.Set();
        }

        public bool AddCompany(Company c)
        {
            if (_allCompany.ContainsKey(c.Id)) return false;
            if (_allCompany.TryAdd(c.Id, c))
            {
                OnAddCompany?.Invoke(c);
                return true;
            }
            return false;
        }

        public bool RemoveCompany(long id)
        {
            Company tmp;
            if (_allCompany.TryRemove(id, out tmp))
            {
                OnRemoveCompany?.Invoke(tmp);
                return true;
            }
            return false;
        }

        public Company GetCompanyById(long id)
        {
            Company tmp;
            _allCompany.TryGetValue(id, out tmp);
            return tmp;
        }

        public IList<Company> GetAllCompany()
        {
            var result = new List<Company>();
            var tmp = _allCompany.GetEnumerator();
            while (tmp.MoveNext())
            {
                result.Add(tmp.Current.Value);
            }
            return result;
        }

        public IQueryModel<T> GetQueryContext<T>() where T : ICacheModel
        {
            var model = this as IQueryModel<T>;
            if (model != null)
                return model;
            throw new NullReferenceException();
        }

        public RouteGpsLogic GetRoute(float lat, float lon)
        {
            return _allRouteGps.FirstOrDefault(m => m.Contains(lat, lon));
        }

        /// <summary>
        /// Đọc dữ liệu cache devicetemp từ file
        /// </summary>
        /// <returns></returns>
        private Dictionary<long, DeviceTemp> LoadCacheData()
        {
            Dictionary<long, DeviceTemp> deviceTemps = new Dictionary<long, DeviceTemp>();
            if (datapath == null) return deviceTemps;
            String cachepath = Path.Combine(datapath, DEVICETEMPS_FILENAME);
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
                _log.Exception("CACHE", e, $"LoadCacheData {cachepath}");
            }
            return deviceTemps;
        }

        /// <summary>
        /// Lưu lại bộ nhớ cache của devicetemp xuống file
        /// </summary>
        public void SaveCacheData()
        {
            if (datapath == null) return;
            String cachepath = Path.Combine(datapath, DEVICETEMPS_FILENAME);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(cachepath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachepath));

                if (File.Exists(cachepath)) File.Delete(cachepath);

                using (FileStream fs = File.Create(cachepath))
                using (BinaryWriter stream = new BinaryWriter(fs))
                {
                    stream.Write(DEVICETEMPS_VERSION);
                    int n = _allDevices.Values.Count;
                    stream.Write(n);
                    foreach (var obj in _allDevices.Values)
                    {
                        obj.Temp.Serial = obj.Serial;
                        obj.Temp.Serializer(stream);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"SaveCacheData {cachepath}");
            }

            SaveOldSerials();
            SaveReplaceSerials();
        }

        /// <summary>
        /// Đọc danh sách serial cần log raw từ file cấu hình
        /// </summary>
        public void LoadRawLogSerials()
        {
            if (configpath == null) return;

            long item; while (!_allRawLogSerials.IsEmpty)
                _allRawLogSerials.TryTake(out item);

            String rawlogpath = Path.Combine(configpath, RAWLOFSERIALS_FILENAME);
            try
            {
                if (!File.Exists(rawlogpath)) return;
                foreach (String serial in File.ReadAllLines(rawlogpath))
                {
                    if (String.IsNullOrWhiteSpace(serial)) continue;
                    _allRawLogSerials.Add(long.Parse(serial));
                }
                _log.Info("CACHE", $"_allRawLogSerials.Count {_allRawLogSerials.Count}");
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"LoadRawLogSerials {rawlogpath}");
            }
        }

        /// <summary>
        /// Kiểm tra để lưu raw cho serial 
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public bool ContainRawLogSerial(long serial)
        {
            if (_allRawLogSerials.Count == 0) return true;
            return _allRawLogSerials.Contains(serial);
        }

        public void TrackRawLogSerial(long serial)
        {
            if (!_allRawLogSerials.Contains(serial))
                _allRawLogSerials.Add(serial);
        }

        public List<long> GetAllCompanyId()
        {
            return _allCompany.Values.Select(m => m.Id).ToList();
        }

        public List<long> GetAllDeviceSerial()
        {
            return _allDevices.Values.Select(m => m.Serial).ToList();
        }


        public void SaveDeviceStatusCache()
        {
            if (datapath == null) return;
            String cachepath = Path.Combine(datapath, DEVICE_STATUS_FILENAME);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(cachepath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachepath));

                if (File.Exists(cachepath)) File.Delete(cachepath);

                using (FileStream fs = File.Create(cachepath))
                using (BinaryWriter stream = new BinaryWriter(fs))
                {
                    stream.Write(1);
                    int n = _allDevices.Values.Count;
                    stream.Write(n);
                    foreach (var obj in _allDevices.Values)
                    {
                        if (obj.Status == null)
                            stream.Write(false);
                        else
                        {
                            stream.Write(true);
                            obj.Status.Serializer(stream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"SaveDeviceStatusCache {cachepath}");
            }
        }

        private Dictionary<long, DeviceStatus> LoadDeviceStatusCache()
        {
            Dictionary<long, DeviceStatus> ret = new Dictionary<long, DeviceStatus>();
            if (datapath == null) return ret;
            String cachepath = Path.Combine(datapath, DEVICE_STATUS_FILENAME);
            if (!File.Exists(cachepath)) return ret;

            try
            {
                using (FileStream fs = File.OpenRead(cachepath))
                using (BinaryReader stream = new BinaryReader(fs))
                {
                    int version = stream.ReadInt32();
                    int n = stream.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        if (stream.ReadBoolean())
                        {
                            DeviceStatus obj = new DeviceStatus();
                            obj.Deserializer(stream, version);
                            ret[obj.Serial] = obj;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"LoadDeviceStatusCache {cachepath}");
            }
            return ret;
        }

        private void LoadOldSerials()
        {
            if (datapath == null) return;

            long item; while (!_allOldSerials.IsEmpty)
                _allOldSerials.TryTake(out item);

            String rawlogpath = Path.Combine(datapath, OLDSERIALS_FILENAME);
            try
            {
                if (!File.Exists(rawlogpath)) return;
                foreach (String serial in File.ReadAllLines(rawlogpath))
                {
                    if (String.IsNullOrWhiteSpace(serial)) continue;
                    _allOldSerials.Add(long.Parse(serial));
                }
                _log.Info("CACHE", $"_allOldSerials.Count {_allOldSerials.Count}");
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"_allOldSerials {rawlogpath}");
            }
        }

        private void SaveOldSerials()
        {
            if (datapath == null) return;
            if (!_OldSerialsChanged) return;
            String cachepath = Path.Combine(datapath, OLDSERIALS_FILENAME);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(cachepath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachepath));
                if (File.Exists(cachepath)) File.Delete(cachepath);
                File.WriteAllText(cachepath, String.Join("\n", _allOldSerials.ToArray()));
                _OldSerialsChanged = false;
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"SaveOldSerials {cachepath}");
            }
        }

        public void TrackOldSerial(long serial)
        {
            if (!_allOldSerials.Contains(serial))
            {
                _allOldSerials.Add(serial);
                _OldSerialsChanged = true;
            }
        }

        public bool ContainOldSerial(long serial)
        {
            return _allOldSerials.Contains(serial);
        }



        private void LoadReplaceSerials()
        {
            if (datapath == null) return;

            _allReplaceSerials.Clear();

            String rawlogpath = Path.Combine(datapath, REPLACESERIALS_FILENAME);
            try
            {
                if (!File.Exists(rawlogpath)) return;
                foreach (String serial in File.ReadAllLines(rawlogpath))
                {
                    if (String.IsNullOrWhiteSpace(serial)) continue;
                    _allReplaceSerials.TryAdd(long.Parse(serial),true);
                }
                _log.Info("CACHE", $"_allReplaceSerials.Count {_allReplaceSerials.Count}");
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"_allReplaceSerials {rawlogpath}");
            }
        }

        private void SaveReplaceSerials()
        {
            if (datapath == null) return;
            if (!_ReplaceSerialsChanged) return;
            String cachepath = Path.Combine(datapath, REPLACESERIALS_FILENAME);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(cachepath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cachepath));
                if (File.Exists(cachepath)) File.Delete(cachepath);
                File.WriteAllText(cachepath, String.Join("\n", _allReplaceSerials.Keys.ToArray()));
                _ReplaceSerialsChanged = false;
            }
            catch (Exception e)
            {
                _log.Exception("CACHE", e, $"SaveOldSerials {cachepath}");
            }
        }

        public void TrackReplaceSerial(long serial)
        {
            if (!_allReplaceSerials.ContainsKey(serial))
            {
                _allReplaceSerials.TryAdd(serial,true);
                _ReplaceSerialsChanged = true;
            }
        }

        public bool ContainReplaceSerial(long serial)
        {
            return _allReplaceSerials.ContainsKey(serial);
        }

        public void UntrackReplaceSerial(long serial)
        {
            if (_allReplaceSerials.ContainsKey(serial))
            {
                bool item;
                _allReplaceSerials.TryRemove(serial, out item);
                _ReplaceSerialsChanged = true;
            }
        }

        public List<long> ReplaceSerialList()
        {
            return _allReplaceSerials.Keys.ToList();
        }

    }
}