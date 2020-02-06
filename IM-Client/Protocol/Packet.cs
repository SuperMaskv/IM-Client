using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol
{
    public abstract class Packet
    {
        public readonly byte VERSION = 1;
        //无服务器登录报文
        public static readonly byte NO_SERVER_LOGIN = 0x01;

        public abstract byte getPacketType();
    }
}
