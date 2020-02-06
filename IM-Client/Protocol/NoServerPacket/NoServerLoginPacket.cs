using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Protocol;

namespace IM_Client.Protocol.NoServerPacket
{
    public class NoServerLoginPacket : Packet
    {
        public string UserName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }

        public override byte getPacketType()
        {
            return Packet.NO_SERVER_LOGIN;
        }
    }
}
