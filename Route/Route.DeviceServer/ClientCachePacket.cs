#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : ClientCachePacket.cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using ServerCore;
using DevicePacketModels;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Route.DeviceServer
{
    [Export(typeof (IClientCachePacket))]
    [Export(typeof(ClientCachePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ClientCachePacket : IClientCachePacket
    {
        private readonly ConcurrentDictionary<long, ConcurrentQueue<byte[]>> _allClientPacket =
            new ConcurrentDictionary<long, ConcurrentQueue<byte[]>>();

        private readonly ConcurrentDictionary<long, PBaseSyncPacket> _allUnknownDevices 
            = new ConcurrentDictionary<long, PBaseSyncPacket>();

        //private readonly ConcurrentDictionary<long, long> _allRawLogSerials
        //    = new ConcurrentDictionary<long, long>();

        public byte[] Pop(long id)
        {
            ConcurrentQueue<byte[]> data;
            if (!_allClientPacket.TryGetValue(id, out data))
            {
                return null;
            }
            byte[] result;
            if (data.Count <= 0 || !data.TryDequeue(out result))
                return null;
            return result;
        }

        public bool Push(long id, byte[] p)
        {
            ConcurrentQueue<byte[]> data;
            if (!_allClientPacket.TryGetValue(id, out data))
            {
                data = new ConcurrentQueue<byte[]>();
                if (!_allClientPacket.TryAdd(id, data)) return false;
            }
            data.Enqueue(p);
            return true;
        }

        public void Clear(long id)
        {
            ConcurrentQueue<byte[]> data;
            if (!_allClientPacket.TryGetValue(id, out data))
            {
                return;
            }
            while (!data.IsEmpty)
            {
                byte[] tmp;
                data.TryDequeue(out tmp);
            }
        }

        public PBaseSyncPacket GetUnknownDevice(long serial)
        {
            PBaseSyncPacket tmp;
            _allUnknownDevices.TryGetValue(serial, out tmp);
            return tmp;
        }

        public bool UpdateUnknownDevice(long serial, PBaseSyncPacket pack)
        {
            PBaseSyncPacket tmp;
            if (_allUnknownDevices.TryGetValue(serial, out tmp))
            {
                return _allUnknownDevices.TryUpdate(serial, pack, tmp);
            }
            return _allUnknownDevices.TryAdd(serial, pack);
        }

        public bool RemoveUnknownDevice(long serial)
        {
            PBaseSyncPacket tmp;
            return _allUnknownDevices.TryRemove(serial, out tmp);
        }

        public bool TryRemoveUnknownDevice(long serial)
        {
            if (!_allUnknownDevices.ContainsKey(serial)) return false;

            PBaseSyncPacket tmp;
            if (_allUnknownDevices.TryGetValue(serial, out tmp))
                return _allUnknownDevices.TryRemove(serial,out tmp);
            return false;
        }

        public IList<PBaseSyncPacket> GetAllUnknownDevices()
        {
            var result = new List<PBaseSyncPacket>();
            var tmp = _allUnknownDevices.GetEnumerator();
            while (tmp.MoveNext())
            {
                result.Add(tmp.Current.Value);
            }
            return result;
        }

        //public bool ContainRawLogSerial(long serial)
        //{
        //    return _allRawLogSerials.ContainsKey(serial);
        //}

        //public void TrackRawLogSerial(long serial)
        //{
        //    if (!_allRawLogSerials.ContainsKey(serial))
        //        _allRawLogSerials.TryAdd(serial, serial);
        //}

    }
}
