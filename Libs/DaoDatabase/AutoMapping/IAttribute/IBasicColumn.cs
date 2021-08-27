namespace DaoDatabase.AutoMapping.IAttribute
{
    public interface IBasicColumn : IColumn
    {
        bool IsIndex { get; }

        string Default { get; }

        bool NotNull { get; }

        long Length { get; }
    }
}