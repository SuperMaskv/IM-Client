using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol.ServerPacket
{
    public class OnlineContactPacket : Packet
    {
        public List<string> onlineContacts { get; set; }

        public override byte getPacketType()
        {
            return PacketType.ONLINE_CONTACT;
        }
    }
}
