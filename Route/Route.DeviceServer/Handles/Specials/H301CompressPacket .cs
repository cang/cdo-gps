#region header

// **********************************************************************
// SOLUTION: StarSg
// PROJECT: Route.DeviceServer
// TIME CREATE : 11:00 PM 03/10/2016
// FILENAME: H301CompressPacket .cs
// AUTHOR: Cang Do (dovancang@gmail.com)
// -----------------------------------
// Copyrights 2016  - All Rights Reserved.
// **********************************************************************

#endregion

#region

using System;
using System.ComponentModel.Composition;
using CorePacket;
using CorePacket.Utils;
using DevicePacketModels.Specials;
using Log;
using ServerCore;

#endregion

namespace Route.DeviceServer.Handles.Specials
{
    [Export(typeof (IDeviceHandlePacket))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [DeviceOpCode(301)]
    public class H301CompressPacket : IDeviceHandlePacket
    {
        [Import] private ILog _log;
        [Import] private StatisticMemoryLog _mlog;
        

        [Import] private IDevicePacketTable _packetTable;
        [Import] private IDevicePacketHandleTable _handleTable;
        public Delegate GetHandle()
        {
            return new Action<IClient, P301CompressPacket>(Handle);
        }

        private void Handle(IClient client, P301CompressPacket p)
        {
            _log.Debug("PACKET", $"Xử lý gói tin 301 : Serial {p.Serial} chứa {p.Datas.Count} gói tin");

            //if(p.InvalidCheckSum>0)
            //{
            //    _log.Error("PACKET", $"Xử lý gói tin 301 : Serial {p.Serial} chứa Checksum sai {p.InvalidCheckSum}/{p.Datas.Count}");
            //}

            try
            {
                bool hasOpcodeZero = false;
                //foreach (var packet in p.Datas)
                int lastidx = p.Datas.Count - 1;
                for (int i = 0; i <= lastidx; i++)
                {
                    var packet = p.Datas[i];
                    _log.Debug("PACKET", $"301  : xử lý gói opcode {packet.Opcode}");
                    var pType = _packetTable.GetPacket(packet.Opcode);
                    if (pType == null)
                    {
                        if (packet.Opcode == 0)
                        {
                            if (_mlog != null) _mlog.UpdateOpcode301Zero(p.Serial);
                            else _log.Error("PACKET", $"Chưa định nghĩa gói tin : Serial {p.Serial} có opcode {packet.Opcode}");
                            if (!hasOpcodeZero) hasOpcodeZero = true;
                        }
                        else
                            _log.Error("PACKET", $"Chưa định nghĩa gói tin : Serial {p.Serial} có opcode {packet.Opcode}");

                        continue;
                    }
                    var pInstance = (IDeviceRecvPacket)Activator.CreateInstance(pType, packet.Data);
                    if (pInstance == null || pInstance.Deserializer() == false)
                    {
                        _log.Error("PACKET", $"Phân tích gói tin {packet.Opcode} không thành công");
                        continue;
                    }
                    var pHandle = _handleTable.GetHandle(packet.Opcode);
                    if (pHandle == null)
                    {
                        _log.Warning("PACKET", $"Chưa có lớp xử lý cho gói tin {packet.Opcode}");

                        //Send(new P02ResponsePacket(ResponseStatus.Error));
                    }
                    else
                    {
                        try
                        {
                            if (packet.Opcode == 1) //gọi cụ thể cho gói 01 // && pHandle is H01SyncPacket && pInstance is DevicePacketModels.P01SyncPacket 
                                (pHandle as H01SyncPacket).Handle301(client, pInstance as DevicePacketModels.P01SyncPacket, i == lastidx);
                            else
                                pHandle.GetHandle().DynamicInvoke(client, pInstance);

                            _log.Success("PACKET",
                                $"Xử lý thông tin gói {packet.Opcode}--{pInstance.GetType()}   Thành công");
                        }
                        catch (Exception pHandleEx)
                        {
                            _log.Exception("PACKET", pHandleEx,
                                $"Có lỗi trong quá trình xử lý gói tin {packet.Opcode}");
                        }
                    }
                }

                if (hasOpcodeZero && _mlog != null)
                {
                    _mlog.UpdateOpcode301Only(p.Serial);
                }
            }
            catch (Exception ex)
            {
                _log.Exception("PACKET", ex,$"H301CompressPacket Handle");
                throw ex;
            }

        }
    }
}