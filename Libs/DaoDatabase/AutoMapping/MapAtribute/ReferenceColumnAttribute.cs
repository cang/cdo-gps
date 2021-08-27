using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ReferenceColumnAttribute : ExportAttribute, IReferenceColumn
    {
        public string Name { get; set; }

        public string CustomSqlType { get; set; }
    }
}