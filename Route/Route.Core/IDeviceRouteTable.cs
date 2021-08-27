using System.Collections.Generic;

namespace Route.Core
{
    /// <summary>
    /// cho phép các lớp truy xuất dữ liệu trên bảng định tuyến thiết bị
    /// </summary>
    public interface IDeviceRouteTable:IRouteTable<long>
    {
        IList<long> GetAll();
    }
}