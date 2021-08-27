#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Packet
// FileName : NeigborInfo.cs
// Time Create : 8:17 AM 27/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

namespace NodeServerPlatform.Core.Models
{
    public class NeighborInfo
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public short Port { get; set; }
    }
}