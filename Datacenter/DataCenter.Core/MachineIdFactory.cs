#region header
// /*********************************************************************************************/
// Project :DataCenter.Core
// FileName : MachineIdFactory.cs
// Time Create : 3:40 PM 21/03/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace DataCenter.Core
{
    public static class MachineIdFactory
    {
        public static Guid GetMachineId()
        {
            // Tạo thông tin định danh từ thông tin phần cứng
            var drive = "C";
            var dsk = new ManagementObject(
                @"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            var volumeSerial = dsk["VolumeSerialNumber"].ToString();

            var cpuInfo = string.Empty;
            var mc = new ManagementClass("win32_processor");
            var moc = mc.GetInstances();

            foreach (var mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }

            var uniqueId = cpuInfo + volumeSerial;
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(uniqueId));
                return new Guid(hash);
            }
        }
    }
}