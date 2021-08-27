#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : HandleTable.cs
// Time Create : 1:55 PM 27/01/2016
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
    [Export(typeof (INodeShareHandleTable))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class NodeShareHandleTable : INodeShareHandleTable, IPartImportsSatisfiedNotification
    {
        private readonly IDictionary<int, INodeShareHandlePacket> _handles = new Dictionary<int, INodeShareHandlePacket>();
        [ImportMany] private IEnumerable<Lazy<INodeShareHandlePacket, IOpcodeAttribute>> _importHandles;
        [Import] private ILog _log;

        #region Implementation of IHandleTable

        public INodeShareHandlePacket GetHandle(int opcode)
        {
            INodeShareHandlePacket result;
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