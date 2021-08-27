#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : ILogic.cs
// Time Create : 9:41 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using Datacenter.Model.Entity;
using DevicePacketModels;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles
{
    /// <summary>
    ///     xử lý các thông tin có được từ thiết bị
    /// </summary>
    public interface ILogic
    {
        /// <summary>
        ///     xử lý các thông tin
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="uTils"></param>
        /// <param name="device"></param>
        /// <param name="company"></param>
        void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company);
        //void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company);
    }
}