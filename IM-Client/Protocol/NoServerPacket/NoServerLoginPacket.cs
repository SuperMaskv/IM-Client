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
        public byte[] Avator { get; set; }
        public int Port { get; set; }


        public override byte getPacketType()
        {
            return PacketType.NO_SERVER_LOGIN;
        }
    }
}
