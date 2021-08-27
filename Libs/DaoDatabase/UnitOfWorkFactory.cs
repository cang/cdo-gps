using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DaoDatabase
{
    /// <summary>
    ///     The unit of work factory.
    /// </summary>
    public static class UnitOfWorkFactory
    {
        /// <summary>
        ///     danh sách các kết nối được tạo ra
        /// </summary>
        private static readonly IDictionary<DbSupportType, IDictionary<string, IDbFactory>> Factories =
            new Dictionary<DbSupportType, IDictionary<string, IDbFactory>>();

        /// <summary>
        ///     thông tin các cấu hình database được nạp vào hệ thống
        /// </summary>
        private static readonly IDictionary<string, DatabaseConfig> InfoConfig =
            new ConcurrentDictionary<string, DatabaseConfig>();


        /// <summary>
        ///     đăng ký database vào hệ thống
        /// </summary>
        /// <param name="dataNamse"></param>
        /// <param name="cfg"></param>
        public static void RegisterDatabase(string dataNamse, DatabaseConfig cfg)
        {
            if (InfoConfig.ContainsKey(dataNamse + cfg.Direct))
            {
                throw new Exception("Đã cấu hình cho data này rồi");
            }
            InfoConfig.Add(dataNamse + cfg.Direct, cfg);

            if (!Factories.ContainsKey(cfg.Direct))
            {
                Factories.Add(cfg.Direct, new Dictionary<string, IDbFactory>());
            }
            var unit = CreateUnitOfWork(cfg);
            Factories[cfg.Direct].Add(dataNamse, unit);
        }

        /// <summary>
        ///     Lấy thông tin phiên làm việc
        /// </summary>
        /// <param name="dataname">
        ///     database cần làm việc
        /// </param>
        /// <param name="type"></param>
        /// <returns>
        ///     The <see cref="Reponsitory" />.
        /// </returns>
        public static Reponsitory GetUnitOfWork(string dataname, DbSupportType type)
        {
            IDictionary<string, IDbFactory> listFactor;
            if (!Factories.TryGetValue(type, out listFactor))
                throw new Exception("Type database doesn't register");
            IDbFactory factory;
            if (!listFactor.TryGetValue(dataname, out factory))
                throw new Exception("Database doesn't register");
            return factory.CreateContext(dataname);
        }
        
        /// <summary>
        ///     Tạo 1 unit với database name tryền vào
        /// </summary>
        /// <returns>
        ///     The <see cref="IDbFactory" />.
        /// </returns>
        private static IDbFactory CreateUnitOfWork(DatabaseConfig cfg)
        {
            switch (cfg.Direct)
            {
                case DbSupportType.MongoDb:
                {
                    var config = cfg as MongoConfig;
                    if (config == null) throw new Exception("Cấu hình database không chính xác");
                    var unit = new MongoFactory(config);
                    // unit.Create(key);
                    return unit;
                }
                case DbSupportType.MicrosoftSqlServer:
                {
                    var config = cfg as NhibernateConfig;
                    if (config == null) throw new Exception("Cấu hình database không chính xác");

                    var unit = new NhibernateFactory(config);
                    //unit.Create(key);
                    return unit;
                }
                default:
                    throw new Exception("Chưa hỗ trợ kiểu cơ sở dữ liệu này");
            }
        }
    }
}