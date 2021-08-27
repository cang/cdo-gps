#region header

// /*********************************************************************************************/
// Project :Datacenter.Api
// FileName : PointLogic.cs
// Time Create : 1:43 PM 01/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System.Linq;
using Datacenter.Model.Entity;
using Datacenter.Model.Log;
using DevicePacketModels;
using StarSg.Utils.Geos;
using System;

#endregion

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    /// <summary>
    /// 
    /// </summary>
    [Sort(5)]
    public class PointLogic : ILogic
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
            var checkPoints = uTils.DataCache.GetQueryContext<PointGps>().GetByGroup(company.Id,device.GroupId);
            if (checkPoints == null)
            {
                uTils.Log.Warning("LOGIC", "Chưa có khởi tạo context cho đối tượng GpsCheckPoint");
                return;
            }

            //Thử Lấy theo Nhóm nếu có thì xử lý theo Nhóm, nếu không có thì xử lý theo Công ty
            if (checkPoints.Count == 0) checkPoints = uTils.DataCache.GetQueryContext<PointGps>().GetByCompany(company.Id);

            //var point = checkPoints.FirstOrDefault(m => m.Location - device.Status.BasicStatus.GpsInfo < m.Radius);
            var point = checkPoints.FirstOrDefault(m => {
                return GeoUtil.Distance(m.Location.Lat, m.Location.Lng
                    , device.Status.BasicStatus.GpsInfo.Lat, device.Status.BasicStatus.GpsInfo.Lng) 
                    <= m.Radius;
            });

            // Kiểm tra trước đó xe có nằm trong điểm nào không ?
            if (device.Temp.IdPoint > 0)
            {
                //Nếu không trùng điểm thì ghi nhận sự kiện này 
                if (point == null || point.Id != device.Temp.IdPoint)
                {
                    uTils.DataContext.Insert(new PointTraceLog
                    {
                        PointId = device.Temp.IdPoint,
                        CompanyId = company.Id,
                        GroupId = device.GroupId,
                        DbId = company.DbId,
                        DeviceId = device.Serial,
                        DriverId = device.Status?.DriverStatus?.DriverId ?? 0,
                        Id = 0,
                        BeginTime = device.Temp.PointBeginTime,
                        EndTime = device?.Status?.BasicStatus?.ClientSend ?? DateTime.Now
                    }, company.DbId);
                    device.Temp.IdPoint = 0;//reset temp sau khi ghi nhận
                }
            }


            //Nếu có nằm trong Điểm 
            if (point != null)
            {
                if (device.Temp.IdPoint <= 0)
                {
                    device.Temp.IdPoint = point.Id;
                    device.Temp.PointBeginTime = device?.Status?.BasicStatus?.ClientSend ?? DateTime.Now;
                }
            }

        }

        #endregion
    }
}