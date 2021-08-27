using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class HasOneColumnAttribute : ExportAttribute, IHasOneColumn
    {
        public HasOneType Type { get; set; }
    }
}