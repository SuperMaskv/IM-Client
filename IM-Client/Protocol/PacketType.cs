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

        //无服务器图片消息
        public static readonly byte NO_SERVER_PIC_MSG = 0x04;

        //无服务器发送文件
        public static readonly byte NO_SERVER_SEND_FILE = 0x05;
    }
}