#region include

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CorePacket;
using DevicePacketModels;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols;
using Hik.Communication.Scs.Server;
using Log;
using Route.Core;
using ServerCore;
using CorePacket.Utils;

#endregion

namespace Route.DeviceServer
{
    public class TextWireProtocol : IScsWireProtocol
    {
        public TextWireProtocol()
        {
        }

        /// <summary>
        ///     Serializes a message to a byte array to send to remote application.
        ///     This method is synchronized. So, only one thread can call it concurrently.
        /// </summary>
        /// <param name="message">Message to be serialized</param>
        public byte[] GetBytes(IScsMessage message)
        {
            if (message is ScsRawDataMessage)
            {
                var mes = message as ScsRawDataMessage;
                return mes.MessageData;
            }
            return new byte[0];
        }

        /// <summary>
        ///     Builds messages from a byte array that is received from remote application.
        ///     The Byte array may contain just a part of a message, the protocol must
        ///     cumulate bytes to build messages.
        ///     This method is synchronized. So, only one thread can call it concurrently.
        /// </summary>
        /// <param name="receivedBytes">Received bytes from remote application</param>
        /// <returns>
        ///     List of messages.
        ///     Protocol can generate more than one message from a byte array.
        ///     Also, if received bytes are not sufficient to build a message, the protocol
        ///     may return an empty list (and save bytes to combine with next method call).
        /// </returns>
        public IEnumerable<IScsMessage> CreateMessages(byte[] receivedBytes)
        {
            //Create a list to collect messages
            var messages = new List<IScsMessage>();
            var mes = new ScsRawDataMessage(receivedBytes);
            //Console.WriteLine(mes.Text);
            messages.Add(mes);
            //Return message list
            return messages;
        }

        /// <summary>
        ///     This method is called when connection with remote application is reset (connection is renewing or first
        ///     connecting).
        ///     So, wire protocol must reset itself.
        /// </summary>
        public void Reset()
        {
        }
    }

    public class ClientConnect : IClient
    {
        private readonly IClientCachePacket _cachePacket;
        private readonly IScsServerClient _client;
        private readonly IDeviceRouteTable _deviceRoute;
        private readonly IDevicePacketHandleTable _handleTable;
        private readonly ILog _log;
        private readonly IDevicePacketTable _packetTable;

        public ClientConnect(IScsServerClient client, ILog log, IDevicePacketTable packetTable,
            IDevicePacketHandleTable handleTable, IClientCachePacket cachePacket, IDeviceRouteTable deviceRoute)
        {
            _client = client;
            _log = log;
            _packetTable = packetTable;
            _handleTable = handleTable;
            _cachePacket = cachePacket;
            _deviceRoute = deviceRoute;

            _client.Tag = this;
            _client.WireProtocol = new TextWireProtocol();
            _client.MessageReceived += _client_MessageReceived;

            //Send(new P02ResponsePacket(ResponseStatus.NotImplement));
            //try
            //{
            //    String slog = $"DEVICE CONNECT ClientId {_client.ClientId}  RemoteEndPoint ";
            //    if (_client.RemoteEndPoint != null)
            //        slog += _client.RemoteEndPoint;
            //    _log.Info("PACKET", slog);
            //}
            //catch (Exception)
            //{
            //}
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public string Name { get; set; }

        public void Send(int opcode,IDeviceSendPacket p)
        {
            //gửi trả lời cho các gói tin khác gói 10 vì gói 10 không cần nhận trả lời
            if (opcode!=10)
                Send(p);
        }

        public void Send(IDeviceSendPacket p)
        {
            try
            {
                var pdata = p.Serializer();
                var opcode = _packetTable.GetOpcode(p.GetType());
                if (opcode <= 0)
                {
                    _log.Error("PACKET", $"Chưa định nghĩa opcode cho gói tin này : {p.GetType()}");
                    return;
                }

                using (var memory = new MemoryStream())
                {
                    using (var w = new BinaryWriter(memory))
                    {
                        w.Write((short)opcode);
                        w.Write((short)pdata.Length);
                        w.Write(pdata);
                        //w.Write(new Crc32().ComputeChecksum(pdata));
                        w.Write(Crc32.ComputeChecksum(pdata));
                        var tmp = memory.ToArray();
                        _client.SendMessage(new ScsRawDataMessage(tmp));
                    }
                }
                //_log.Debug("PACKET", $"DATA PACKET SEND : {BitConverter.ToString(tmp).Replace("-", "")}");
               // _log.Success("PACKET", $"Send thành công gói tin opcode {opcode} -- {p.GetType()}  xuống thiết bị");
                //throw new NotImplementedException();
            } 
            catch (Exception ex)
            {
                _log.Exception("PACKET", ex, "Send dữ liệu cho thiết bị lỗi ");
            }
        }

        private long getSerialOfPackage(IDevicePacket packet)
        {
            if (packet == null) return 0;
            if (packet.Data == null) return 0;
            try
            {
                using (MemoryStream ms = new MemoryStream(packet.Data))
                {
                    using (BinaryReader r = new BinaryReader(ms))
                    {
                        return r.ReadInt64();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Exception("PACKET", ex, "getSerialOfPackage");
            }
            return 0;
        }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                var mess = e.Message as ScsRawDataMessage;
                if (mess != null)
                {
                    try
                    {
                        //packet = DevicePacketFactory.CreatePacket(mess.MessageData);
                        IDevicePacket packet = null;
                        try
                        {
                            packet = DevicePacketFactory.CreatePacket(mess.MessageData);
                        }
                        catch (DevicePacketFactory.ChecksumException ex1)
                        {
                            _log.Exception("PACKET", ex1, $"CreatePacket Checksum {ex1.Opcode} {ex1.Serial}");
                            Send(new P02ResponsePacket(ResponseStatus.Error));
                            return;
                        }
                        catch (Exception ex)
                        {
                            _log.Exception("PACKET", ex, $"CreatePacket bị lỗi k đúng format <2+2+data>");
                            Send(new P02ResponsePacket(ResponseStatus.Error));
                            return;
                        }

                        var pType = _packetTable.GetPacket(packet.Opcode);
                        if (pType == null)
                        {

                            _log.Error("PACKET", $"Chưa định nghĩa gói tin có opcode {packet.Opcode} serial {getSerialOfPackage(packet)}");
                            Send(packet.Opcode, new P02ResponsePacket(ResponseStatus.InvalidOpCode));
                        }
                        else
                        {
                            try
                            {
                                var pInstance = (IDeviceRecvPacket) Activator.CreateInstance(pType, packet.Data);
                                if (pInstance == null || pInstance.Deserializer() == false)
                                {
                                    _log.Error("PACKET", $"Phân tích gói tin {packet.Opcode}  serial {getSerialOfPackage(packet)} không thành công");
                                    Send(packet.Opcode, new P02ResponsePacket(ResponseStatus.Error));
                                }
                                else
                                {
                                    // lấy thông tin lớp logic xử lý
                                    var serial = pInstance.Serial;

                                    if (!_deviceRoute.Check(serial))
                                    {
                                        _log.Warning("PACKET",
                                            $"Thiết bị chưa khai báo : {serial} -- {pInstance.GetType()}");

                                        #region xử lý thêm vào tạm để hiển thị lên map
                                        if (pInstance.GetType() == typeof(P01SyncPacket) && _cachePacket is ClientCachePacket)
                                        {
                                            try
                                            {
                                                (_cachePacket as ClientCachePacket).UpdateUnknownDevice(serial, pInstance as P01SyncPacket);
                                            }
                                            catch (Exception ex)
                                            {
                                                _log.Exception("PACKET", ex, $"Lỗi thêm Unknown device {serial}");
                                            }
                                        }
                                        #endregion xử lý thêm vào tạm

                                        Send(packet.Opcode, new P02ResponsePacket(ResponseStatus.NotFound));
                                        return;
                                    }
                                    else
                                    {
                                        #region kiểm tra và xử lý xóa khỏi tạm nếu tồn tại
                                        try
                                        {
                                            _cachePacket.TryRemoveUnknownDevice(serial);
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.Exception("PACKET", ex, $"Lỗi xóa Unknown device {serial}");
                                        }
                                        #endregion 
                                    }

                                    _log.Debug("PACKET", $"Xử lý thông tin cho gói {pInstance.GetType()}");
                                    var pHandle = _handleTable.GetHandle(packet.Opcode);
                                    if (pHandle == null)
                                    {
                                        //_log.Warning("PACKET", $"Chưa có lớp xử lý cho gói tin {packet.Opcode}");
                                        _log.Error("PACKET", $"Chưa có lớp xử lý cho gói tin {packet.Opcode} {serial}");
                                        Send(packet.Opcode, new P02ResponsePacket(ResponseStatus.Error));
                                    }
                                    else
                                    {
                                        //Process received package and get command stack data back
                                        try
                                        {
                                            pHandle.GetHandle().DynamicInvoke(this, pInstance);
                                            _log.Success("PACKET",
                                                $"Xử lý thông tin gói {packet.Opcode}--{pInstance.GetType()}   Thành công");
                                        }
                                        catch (Exception pHandleEx)
                                        {
                                            _log.Exception("PACKET", pHandleEx,
                                                $"Có lỗi trong quá trình xử lý gói tin {packet.Opcode} {serial}");
                                        }

                                        //Send command in command stack to device {serial}
                                        do
                                        {
                                            //thật ra là thiết bị chỉ nhận được 1 lệnh tại 1 thời điểm mà thôi
                                            var result = _cachePacket.Pop(serial);
                                            if (result == null) break;
                                            if (result.Length > 0)
                                            {
                                                _client.SendMessage(new ScsRawDataMessage(result));
                                                _log.Success("PACKET"
                                                    , $"Send setup {serial} {BitConverter.ToString(result).Replace("-", "")}");
                                            }
                                        } while (true);

                                        //Send Ok for client check and close connection
                                        Send(packet.Opcode, new P02ResponsePacket(ResponseStatus.Ok));

                                    }//end if pHandle

                                }
                            }
                            catch (EndOfStreamException)
                            {
                                _log.Error("PACKET", $"Phân tích gói tin {packet.Opcode} không thành công, lỗi EndOfStream");
                                Send(packet.Opcode, new P02ResponsePacket(ResponseStatus.Error));
                            }
                            catch (Exception ex)
                            {
                                _log.Exception("PACKET", ex, $"Building packet opcode {packet.Opcode} lỗi");
                                Send(packet.Opcode,new P02ResponsePacket(ResponseStatus.Error));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Exception("PACKET", ex, "Phân tích packet lỗi ");
                        Send(new P02ResponsePacket(ResponseStatus.Error));
                    }
                }
                else Send(new P02ResponsePacket(ResponseStatus.InvalidData));
            }
            catch (Exception ex)
            {
                _log.Exception("EX", ex, "");
                Send(new P02ResponsePacket(ResponseStatus.Error));
            }
        }


    }
}