#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : AreaLogic.cs
// Time Create : 1:46 PM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Linq;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using Datacenter.Model.Utils;
using DevicePacketModels;
using System;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    [Sort(5)]
    public class AreaLogic : ILogic
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
            var areas = uTils.DataCache.GetQueryContext<Area>().GetByGroup(company.Id, device.GroupId);
            if (areas == null)
            {
                uTils.Log.Warning("LOGIC", "Chưa có khởi tạo context cho đối tượng Area");
                return;
            }

            //Thử Lấy theo Nhóm nếu có thì xử lý theo Nhóm, nếu không có thì xử lý theo Công ty
            if (areas.Count==0) areas = uTils.DataCache.GetQueryContext<Area>().GetByCompany(company.Id);

            // kiểm tra xem thiết bị đang ở trong area nào.
            var area = areas.FirstOrDefault(m => m.Contain(device.Status.BasicStatus.GpsInfo));

            // Kiểm tra trước đó xe có nằm trong vùng nào không ?
            if (device.Temp.IdArea > 0)
            {
                //Nếu không trùng vùng thì ghi nhận sự kiện này 
                if(area==null || area.Id!= device.Temp.IdArea)
                {
                    uTils.DataContext.Insert(new AreaTraceLog
                    {
                        AreaId = device.Temp.IdArea,
                        CompanyId = company.Id,
                        GroupId = device.GroupId,
                        DbId = company.DbId,
                        DeviceId = device.Serial,
                        DriverId = device.Status?.DriverStatus?.DriverId ?? 0,
                        Id = 0,
                        BeginTime = device.Temp.AreaBeginTime,
                        EndTime = device?.Status?.BasicStatus?.ClientSend ?? DateTime.Now
                    }, company.DbId);
                    device.Temp.IdArea = 0;//reset temp sau khi ghi nhận
                }
            }


            //Nếu có nằm trong vùng 
            if(area!=null)
            {
                if (device.Temp.IdArea <= 0)
                {
                    device.Temp.IdArea = area.Id;
                    device.Temp.AreaBeginTime = device?.Status?.BasicStatus?.ClientSend ?? DateTime.Now;
                }
            }

        }

        #endregion
    }
}