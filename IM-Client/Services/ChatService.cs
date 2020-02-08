using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using IM_Client.Protocol;
using IM_Client.Protocol.Handler;

namespace IM_Client.Services
{
    public class ChatService:IChatService
    {
        public event Action<Packet> _broadcastPacket;
        public event Action<Packet,IPEndPoint> _unicastPacket;

        public ChatService()
        {
            _broadcastPacket += SendBroadcast;
            _unicastPacket += SendUnicast;
        }

        public void SendBroadcast(Packet packet)
        {
            UdpClient sendUdpClient = new UdpClient();
            byte[] bytes = PacketCodec.INSTANCE.Encode(packet);
            sendUdpClient.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, 20000));
        }

        public void SendUnicast(Packet packet,IPEndPoint iPEndPoint)
        {
            UdpClient sendUdpClient = new UdpClient();
            byte[] bytes = PacketCodec.INSTANCE.Encode(packet);
            sendUdpClient.Send(bytes, bytes.Length, iPEndPoint);
        }

        public async Task UdpListen()
        {
            UdpClient rcvClient = new UdpClient(20000);
            while (true)
            {
                var rcvResult =await rcvClient.ReceiveAsync();

                NoServerPacketHandler.INSTANCE.INVOKE(PacketCodec.INSTANCE.Decode(rcvResult.Buffer));
            }
        }
    }
}
