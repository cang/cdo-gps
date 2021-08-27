#region header
// /*********************************************************************************************/
// Project :Route.Sync
// FileName : P312GetSerialRouteTable.cs
// Time Create : 3:29 PM 03/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Route.Sync.Models
{
    [Export(typeof(NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(312)]
    public class P312GetSerialRouteTable:NodeSharePacketModel
    {
        public P312GetSerialRouteTable()
        {
            
        }

        public P312GetSerialRouteTable(byte[] data):base(data)
        {
            
        }

        public Guid IdDatacenter { get; set; }
        #region Overrides of PacketModel

        public override bool Deserializer()
        {
            var id = ReadString(50);
            IdDatacenter = Guid.Parse(id);
            return true;
        }

        #region Overrides of PacketModel

        public override byte[] Serializer()
        {
            WriteString(IdDatacenter.ToString(), 50);
            return base.Serializer();
        }

        #endregion

        #endregion
    }
}