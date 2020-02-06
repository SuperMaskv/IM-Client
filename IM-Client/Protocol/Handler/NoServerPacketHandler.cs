using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Protocol.NoServerPacket;

namespace IM_Client.Protocol.NoServerHandler
{
    public class NoServerPacketHandler
    {
        private static event Action<Packet> NoServerPacketHandlers;
        public static NoServerPacketHandler INSTANCE = new NoServerPacketHandler();

        private NoServerPacketHandler()
        {
            NoServerPacketHandlers += NoServerLoginPacketHanler;
        }
        public void NoServerLoginPacketHanler(Packet packet)
        {
            if (!(packet is NoServerLoginPacket)) return;


        }

    }
}
