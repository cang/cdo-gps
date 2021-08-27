#region header

// /*********************************************************************************************/
// Project :Route.DeviceServer
// FileName : H01SyncPacket.cs
// Time Create : 8:13 AM 12/07/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

#region include

using System;
using System.ComponentModel.Composition;
using System.Linq;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels;
using Log;
using ServerCore;

#endregion

namespace Route.DeviceServer.Handles
{
    /// <summary>
    ///     lớp xử lý thông tin packet 10
    /// </summary>
    [Export(typeof(IDeviceHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(10)]
    public class H10SyncPacket : BaseHandle, IDeviceHandlePacket
    {
        [Import]
        private ILog _log;

        public Delegate GetHandle()
        {
            return new Action<IClient, P10SyncPacket>(Handle);
        }

        private void Handle(IClient client, P10SyncPacket p)
        {
            _log.Warning("PACKET", $"Serial : {p.Serial}");
            _log.Warning("PACKET", $"Time : {p.Time.ToString("G")}");
            _log.Warning("PACKET", $"GpsStatus : {p.GpsStatus}");
            if (p.GpsStatus)
            {
                _log.Warning("PACKET", $"Lat : {p.GpsInfo.Lat}");
                _log.Warning("PACKET", $"Lng : {p.GpsInfo.Lng}");
                _log.Warning("PACKET", $"Speed : {p.GpsInfo.Speed}");
            }
            _log.Warning("PACKET", $"TotalGpsDistance : {p.TotalGpsDistance}");
            _log.Warning("PACKET", $"TotalCurrentGpsDistance : {p.TotalCurrentGpsDistance}");
            _log.Warning("PACKET", $"IOStatus : {p.IOValue}");
            _log.Warning("PACKET", $"    Chì khóa : {p.StatusIo.Key}");
            _log.Warning("PACKET", $"    Máy lạnh : {p.StatusIo.AirMachine}");
            _log.Warning("PACKET", $"    Sos : {p.StatusIo.Sos}");
            _log.Warning("PACKET", $"    Sử dụng cảm biến nhiệt : {p.StatusIo.UseTemperature}");
            _log.Warning("PACKET", $"    Sử dụng cảm biến nhiên liệu : {p.StatusIo.UseFuel}");
            _log.Warning("PACKET", $"    Sử dụng RFID : {p.StatusIo.UseRfid}");
            _log.Warning("PACKET", $"    Cửa : {p.StatusIo.Door}");

            _log.Warning("PACKET", $"DriverId: {p.DriverId}");
            _log.Warning("PACKET", $"Fuel: {p.Fuel}");
            _log.Warning("PACKET", $"Temperature: {p.Temperature}");
            _log.Warning("PACKET", $"GsmSignal: {p.GsmSignal}");
            _log.Warning("PACKET", $"Power: {p.Power}");
            var speeds = p.SpeedLogs.Aggregate("", (current, speedLog) => current + $"{speedLog}   ");
            _log.Warning("PACKET", $"SpeedTrace: {speeds}");
            _log.Warning("PACKET", $"OverTime: {p.TimeWork}");
            _log.Warning("PACKET", $"OverTimeInDay: {p.TimeWorkInDay}");
            _log.Debug("PACKET",
                $"Chuyển tiếp thông tin qua máy chủ xử lý : {(ForwardSysn10(p.Serial, p) ? "thành công" : "thất bại")}");
        }
    }
}