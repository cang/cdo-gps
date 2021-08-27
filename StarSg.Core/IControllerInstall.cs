using System.Net.Http.Headers;
using System.Web.Http.Dependencies;

namespace StarSg.Core
{
    /// <summary>
    ///     cài đặt các thông tin cần thiết cho 1 controller hoạt động
    /// </summary>
    public interface IControllerInstall
    {
        /// <summary>
        ///     Kiểm tra thông tin truy cập
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        bool ValidAccess(IDependencyResolver dependency, HttpRequestHeaders header);

        /// <summary>
        ///     cài đặt một số thông tin phụ kèm theo cho mỗi request.
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        bool InstallOption(IDependencyResolver dependency, HttpRequestHeaders header);
    }
}