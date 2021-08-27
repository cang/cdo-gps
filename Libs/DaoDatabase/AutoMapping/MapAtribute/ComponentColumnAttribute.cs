using System;
using System.ComponentModel.Composition;
using DaoDatabase.AutoMapping.IAttribute;

namespace DaoDatabase.AutoMapping.MapAtribute
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ComponentColumnAttribute : ExportAttribute, IComponentColumn
    {
        public int Index { get; set; }
    }
}