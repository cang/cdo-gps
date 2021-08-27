using Datacenter.Model.Entity;
using DevicePacketModels;

namespace Datacenter.Api.Core.DeviceLogicHandles.Logics
{
    [Sort(5)]
    public class OverSpeedLogic:ILogic
    {
        //public void Handle(P01SyncPacket packet, ILogicUtil uTils, Device device, Company company)
        public void Handle(PBaseSyncPacket packet, ILogicUtil uTils, Device device, Company company)
        {
            // tính toán vận tốc tối đa , vận tốc trung bình , tọa độ

            foreach (var speedLog in packet.SpeedLogs)
            {
                if (device.Temp.MaxSpeed < speedLog)
                    device.Temp.MaxSpeed = speedLog;
            }
        }
    }
}