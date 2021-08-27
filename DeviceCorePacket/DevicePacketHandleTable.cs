#region header

// /*********************************************************************************************/
// Project :CorePacket
// FileName : HandleTable.cs
// Time Create : 1:55 PM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CorePacket.Utils;
using Log;

namespace CorePacket
{
    [Export(typeof (IDevicePacketHandleTable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DevicePacketHandleTable : IDevicePacketHandleTable, IPartImportsSatisfiedNotification
    {
        private readonly IDictionary<int, IDeviceHandlePacket> _handles = new Dictionary<int, IDeviceHandlePacket>();
        [ImportMany] private IEnumerable<Lazy<IDeviceHandlePacket, IOpcodeAttribute>> _importHandles;
        [Import] private ILog _log;

        #region Implementation of IHandleTable

        public IDeviceHandlePacket GetHandle(int opcode)
        {
            IDeviceHandlePacket result;
            _handles.TryGetValue(opcode, out result);
            return result;
        }

        #endregion

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            foreach (var lp in _importHandles)
            {
                if (!_handles.ContainsKey(lp.Metadata.Opcode))
                    _handles.Add(lp.Metadata.Opcode, lp.Value);
                else
                    _log.Warning("HandleTable",
                        $"Trùng opcode {lp.Metadata.Opcode} -- {lp.Value.GetType()} ----> {_handles[lp.Metadata.Opcode].GetType()}");
            }
        }

        #endregion
    }
}