using System;
using System.Collections.Generic;
using FluentNHibernate.Cfg.Db;

namespace DaoDatabase
{
    public class NhibernateConfig:DatabaseConfig
    {
        public IPersistenceConfigurer Config { set; get; }
        public ICollection<Type> Maps { get; set; }
        public bool isCreateNew { get; set; }
    }
}