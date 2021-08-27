#region header

// /*********************************************************************************************/
// Project :DevicePacketModels
// FileName : StatusIO.cs
// Time Create : 8:33 AM 04/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using CorePacket.Utils;

namespace DevicePacketModels.ExternModel
{
    public class StatusIO
    {
        [BitField(Index = 0)]
        public bool Key { get; set; }

        [BitField(Index = 1)]
        public bool AirMachine { get; set; }

        [BitField(Index = 2)]
        public bool Sos { get; set; }

        [BitField(Index = 3)]
        public bool UseTemperature { get; set; }

        [BitField(Index = 4)]
        public bool UseFuel { get; set; }

        [BitField(Index = 5)]
        public bool UseRfid { get; set; }

        [BitField(Index = 6)]
        public bool Door { get; set; }
    }
}