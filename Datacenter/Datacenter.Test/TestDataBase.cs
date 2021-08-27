#region header

// /*********************************************************************************************/
// Project :Datacenter.Test
// FileName : TestDataBase.cs
// Time Create : 3:43 PM 15/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DaoDatabase;
using DaoDatabase.AutoMapping;
using Datacenter.Model;
using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datacenter.Model.Log.ZipLog;
using Core.Utils;
using StarSg.Utils.Utils;

#endregion

namespace Datacenter.Test
{
    [TestClass]
    public class TestDataBase
    {
        [TestMethod]
        public void TestInsertDeviceLog()
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

            //UnitOfWorkFactory.RegisterDatabase("ABCREPORT", new NhibernateConfig
            //{
            //    Maps = mapEntity,
            //    Config =
            //             DatabaseConfigFactory.GetDataConfig(false, "210.211.109.130", 0, "Report1",
            //                 "sa", "@123456a", false, null)
            //});

            //var dataContextReport = UnitOfWorkFactory.GetUnitOfWork("ABCREPORT", DbSupportType.MicrosoftSqlServer);

            var dataContext = UnitOfWorkFactory.GetUnitOfWork("ABC", DbSupportType.MicrosoftSqlServer);

            dataContext.Insert(new DeviceLog()
            {
                CompanyId = 1,
                DbId = 1,
                DeviceStatus = new DeviceStatusInfo(),
                DriverStatus = new DriverStatusInfo(),
                GroupId = 0,
                Id = 0,
                Indentity = Guid.Empty,
                Serial = 1,
            });
            
            dataContext.Commit();
        }

        //[TestMethod]
        //public void TestZipDeviceLog()
        //{
        //    var mapEntity = Assembly.GetAssembly(typeof(Company))
        //           .GetTypes()
        //           .Where(m => m.GetInterfaces().FirstOrDefault(x => x == typeof(IEntity)) != null && m.IsClass)
        //           .Select(
        //               m =>
        //               {
        //                   var makeme = typeof(FactoryMap<>).MakeGenericType(m);
        //                   return makeme;
        //               }).ToList();

        //    UnitOfWorkFactory.RegisterDatabase("ABC", new NhibernateConfig
        //    {
        //        Maps = mapEntity,
        //        Config =
        //                DatabaseConfigFactory.GetDataConfig(false, "127.0.0.1", 0, "SgsiData",
        //                    "sa", "@123456a", false, null)
        //    });

        //    var dataContext = UnitOfWorkFactory.GetUnitOfWork("ABC", DbSupportType.MicrosoftSqlServer);

        //    // chuyển thời gian về ngày hôm qua , task này luôn chạy sau 0h nên dữ liệu phải đọc của ngày hôm trước
        //    var now = new DateTime(2017, 02, 22, 0, 0, 1);
        //    var begin = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0); // đầu ngày
        //    var end = begin.AddDays(1); // cuối ngày

        //    try
        //    {
        //        Guid guid = Guid.Parse("6619A23A-DE45-4F68-B34F-B35A2474FF4C");//17010029

        //        // đọc log trong 1 ngày ra
        //        var allLog =
        //            dataContext.GetWhere<DeviceLog>(
        //                m =>
        //                    m.DeviceStatus.ClientSend >= begin && m.DeviceStatus.ClientSend < end &&
        //                    m.Indentity == guid);

        //        //check data nén
        //        if (allLog.Count == 0)
        //        {
        //            Console.WriteLine("if (allLog.Count == 0)");
        //            return;
        //        }

        //        DeviceLog first = allLog.FirstOrDefault();

        //        // nén dữ liệu lại 
        //        //todo: do một số trường trong record ko cần thiết nên tạo thêm 1 lớp chuyển đổi nữa
        //        //var data = allLog.ObjectToByteArray().Zip();
        //        DeviceLogCollection sellog = new DeviceLogCollection(allLog);
        //        var data = sellog.Serializer().Zip();

        //        DeviceLogCollection sellog1 = new DeviceLogCollection();
        //        sellog1.Deserializer(data.UnZip());

        //        // lưu vào bảng mới
        //        dataContext.Insert<DeviceLogZip>(new DeviceLogZip
        //            {
        //                //Id = first.Id,
        //                Data = data,
        //                Serial = first.Serial,
        //                TimeUpdate = now,
        //                DbId = 0,
        //                CompanyId = first.CompanyId,
        //                GroupId = first.GroupId,
        //                Indentity = guid,
        //            });
        //        dataContext.Commit();

        //        dataContext.DeleteWhere<DeviceLog>(
        //            o => o.Indentity == guid
        //            && o.DeviceStatus.ClientSend >= begin
        //            && o.DeviceStatus.ClientSend < end
        //            );
        //        dataContext.Commit();

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("ZipDeviceLogLogic", e, $"Lỗi nén data device: {16071801}");
        //    }
        //}

    }
}