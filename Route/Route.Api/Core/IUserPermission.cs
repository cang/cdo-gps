namespace Route.Api.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserPermisionManager
    {
        /// <summary>
        /// kiểm tra xem công ty này có được quản lý hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ContainCompanyId(long id);
        /// <summary>
        /// kiểm tra xem thiết bị có được quản lý hay không
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        bool ContainDeviceSerial(long serial);
        /// <summary>
        /// Lấy cấp bậc tài khoản
        /// </summary>
        /// <returns></returns>
        int GetLevel();
        /// <summary>
        /// Kiểm tra  xem user có hợp lệ hay không
        /// </summary>
        /// <returns></returns>
        bool Check();

    }
}