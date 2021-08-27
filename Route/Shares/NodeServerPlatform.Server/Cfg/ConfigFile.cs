#region header

// /*********************************************************************************************/
// Project :NodeServerPlatform.Server
// FileName : ConfigFile.cs
// Time Create : 3:11 PM 28/01/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using NodeServerPlatform.Core.Models;

namespace NodeServerPlatform.Server.Cfg
{
    [DataContract]
    public class ConfigFile
    {
        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public short Port { get; set; }

        [DataMember]
        public IDictionary<string, NeighborInfo> Neighbor { get; set; } = new Dictionary<string, NeighborInfo>();

        [DataMember]
        public string TmpIp { get; set; }

        [DataMember]
        public int TmpPort { get; set; }

        [DataMember]
        public string TmpName { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}