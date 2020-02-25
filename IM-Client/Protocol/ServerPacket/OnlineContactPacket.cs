using System.Collections.Generic;

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