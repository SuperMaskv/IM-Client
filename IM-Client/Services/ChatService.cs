using IM_Client.Protocol;
using IM_Client.Protocol.Handler;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IM_Client.Services
{
    public class ChatService : IChatService
    {
        public event Action<Packet> BroadcastPacket;

        public event Action<Packet, IPEndPoint> UnicastPacket;

        public event Action UdpListener;

        public ChatService()
        {
            BroadcastPacket += SendBroadcast;
            UnicastPacket += SendUnicast;
            UdpListener += UdpListen;
        }

        public void InvokeBroadcastPacketEvent(Packet packet)
        {
            BroadcastPacket?.Invoke(packet);
        }

        public void InvokeUnicastPacketEvent(Packet packet, IPEndPoint iPEndPoint)
        {
            UnicastPacket?.Invoke(packet, iPEndPoint);
        }

        private void SendBroadcast(Packet packet)
        {
            UdpClient sendUdpClient = new UdpClient();
            byte[] bytes = PacketCodec.INSTANCE.Encode(packet);
            sendUdpClient.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, 20000));
        }

        private void SendUnicast(Packet packet, IPEndPoint iPEndPoint)
        {
            UdpClient sendUdpClient = new UdpClient();
            byte[] bytes = PacketCodec.INSTANCE.Encode(packet);
            sendUdpClient.Send(bytes, bytes.Length, iPEndPoint);
        }

        public void UdpListen()
        {
            Console.WriteLine("Listener is on.");
            Console.WriteLine(Thread.CurrentThread.Name);
            UdpClient rcvClient = new UdpClient(20000);

            var rcvResult = rcvClient.ReceiveAsync();

            NoServerPacketHandler.INSTANCE.INVOKE(PacketCodec.INSTANCE.Decode(rcvResult.Result.Buffer));
        }
    }
}