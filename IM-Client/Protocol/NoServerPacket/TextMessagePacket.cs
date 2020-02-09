using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol.NoServerPacket
{
    public class TextMessagePacket : Packet
    {
        public string TextMessage { get; set; }
        public string Author { get; set; }


        public override byte getPacketType()
        {
            return PacketType.NO_SERVER_TEXT_MSG;
        }
    }
}
