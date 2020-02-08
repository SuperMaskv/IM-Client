using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Protocol.NoServerPacket;

namespace IM_Client.Protocol.Handler
{
    public class NoServerPacketHandler
    {
        public event Action<Packet> NoServerPacketHandlers;
        public static NoServerPacketHandler INSTANCE = new NoServerPacketHandler();

        public void INVOKE(Packet packet)
        {
            NoServerPacketHandlers?.Invoke(packet);
        }

        private NoServerPacketHandler()
        {
            NoServerPacketHandlers += NoServerLoginPacketHanler;
        }
        private void NoServerLoginPacketHanler(Packet packet)
        {
            if (!(packet is NoServerLoginPacket)) return;


        }

    }
}
