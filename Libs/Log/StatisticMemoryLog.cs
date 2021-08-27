using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    [Export(typeof(StatisticMemoryLog))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StatisticMemoryLog
    {
        private readonly ConcurrentDictionary<long, DeviceStatisticLog> _allDevices = new ConcurrentDictionary<long, DeviceStatisticLog>();
        private readonly ConcurrentDictionary<long, DeviceStatisticLog> _allDevice301 = new ConcurrentDictionary<long, DeviceStatisticLog>();

        [Import] private ILog _log;

        public void UpdateOpcode301Zero(long serial)
        {
            try
            {
                if (!_allDevices.ContainsKey(serial)) _allDevices.TryAdd(serial, new DeviceStatisticLog() { Serial = serial, Opcode301Zero = 1 });
                else _allDevices[serial].Opcode301Zero++;
            }
            catch (Exception e)
            {
                _log.Exception("StatisticMemoryLog", e, "UpdateOpcode301Zero");
            }
        }

        public void UpdateOpcode301Only(long serial)
        {
            try
            {
                if (!_allDevice301.ContainsKey(serial)) _allDevice301.TryAdd(serial, new DeviceStatisticLog() { Serial = serial, Opcode301Zero = 1 });
                else _allDevice301[serial].Opcode301Zero++;
            }
            catch (Exception e)
            {
                _log.Exception("StatisticMemoryLog", e, "UpdateOpcode301Only");
            }
        }

        public IList<DeviceStatisticLog> GetOpcode301ZeroList()
        {
            return _allDevices.Values.ToList();
        }

        public IList<DeviceStatisticLog> GetOpcode301ListOnly()
        {
            return _allDevice301.Values.ToList();
        }

        public void Reset()
        {
            try
            {
                foreach (var item in _allDevices.Values)
                {
                    item.Reset();
                }
                foreach (var item in _allDevice301.Values)
                {
                    item.Reset();
                }
            }
            catch (Exception e)
            {
                _log.Exception("StatisticMemoryLog", e, "Reset");
            }
        }

    }

    public class DeviceStatisticLog
    {
        public long Serial { get; set; }
        public int  Opcode301Zero { get; set; }
        public void Reset()
        {
            Opcode301Zero = 0;
        }
    }

}
