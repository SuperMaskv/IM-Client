namespace IM_Client.Protocol
{
    public class PacketType
    {
        //无服务器登录报文
        public static readonly byte NO_SERVER_LOGIN = 0xf1;

        //无服务器注销报文
        public static readonly byte NO_SERVER_LOGOUT = 0xf2;

        //无服务器文字消息
        public static readonly byte NO_SERVER_TEXT_MSG = 0xf3;

        //无服务器图片消息
        public static readonly byte NO_SERVER_PIC_MSG = 0xf4;

        //无服务器发送文件
        public static readonly byte NO_SERVER_SEND_FILE = 0xf5;

        //服务器报文标志
        public static readonly byte SERVER_RESPONSE = 0x00;

        //用户相关
        //用户注册
        public static readonly byte REGISTER = 0X01;

        //用户登录
        public static readonly byte LOGIN = 0x02;

        //用户注销
        public static readonly byte LOGOUT = 0x03;

        //联系人相关
        //添加联系人
        public static readonly byte ADD_CONTACT = 0x04;

        //删除联系人
        public static readonly byte REMOVE_CONTACT = 0x05;

        //修改联系人备注
        public static readonly byte EDIT_CONTACT_ALIAS = 0x06;

        //返回联系人列表
        public static readonly byte CONTACT_LIST = 0x0E;
        //在线联系人报文
        public static readonly byte ONLINE_CONTACT = 0x0F;
        //向联系人通知下线状态
        public static readonly byte OFFLINE_CONTACT = 0x10;

        //群组相关
        //创建群组
        public static readonly byte CREATE_GROUP = 0x07;

        //解散群组
        public static readonly byte DISMISS_GROUP = 0x08;

        //修改群组名称
        public static readonly byte EDIT_GROUP_NAME = 0x09;

        //添加用户到群组
        public static readonly byte ADD_USER = 0x0A;

        //将用户移除群组
        public static readonly byte REMOVE_USER = 0x0B;

        //消息相关
        //私聊数据包
        public static readonly byte TO_USER_MESSAGE = 0x0C;

        //群聊消息数据包
        public static readonly byte TO_GROUP_MESSAGE = 0x0D;
    }
}