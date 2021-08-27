#region header

// /*********************************************************************************************/
// Project :Datacenter.UpdateDataToRoute
// FileName : UpdateDataConfig.cs
// Time Create : 8:56 AM 23/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Runtime.Serialization;
using ConfigFile;

namespace Datacenter.RegisterRoute
{
    [DataContract]
    public class UpdateDataConfig : IConfigObject
    {
        [DataMember]
        public string DefaultIp { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string MyIp { get; set; }
        [DataMember]
        public int MyPort { get; set; }
        [DataMember]
        public string Name { get; set; }

        #region Implementation of IConfigObject

        public void Fix()
        {
            if (string.IsNullOrEmpty(DefaultIp))
                DefaultIp = "127.0.0.1";

            if (Port == 0)
                Port = 1500;
            if (string.IsNullOrEmpty(MyIp))
                MyIp = "http://127.0.0.1";
            if (MyPort == 0)
                MyPort = 80;
            if (string.IsNullOrEmpty(Name))
                Name = "DATACENTER_01";
        }

        #endregion
    }
}