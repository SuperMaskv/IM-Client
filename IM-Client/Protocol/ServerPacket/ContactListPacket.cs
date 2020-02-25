using IM_Client.Models;
using System.Collections.Generic;

namespace IM_Client.Protocol.ServerPacket
{
    public class ContactListPacket : Packet
    {
        public List<Contact> contacts { get; set; }

        public override byte getPacketType()
        {
            return PacketType.CONTACT_LIST;
        }
    }
}