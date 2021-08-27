#region header
// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : IZip.cs
// Time Create : 1:12 PM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;

namespace Datacenter.Model.Log.ZipLog
{
    public interface IZip:IDbLog
    {
        long Id { get; set; }
        byte[] Data { get; set; }
        DateTime TimeUpdate { get; set; }
    }
    public interface IDeviceZip : IZip
    {
        long Serial { get; set; }
    }
}