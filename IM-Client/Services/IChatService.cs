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
        event Action<Packet> _broadcastPacket;
        event Action<Packet, IPEndPoint> _unicastPacket;

        Task UdpListen();
    }
}
