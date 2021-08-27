using DaoDatabase;
using DaoDatabase.AutoMapping.MapAtribute;

namespace Datacenter.Model
{
    public interface IDbLog : IEntity
    {
        /// <summary>
        ///     Id của máy chủ lưu trữ dữ liệu log
        /// </summary>
        [BasicColumn]
        int DbId { get; set; }
    }
}