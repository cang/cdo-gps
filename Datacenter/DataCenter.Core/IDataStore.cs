#region header

// /*********************************************************************************************/
// Project :DataCenter.Core
// FileName : IDataCenterStore.cs
// Time Create : 2:21 PM 22/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using Datacenter.Model;
using Datacenter.Model.Entity;

namespace DataCenter.Core
{
    /// <summary>
    ///     chứa thông tin được lưu trữ trên memory.
    /// </summary>
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IDataStore : IQueryModel<Device>, IQueryModel<DeviceGroup>, IQueryModel<DeviceModel>, IQueryModel<PointGps>, IQueryModel<Area>, IQueryModel<Driver>
        , IQueryModel<FuelSheet>
    {
        bool AddCompany(Company c);
        bool RemoveCompany(long id);
        Company GetCompanyById(long id);
        IList<Company> GetAllCompany();
        IQueryModel<T> GetQueryContext<T>() where T : ICacheModel;

        RouteGpsLogic GetRoute(float lat, float lon);
        void Reload(int idSql, String datapath, String configpath);
        void SaveCacheData();
        void LoadRawLogSerials();
        bool ContainRawLogSerial(long serial);
        void TrackRawLogSerial(long serial);
        void SaveDeviceStatusCache();

        //void SaveOldSerials();
        void TrackOldSerial(long serial);
        bool ContainOldSerial(long serial);

        //void SaveReplaceSerials();
        void TrackReplaceSerial(long serial);
        bool ContainReplaceSerial(long serial);
        void UntrackReplaceSerial(long serial);
        List<long> ReplaceSerialList();
    }

    public interface IQueryModel<T> where T : ICacheModel
    {
        IList<T> GetAll();
        IList<T> GetByCompany(long companyId);
        IList<T> GetByGroup(long companyId, long groupId);
        T GetByKey(object key);
        IList<T> GetWhere(Func<T, bool> where);
        bool Add(T obj, long companyId);
        bool Del(T obj, long companyId);
    }

    /// <summary>
    ///     các sự kiện phát sinh khi có sự thay đổi dữ liệu trên cache , giúp lớp
    ///     cập nhật dữ liệu qua router dễ dàng làm việc hơn.
    /// </summary>
    public interface IDataCenterStoreEvent
    {
        event Action<Company> OnAddCompany;
        event Action<Company> OnRemoveCompany;
        event Action<Device> OnAddDevice;
        event Action<Device> OnRemoveDevice;

        ///// <summary>
        ///// chạy lại các thông tin đồng bộ
        ///// </summary>
        //void ReSync();

        List<long> GetAllCompanyId();
        List<long> GetAllDeviceSerial();
    }

}