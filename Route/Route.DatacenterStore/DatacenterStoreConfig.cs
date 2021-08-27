#region header

// /*********************************************************************************************/
// Project :Route.DatacenterStore
// FileName : DatacenterStoreConfig.cs
// Time Create : 9:20 AM 24/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System.Runtime.Serialization;
using ConfigFile;

namespace Route.DatacenterStore
{
    [DataContract]
    public class DatacenterStoreConfig : IConfigObject
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
                Port = 1500;
        }

        #endregion
    }
}