#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : ServerConfig.cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Runtime.Serialization;
using ConfigFile;

#endregion

namespace Route.DeviceServer
{
    [DataContract]
    public class ServerConfig : IConfigObject
    {
        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Port { get; set; }

        #region Implementation of IConfigObject

        public void Fix()
        {
            if (string.IsNullOrEmpty(Ip))
                Ip = "127.0.0.1";
            if (Port == 0)
                Port = 1300;
        }

        #endregion
    }
}