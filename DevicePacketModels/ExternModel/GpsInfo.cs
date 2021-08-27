#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: DevicePacketModels
// TIME CREATE : 6:18 PM 06/04/2016
// FILENAME: GpsInfo.cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

namespace DevicePacketModels.ExternModel
{
    public class GpsInfo
    {
        public float Lat { get; set; }
        public float Lng { get; set; }
        public byte Speed { get; set; }

        public override string ToString()
        {
            return $"{Lat}, {Lng}";
        }

        public bool IsValid()
        {
            return (Lat >= -90 && Lat <= 90
                && Lng >= -180 && Lng <= 180);
        }

    }
}