using System.Collections.Generic;

namespace Route.Api.Hubs
{
    /// <summary>
    /// Thông tin client theo serial
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClientInfo<T>
    {
        public T Client { get; set; }
        public string Id { get; set; }
        public IList<long> Serials { get; set; } = new List<long>();
    }
}