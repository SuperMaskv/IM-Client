using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol.NoServerPacket
{
    public class NoServerPicMsgPacket : Packet
    {
        public string Author { get; set; }
        public byte[] Pic { get; set; }


        public override byte getPacketType()
        {
            return PacketType.NO_SERVER_PIC_MSG;
        }
    }
}
