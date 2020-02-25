using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol.ServerPacket
{
    public class RegisterPacket : Packet
    {
        public string userName { get; set; }
        public string userPwd { get; set; }

        public override byte getPacketType()
        {
            return PacketType.REGISTER;
        }
    }
}
