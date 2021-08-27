using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class HasManyColumnAttribute : ExportAttribute, IHasManyColumn
    {
        public string Name { get; set; }

        public string CustomSqlType { get; set; }

        public string ForeignKeyName { get; set; }

        public HasManyType Type { get; set; }

        public bool LazyLoad { get; set; }

        public Type Child { get; set; }

        public Type Key { get; set; }

        public string KeyName { get; set; }
    }
}