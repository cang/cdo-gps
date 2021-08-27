#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: DevicePacketModels
// TIME CREATE : 7:04 PM 01/11/2016
// FILENAME: P114EndOvertime.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.ExternModel;
using DevicePacketModels.Utils;

#endregion

namespace DevicePacketModels.Events
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(116)]
    [Export(typeof (DevicePacketModel))]
    [DataContract]
    public class P116EndGuest : BaseEvent
    {
        public P116EndGuest(byte[] data) : base(data)
        {
        }
        public P116EndGuest()
        {
        }
    }

}