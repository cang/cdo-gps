namespace DaoDatabase.AutoMapping.IAttribute
{
    public interface IColumn
    {
        /// <summary>
        /// Tên cột (Nếu để trống thì lấy theo tên property)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của property khi ánh xạ vào sql, bỏ trống thì hệ thống sẽ lấy mặc định
        /// </summary>
        string CustomSqlType { get; }
    }
}