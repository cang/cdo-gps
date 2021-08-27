using DaoDatabase.AutoMapping.Enums;

namespace DaoDatabase.AutoMapping.IAttribute
{
    public interface IPrimaryKeyColumn : IColumn
    {
        KeyGenerateType KeyGenerateType { get; }

        string ForeignKey { get; }
    }
}