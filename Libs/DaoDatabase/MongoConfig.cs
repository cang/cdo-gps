using System;
using System.Collections.Generic;

namespace DaoDatabase
{
    public class MongoConfig:DatabaseConfig
    {
        public string Host { set; get; }
        public int Port { set; get; }
        public string DbName { set; get; }
        public IList<Type> Maps { set; get; }
        public bool IsCreateNew { get; set; }

        public MongoConfig():base(DbSupportType.MongoDb)
        {
            Maps=new List<Type>();
        }
    }
}