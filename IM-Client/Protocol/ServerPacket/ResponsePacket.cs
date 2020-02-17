using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol.ServerPacket
{
    public class ResponsePacket : Packet
    {
        public bool status { get; set; }
        public string info { get; set; }
        public byte packetType { get; set; }


        public override byte getPacketType()
        {
            return PacketType.SERVER_RESPONSE;
        }
    }
}
