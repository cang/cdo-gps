#region header

// /*********************************************************************************************/
// Project :Datacenter.UpdateDataToRoute
// FileName : ClientConnection.cs
// Time Create : 8:34 AM 23/02/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Core.Models;
using Datacenter.Model.Entity;
using DataCenter.Core;
using Log;
using NodeServerPlatform.Core;
using NodeServerPlatform.Packet;

namespace Datacenter.RegisterRoute
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UpdateData : IPartImportsSatisfiedNotification
    {
        [Import] private IDataCenterStoreEvent _dataEvent;

        [Import] private INodeShareHandleTable _handleTable;

        [Import] private ILog _log;

        [Import] private INodeSharePacketTable _packetTable;

        public NodeShareClient Client { get; set; }
        public Guid Id { get; set; }

        private  bool IsConnected = false;

        #region Implementation of IPartImportsSatisfiedNotification

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            // cài đặt các thông tin sự kiện thay đổi dữ liệu trên memory
            // cập nhật sang cho hệ thống router
            try
            {
                _dataEvent.OnAddCompany += _dataEvent_OnAddCompany;
                _dataEvent.OnAddDevice += _dataEvent_OnAddDevice;
                _dataEvent.OnRemoveCompany += _dataEvent_OnRemoveCompany;
                _dataEvent.OnRemoveDevice += _dataEvent_OnRemoveDevice;
                Id = MachineIdFactory.GetMachineId();
            }
            catch (Exception ex)
            {
                _log.Exception("Updata", ex, "");
                throw;
            }
        }

        public void Connect(string ip, int port)
        {
            _log.Info("UpdateData", "Chờ cho đến khi DataStore load xong mới kết nối đến route");
            DataStore.LoadReadyEvent.WaitOne();

            _log.Info("UpdateData", $"Kết nối tới route {ip} : {port}");
            Client = new NodeShareClient(new NodeClientConfig(_log, _packetTable, _handleTable)
            {
                Ip = ip,
                Port = port,
                ReConnect = true
            })
            {LimitReconect = -1};
            Client.OnConected += Client_OnConected;
            Client.Start();
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        //public static IEnumerable<IEnumerable<T>> Split<T>(this List<T> array, int size)
        private IEnumerable<IEnumerable<T>> SplitArray<T>(List<T> array, int size)
        {
            for (var i = 0; i < (float)array.Count / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        private void Client_OnConected()
        {
            IsConnected = true;

            _log.Info("UpdateData", "Client_OnConected");

            _log.Info("UpdateData", "Gửi thông tin đăng ký qua route");
            Client.Send(new P101MyInfo
            {
                Id = Id.ToString(),
                NodeName = UpdateDataToRouterFactory.Config.Name,
                Port = UpdateDataToRouterFactory.Config.MyPort,
                Ip = UpdateDataToRouterFactory.Config.MyIp,
                ReportCount = 1
            });

            _log.Info("UpdateData", "Sync data qua route");

            //_dataEvent.ReSync();
            List<long> serials = _dataEvent.GetAllDeviceSerial();
            List<long> companyids = _dataEvent.GetAllCompanyId();

            _log.Info("UpdateData", $"Total devices: {serials.Count}");

            //do tcp k xử lý nhiều nên cần chẽ ra để gửi

            //Client.Send(new P103AddSerial { SerialList = serials, DataCenterId = Id.ToString() });
            var sendserials_list = SplitArray<long>(serials, 500);
            foreach (var sendserials in sendserials_list)
                Client.Send(new P103AddSerial { SerialList = sendserials.ToList(), DataCenterId = Id.ToString() });

            //Client.Send(new P105AddCompanyId { DataCenterId = Id.ToString(), CompanyIdList = companyids });
            var sendcompanies_list = SplitArray<long>(companyids, 500);
            foreach (var sendcompanies in sendcompanies_list)
                Client.Send(new P105AddCompanyId { DataCenterId = Id.ToString(), CompanyIdList = sendcompanies.ToList() });

            _log.Info("UpdateData", "DONE Sync data qua route");
        }

        private void _dataEvent_OnRemoveDevice(Device obj)
        {
            if (!IsConnected) return;
            _log.Debug("Updata", $"Xóa bỏ thiết bị {obj.Serial}");
            Client.Send(new P104RemoveSerial {SerialList = new List<long> {obj.Serial}});
        }

        private void _dataEvent_OnRemoveCompany(Company obj)
        {
            if (!IsConnected) return;
            _log.Debug("Updata",$"Xóa bỏ công ty {obj.Id}");
            Client.Send(new P106RemoveCompanyId {CompanyIdList = new List<long> {obj.Id}});
        }

        private void _dataEvent_OnAddDevice(Device obj)
        {
            if (!IsConnected) return;
            _log.Debug("Updata", $"Thêm thiết bị {obj.Serial}");
            Client.Send(new P103AddSerial {SerialList = new List<long> {obj.Serial}, DataCenterId = Id.ToString()});
        }

        private void _dataEvent_OnAddCompany(Company obj)
        {
            if (!IsConnected) return;
            _log.Debug("Updata", $"Thêm công ty {obj.Id}");
            Client.Send(new P105AddCompanyId {DataCenterId = Id.ToString(), CompanyIdList = new List<long> {obj.Id}});
        }

        #endregion
    }
}