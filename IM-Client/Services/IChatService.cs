using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Protocol;
using System.Net;
using System.Net.Sockets;

namespace IM_Client.Services
{
    public interface IChatService
    {
        event Action<Packet> BroadcastPacket;
        event Action<Packet, IPEndPoint> UnicastPacket;
        event Action UdpListener;

        void InvokeBroadcastPacketEvent(Packet packet);
        void InvokeUnicastPacketEvent(Packet packet,IPEndPoint iPEndPoint);
    }
}
