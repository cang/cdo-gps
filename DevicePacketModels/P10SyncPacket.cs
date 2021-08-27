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
    [DeviceOpCode(10)]
    [Export(typeof(DevicePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [DataContract]
    public class P10SyncPacket : PBaseSyncPacket
    {
        public P10SyncPacket()
        {
        }

        public P10SyncPacket(byte[] data) : base(data)
        {
        }
    }
}
