using IM_Client.Protocol;
using System;
using System.Net;

namespace IM_Client.Services
{
    public interface IChatService
    {
        event Action<Packet> BroadcastPacket;

        event Action<Packet, IPEndPoint> UnicastPacket;

        event Action UdpListener;

        void InvokeBroadcastPacketEvent(Packet packet);

        void InvokeUnicastPacketEvent(Packet packet, IPEndPoint iPEndPoint);
    }
}