#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : OpcodeAttribute.cs
// Time Create : 4:12 PM 26/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.ComponentModel.Composition;

namespace NodeServerPlatform.Core.Utils
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeShareOpCodeAttribute : ExportAttribute, IOpcodeAttribute
    {
        public NodeShareOpCodeAttribute(int op)
        {
            Opcode = op;
        }

        #region Implementation of IOpcodeAttribute

        public int Opcode { get; }

        #endregion
    }

    public interface IOpcodeAttribute
    {
        int Opcode { get; }
    }
}