using DaoDatabase.AutoMapping.Enums;

namespace DaoDatabase.AutoMapping.IAttribute
{
    public interface IHasOneColumn
    {
        HasOneType Type { get; }
    }
}