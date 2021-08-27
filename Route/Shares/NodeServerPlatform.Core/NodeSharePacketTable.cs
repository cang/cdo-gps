#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : PacketTable.cs
// Time Create : 3:52 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Log;
using NodeServerPlatform.Core.Utils;

namespace NodeServerPlatform.Core
{
    [Export(typeof (INodeSharePacketTable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NodeSharePacketTable : IPartImportsSatisfiedNotification, INodeSharePacketTable
    {
        [ImportMany] private IEnumerable<Lazy<NodeSharePacketModel, IOpcodeAttribute>> _importPacket;
        [Import] private ILog _log;
        private IDictionary<int, Type> _recPacketTypes;
        private IDictionary<Type, int> _sendPacketTypes;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            _recPacketTypes = new Dictionary<int, Type>();
            _sendPacketTypes = new Dictionary<Type, int>();
            foreach (var lp in _importPacket)
            {
                if (!_recPacketTypes.ContainsKey(lp.Metadata.Opcode))
                {
                    _log.Debug("PACKET", $"RecvPacket Add OpCode : {lp.Metadata.Opcode} - {lp.Value.GetType()}");
                    _recPacketTypes.Add(lp.Metadata.Opcode, lp.Value.GetType());
                }
                if (!_sendPacketTypes.ContainsKey(lp.Value.GetType()))
                {
                    _log.Debug("PACKET", $"SendPacket Add Type :  {lp.Value.GetType()} - {lp.Metadata.Opcode}");
                    _sendPacketTypes.Add(lp.Value.GetType(), lp.Metadata.Opcode);
                }
                lp.Value.Dispose();
            }
        }

        #endregion

        #region Implementation of IPacketTable

        public Type GetPacket(int opcode)
        {
            Type result;
            _recPacketTypes.TryGetValue(opcode, out result);
            return result;
        }

        public int GetOpcode(Type type)
        {
            int result;
            if (!_sendPacketTypes.TryGetValue(type, out result)) result = -1;
            return result;
        }

        #endregion
    }
}