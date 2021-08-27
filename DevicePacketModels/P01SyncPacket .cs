#region header

// /*********************************************************************************************/
// Project :DevicePacketModels
// FileName : SyncPacket .cs
// Time Create : 8:26 AM 04/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.ExternModel;
using DevicePacketModels.Utils;

namespace DevicePacketModels
{
    [DeviceOpCode(1)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P01SyncPacket : PBaseSyncPacket
    {
        public P01SyncPacket()
        {
        }

        public P01SyncPacket(byte[] data) : base(data)
        {
        }
    }

}