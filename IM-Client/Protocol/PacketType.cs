using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM_Client.Protocol
{
    public class PacketType
    {
        //无服务器登录报文
        public static readonly byte NO_SERVER_LOGIN = 0x01;
        //无服务器注销报文
        public static readonly byte NO_SERVER_LOGOUT = 0x02;
        //无服务器文字消息
        public static readonly byte NO_SERVER_TEXT_MSG = 0x03;
    }
}
