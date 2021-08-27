using System;
using CorePacket;

namespace ServerCore
{
    public interface IClient : IDisposable
    {
        string Name { get; set; }
        void Send(IDeviceSendPacket p);
        void Send(int opcode, IDeviceSendPacket p);
    }
}