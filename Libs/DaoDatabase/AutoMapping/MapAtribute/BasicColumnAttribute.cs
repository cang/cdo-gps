using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BasicColumnAttribute : ExportAttribute, IBasicColumn
    {
        public string Name { get; set; }

        public string CustomSqlType { get; set; }

        public bool NotNull { get; set; }

        public long Length { get; set; }

        public bool IsIndex { get; set; }

        public string Default { get; set; }
    }
}