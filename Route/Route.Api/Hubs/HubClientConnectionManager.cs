using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Tranfer;
using StarSg.Utils.Models.DatacenterResponse.Status;

namespace Route.Api.Hubs
{
    public class HubClientConnectionManager<T>
    {
        private readonly ConcurrentDictionary<string, IList<ClientInfo<T>>> _connections =
            new ConcurrentDictionary<string, IList<ClientInfo<T>>>();

        private readonly ConcurrentDictionary<long, ConcurrentList<T>> _serialClients =
            new ConcurrentDictionary<long, ConcurrentList<T>>();

        /// <summary>
        /// đăng ký Websocket
        /// </summary>
        /// <param name="token"></param>
        /// <param name="client"></param>
        /// <param name="serials"></param>
        public void RegisterConnection(string token, T client,string id, IList<long> serials)
        {
            IList<ClientInfo<T>> tmp;
            if (!_connections.TryGetValue(token, out tmp))
            {
                tmp = new List<ClientInfo<T>>();
                _connections.TryAdd(token, tmp);
            }
            tmp.Add(new ClientInfo<T> {Client = client, Serials = serials,Id = id});
            //if (_connections.TryAdd(token, ))
            {
                foreach (var serial in serials)
                {
                    if (_serialClients.ContainsKey(serial))
                        _serialClients[serial].Add(client);
                }
            }
        }
        /// <summary>
        /// hủy đăng ký WebSocket
        /// </summary>
        /// <param name="token"></param>
        public void UnregisterConnection(string token,string id)
        {
            IList<ClientInfo<T>> tmp;
            if (_connections.TryGetValue(token, out tmp))
            {
                // do trong danh sách client của mỗi serial rồi remove cái client đó đi
                var client = tmp.FirstOrDefault(m => m.Id == id);
                if(client!=null)
                foreach (var serial in client.Serials)
                {
                    ConcurrentList<T> clients;
                    if (_serialClients.TryGetValue(serial, out clients))
                    {
                        clients.Remove(client.Client);
                    }
                }
            }
        }
        /// <summary>
        /// Cập nhật dang sách quản lý các socket theo serials
        /// </summary>
        /// <param name="serials"></param>
        public void InsertOrUpdate(IList<long> serials)
        {
            foreach (var serial in serials)
            {
                if (!_serialClients.ContainsKey(serial))
                {
                    _serialClients.TryAdd(serial, new ConcurrentList<T>());
                }
            }
        }

        /// <summary>
        /// lấy toàn bộ socket mà serial đó quản lý
        /// </summary>
        /// <param name="serial"></param>
        public IList<T> GetAllSocketBySerial(long serial)
        {
            IList<T> result = new List<T>();
            ConcurrentList<T> tmp;
            if (_serialClients.TryGetValue(serial,out tmp))
            {
                result = tmp.ToArray();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial"></param>
        public void RemoveSerial(long serial)
        {
            if (_serialClients.ContainsKey(serial))
            {
                ConcurrentList<T> tmp;
                _serialClients.TryRemove(serial, out tmp);
            }
        }
    }
}