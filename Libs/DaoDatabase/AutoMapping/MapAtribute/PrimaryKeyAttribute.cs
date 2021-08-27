using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrimaryKeyAttribute : ExportAttribute, IPrimaryKeyColumn
    {
        public string Name { get; set; }

        public string CustomSqlType { get; set; }

        public KeyGenerateType KeyGenerateType { get; set; }

        public string ForeignKey { get; set; }
    }
}