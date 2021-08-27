#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : DriverLogic.cs
// Time Create : 11:06 AM 28/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using Datacenter.Model.Entity;
using DevicePacketModels;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    ///     quản lý thông tin tài xế
    /// </summary>
    [Sort(2)]
    public class DriverLogic : ILogic
    {
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            //var dr = uTils.DataCache.GetQueryContext<Driver>().GetByKey(device.Status.DriverStatus.DriverId);
            //if (dr != null)
            //{
            //    //todo: quản lý thời gian lái xe và các thông tin liên quan đến tài xế
            //}
        }
    }
}