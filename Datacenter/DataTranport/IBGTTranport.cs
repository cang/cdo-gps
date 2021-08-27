#region header
// /*********************************************************************************************/
// Project :Datacenter.DataTranport
// FileName : IBGTTranport.cs
// Time Create : 8:29 AM 02/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using Datacenter.DataTranport.Models;
using Datacenter.Model.Utils;

namespace Datacenter.DataTranport
{
    public interface IBgtTranport
    {
        void setConfigPath(string datapath, string configpath);
        bool Push(long serial, DeviceInfo his, DeviceTemp devicetemp);
    }
}