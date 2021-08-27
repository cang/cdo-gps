using System;
using DaoDatabase.AutoMapping.Enums;

namespace DaoDatabase.AutoMapping.IAttribute
{
    public interface IHasManyColumn : IColumn
    {
        string ForeignKeyName { get; }

        HasManyType Type { get; }

        bool LazyLoad { get; }

        Type Child { get; }

        Type Key { get; }

        string KeyName { get; }
    }
}