#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : ValidDeviceLogic.cs
// Time Create : 2:53 PM 23/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using Datacenter.Model.Components;
using Datacenter.Model.Entity;
using DevicePacketModels;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    [Sort(0)]
    public class ValidDeviceLogic : ILogic
    {
        #region Implementation of ILogic

        /// <summary>
        ///     xử lý các thông tin
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="uTils"></param>
        /// <param name="device"></param>
        /// <param name="company"></param>
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            if (device.Status == null)
            {
                device.Status = new DeviceStatus
                {
                    Device = device,
                    Serial = device.Serial,
                    BasicStatus = new DeviceStatusInfo()
                };
                uTils.Log.Debug("ValidDevice", $"Thêm mới thông tin status của thiết bị {device.Status.Serial}");
                uTils.DataContext.Insert(device.Status, uTils.MotherSqlId);
                uTils.DataContext.Commit(uTils.MotherSqlId);
            }
            if (device.SimInfo == null)
            {
                device.SimInfo = new DeviceSimInfo {Device = device, Serial = device.Serial};
                uTils.Log.Debug("ValidDevice", $"Thêm mới thông tin sim của thiết bị {device.Status.Serial}");
                uTils.DataContext.Insert(device.SimInfo, uTils.MotherSqlId);
                uTils.DataContext.Commit(uTils.MotherSqlId);
            }
            if (device.SetupInfo == null)
            {
                device.SetupInfo = new DeviceSetupInfo {Device = device, Serial = device.Serial};
                uTils.Log.Debug("ValidDevice", $"Thêm mới thông tin cấu hình của thiết bị {device.Status.Serial}");
                uTils.DataContext.Insert(device.SetupInfo, uTils.MotherSqlId);
                uTils.DataContext.Commit(uTils.MotherSqlId);
            }
        }

        #endregion
    }
}