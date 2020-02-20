using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IM_Client.Models;

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
