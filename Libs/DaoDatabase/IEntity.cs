// --------------------------------------------------------------------------------------------------------------------
namespace DaoDatabase
{
    /// <summary>
    /// The Entity interface.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        void FixNullObject();
    }
    public interface IComponent { }

    public interface ISettingModel
    {
        void Default();
        
    }
}