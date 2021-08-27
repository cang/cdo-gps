using System.Threading.Tasks;

namespace Log
{
    /// <summary>
    /// The AttackLog interface.
    /// </summary>
    public interface IAttackLog
    {
        /// <summary>
        /// hàm nhận thông tin log từ trình quản lý
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="log">
        /// log data
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> Writer(string tag,LogType type,string log);

        /// <summary>
        /// cài đặt đường dẫn cần lưu log
        /// </summary>
        /// <param name="path"></param>
        void SetPath(string path);
    }
}