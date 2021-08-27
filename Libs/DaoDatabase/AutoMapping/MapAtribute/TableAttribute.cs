using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : ExportAttribute, ITable
    {
        public string Name { get; set; }

        public string Schema { get; set; }
        public DbSupportType DbType { get; set; }

        public TableAttribute()
        {
            DbType = DbSupportType.MicrosoftSqlServer;
        }
    }
}